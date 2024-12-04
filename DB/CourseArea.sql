
CREATE TABLE CourseCategories
(
	CourseCategoryId SMALLINT IDENTITY PRIMARY KEY,
	ParentId	SMALLINT DEFAULT NULL,
	CourseCategoryName VARCHAR(100) NOT NULL UNIQUE,
	Status BIT DEFAULT 0,
	CourseCategoryOrder SMALLINT NOT NULL DEFAULT 1,
	DeletedAt DATETIME DEFAULT NULL,
);


CREATE TABLE Courses
(
	CourseId INT IDENTITY PRIMARY KEY,
	FirmId INT NOT NULL,
	CourseCategoryId SMALLINT NOT NULL,
	CourseName NVARCHAR(100) NOT NULL,
	CourseDescription NVARCHAR(MAX) DEFAULT NULL,
	CourseLevel VARCHAR(30) NOT NULL DEFAULT 'Beginner',
	Status BIT DEFAULT 0,
	CourseOrder SMALLINT NOT NULL DEFAULT 1,
	DeletedAt DATETIME DEFAULT NULL,
	CONSTRAINT ENUM_Courses_CourseLevel CHECK (CourseLevel in ('Beginner', 'Intermediate','Expert'))
);

CREATE TABLE CourseFees
(
	CourseFeeId INT IDENTITY PRIMARY KEY,
	CourseId INT,
	TotalInstallments TINYINT NOT NULL,
	FeeAmount float NOT NULL DEFAULT 0,
	GstPercentage float NOT NULL DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL,

	CONSTRAINT FK_CourseFees_CourseId FOREIGN KEY (CourseId) REFERENCES Courses(CourseId)
);


CREATE type CourseFees_type AS TABLE
(
	TotalInstallments TINYINT NOT NULL,
	FeeAmount float NOT NULL,
	GstPercentage float NOT NULL
);



CREATE TABLE CourseModules
(
	CourseModuleId INT IDENTITY PRIMARY KEY,
	CourseId INT DEFAULT NULL,
	ModuleName VARCHAR(250) NOT NULL,
	ModuleDescription NVARCHAR(MAX) DEFAULT NULL,
	ModuleOrder SMALLINT NOT NULL DEFAULT 1,
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL,
	CONSTRAINT FK_CourseModules_CourseId FOREIGN KEY (CourseId) REFERENCES Courses(CourseId)
)

CREATE TABLE CourseModuleContents
(
	CourseModuleContentId INT IDENTITY PRIMARY KEY,
	CourseModuleId INT NOT NULL,
	ContentName VARCHAR(250) NOT NULL,
	ContentDescription NVARCHAR(MAX) DEFAULT NULL,
	DurationInHrs TINYINT,
	ContentOrder SMALLINT NOT NULL DEFAULT 1,
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_GetAll_CourseCategories
	AS
	BEGIN
	 SELECT 
			c.CourseCategoryId,
			c.ParentId,
			c.CourseCategoryName,
			ISNULL(pc.CourseCategoryName, '-') AS ParentCategoryName, -- Parent category name or '-' if null
			c.Status,
			CASE 
				WHEN c.Status = 1 THEN 'Active' 
				WHEN c.Status = 0 THEN 'Inactive' 
				ELSE 'Unknown'
			END AS StatusLabel,  -- Custom StatusLabel column
			NULL AS ParentCategoryCourseCategoryId -- Include this if using navigation properties
		FROM 
			CourseCategories c
		LEFT JOIN 
			CourseCategories pc ON c.ParentId = pc.CourseCategoryId
		WHERE 
			c.DeletedAt IS NULL
		ORDER BY c.CourseCategoryOrder DESC
	END;


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_CourseCategories
(
	@Type VARCHAR(10),
	@CourseCategoryId INT,
    @ParentId INT NULL,
    @CourseCategoryName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO CourseCategories( ParentId,CourseCategoryName, Status) values ( @ParentId, @CourseCategoryName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM CourseCategories WHERE CourseCategoryId = @CourseCategoryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CourseCategoryId. The specified CourseCategoryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE CourseCategories SET 
					ParentId = @ParentId,
					CourseCategoryName = @CourseCategoryName,
					Status = @Status
					WHERE CourseCategoryId = @CourseCategoryId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM CourseCategories WHERE CourseCategoryId = @CourseCategoryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CourseCategoryId. The specified CourseCategoryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE CourseCategories SET DeletedAt = GETDATE() WHERE CourseCategoryId = @CourseCategoryId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE CourseCategories SET DeletedAt = NULL WHERE CourseCategoryId = @CourseCategoryId
			END	
END;


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Courses
(
	@Type VARCHAR(10),
	@CourseId INT = 0,
	@FirmId INT = 0,
	@CourseCategoryId INT = 0,
    @CourseName VARCHAR(100) = '',
	@CourseDescription VARCHAR(MAX) = '',
	@CourseLevel VARCHAR(20) = 'Beginner',
	@Status BIT = 0
)
AS
BEGIN

		IF @Type = 'INSERT'
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM CourseCategories WHERE CourseCategoryId = @CourseCategoryId AND DeletedAt IS NULL)
			BEGIN
				RAISERROR('Invalid CourseCategoryId. The specified CourseCategoryId does not exist.', 16, 1)
				RETURN
			END

			INSERT INTO Courses( FirmId, CourseCategoryId, CourseName, CourseDescription, CourseLevel, Status) values (@FirmId, @CourseCategoryId, @CourseName, @CourseDescription, @CourseLevel, @Status)
		END

		IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
		BEGIN
			IF NOT EXISTS (SELECT 1 FROM CourseCategories WHERE CourseCategoryId = @CourseCategoryId AND DeletedAt IS NULL)
			BEGIN
				RAISERROR('Invalid CourseCategoryId. The specified CourseCategoryId does not exist.', 16, 1)
				RETURN
			END

				UPDATE Courses SET 
				CourseCategoryId = @CourseCategoryId,
				CourseName = @CourseName,
				CourseDescription = @CourseDescription,
				CourseLevel = @CourseLevel,
				Status = @Status
				WHERE CourseId = @CourseId AND  DeletedAt IS NULL
		END

		IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
		BEGIN
			UPDATE Courses SET DeletedAt = GETDATE() WHERE CourseId = @CourseId AND DeletedAt IS NULL
		END

		IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
		BEGIN
			UPDATE Courses SET DeletedAt = NULL WHERE CourseId = @CourseId AND DeletedAt IS NOT NULL
		END

		IF @Type = 'STATUS'			-- RESTORE BY CourseFeesId
		BEGIN
			UPDATE Courses SET Status = Status WHERE CourseId = @CourseId AND DeletedAt IS NULL
		END

END;


CREATE PROCEDURE sp_Create_CourseWithFees
(
	@CourseId INT OUTPUT,
	@FirmId INT = 0,
	@CourseCategoryId INT,
    @CourseName VARCHAR(100),
	@CourseDescription VARCHAR(MAX) = '',
	@CourseLevel VARCHAR(20) = 'Beginner',
	@Status BIT = 0,
	@CourseFeesList CourseFees_type READONLY
)
AS
	BEGIN TRY
		BEGIN TRANSACTION
			INSERT INTO Courses(FirmId, CourseCategoryId, CourseName, CourseDescription, CourseLevel, Status) 
			values (@FirmId, @CourseCategoryId, @CourseName, @CourseDescription, @CourseLevel, @Status)
			SET @CourseId = SCOPE_IDENTITY();

			INSERT INTO CourseFees(CourseId, TotalInstallments, FeeAmount, GstPercentage)
			SELECT @CourseId, TotalInstallments, FeeAmount, GstPercentage FROM @CourseFeesList
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
				RAISERROR('Invalid FeesDetails. Please check given Fees Details".', 16, 1)
	END CATCH;



CREATE PROCEDURE sp_GetAll_CourseFees
(
    @CourseId INT = 0,
    @CourseFeeId INT = 0
)
AS
BEGIN
		IF (@CourseFeeId <> 0)
		BEGIN
			SELECT 
				cf.CourseFeeId, 
				cf.CourseId, 
				c.CourseName, 
				cf.TotalInstallments, 
				cf.FeeAmount, 
				cf.GstPercentage, 
				(cf.FeeAmount * cf.GstPercentage / 100) AS GstAmount,  -- Calculating GST Amount
				(cf.FeeAmount + (cf.FeeAmount * cf.GstPercentage / 100)) AS TotalAmountWithGst  -- Calculating Total Amount including GST
			FROM 
				CourseFees cf
				JOIN Courses c ON c.CourseId = cf.CourseId

			WHERE 
				cf.DeletedAt IS NULL
				AND c.DeletedAt IS NULL
				AND cf.CourseFeeId = @CourseFeeId
		END
		ELSE
		BEGIN

			SELECT 
				cf.CourseFeeId, 
				cf.CourseId, 
				c.CourseName, 
				cf.TotalInstallments, 
				cf.FeeAmount, 
				cf.GstPercentage, 
				(cf.FeeAmount * cf.GstPercentage / 100) AS GstAmount,  -- Calculating GST Amount
				(cf.FeeAmount + (cf.FeeAmount * cf.GstPercentage / 100)) AS TotalAmountWithGst  -- Calculating Total Amount including GST
			FROM 
				CourseFees cf
				JOIN Courses c ON c.CourseId = cf.CourseId

			WHERE 
				cf.DeletedAt IS NULL
				AND c.DeletedAt IS NULL
				AND (@CourseId = 0 OR cf.CourseId = @CourseId)  -- Optional filter for CourseId
		END
END;




CREATE PROCEDURE sp_GetAll_Course
(
	@CourseId INT = 0,
	@CourseCategoryId INT = 0
)
AS
	BEGIN
		IF (@CourseId <> 0)
			BEGIN
				SELECT 
					c.CourseId,
					c.CourseCategoryId,
					cc.CourseCategoryName,
					c.CourseName,
					c.CourseDescription,
					c.CourseLevel,
					c.CourseOrder,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM Courses c
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND c.CourseId = @CourseId
				ORDER BY c.CourseOrder ASC
			END
		ELSE IF (@CourseCategoryId <> 0)
			BEGIN
				SELECT 
					c.CourseId,
					c.CourseCategoryId,
					cc.CourseCategoryName,
					c.CourseName,
					c.CourseDescription,
					c.CourseLevel,
					c.CourseOrder,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM Courses c
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND c.CourseCategoryId = @CourseCategoryId
				ORDER BY c.CourseOrder ASC

			END
		ELSE
			BEGIN
				SELECT 
					c.CourseId,
					c.CourseCategoryId,
					cc.CourseCategoryName,
					c.CourseName,
					c.CourseDescription,
					c.CourseLevel,
					c.CourseOrder,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM Courses c
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
				ORDER BY c.CourseOrder ASC

			END
	END;



	
CREATE PROCEDURE sp_CreateUpdateDeleteRestore_CourseModules
(
	@Type VARCHAR(10),
	@CourseModuleId INT = 0,
    @CourseId INT = 0,
    @ModuleName VARCHAR(100) = '',
    @ModuleDescription NVARCHAR(MAX) = '',
	@ModuleOrder INT = 1,
	@Status BIT = 0
)
AS
BEGIN
	IF @Type = 'INSERT'
	BEGIN
		-- Check if CourseId exists and is not deleted
		IF NOT EXISTS (SELECT 1 FROM Courses WHERE CourseId = @CourseId AND DeletedAt IS NULL)
		BEGIN
			RAISERROR('Invalid CourseId. The specified CourseId does not exist.', 16, 1);
			RETURN;
		END
		
		-- Insert a new Course Module
		INSERT INTO CourseModules(CourseId, ModuleName, ModuleDescription, ModuleOrder, Status) 
		VALUES (@CourseId, @ModuleName, @ModuleDescription, @ModuleOrder, @Status);
	END

	ELSE IF @Type = 'UPDATE'
	BEGIN
		-- Check if CourseModuleId exists and is not deleted
		IF NOT EXISTS (SELECT 1 FROM Courses WHERE CourseId = @CourseId AND DeletedAt IS NULL)
		BEGIN
				RAISERROR('Invalid CourseId. The specified CourseId does not exist.', 16, 1);
			RETURN;
		END
		
		-- Update an existing Course Module
		UPDATE CourseModules 
		SET CourseId = @CourseId,
		    ModuleName = @ModuleName,
			ModuleDescription = @ModuleDescription,
		    ModuleOrder = @ModuleOrder,
		    Status = @Status
		WHERE CourseModuleId = @CourseModuleId AND DeletedAt IS NULL;
	END

	ELSE IF @Type = 'DELETE'
	BEGIN
		-- Check if CourseModuleId exists and is not deleted
		IF NOT EXISTS (SELECT 1 FROM CourseModules WHERE CourseModuleId = @CourseModuleId AND DeletedAt IS NULL)
		BEGIN
			RAISERROR('Invalid CourseModuleId. The specified CourseModuleId does not exist.', 16, 1);
			RETURN;
		END

		-- Soft delete the Course Module
		UPDATE CourseModules 
		SET DeletedAt = GETDATE() 
		WHERE CourseModuleId = @CourseModuleId AND DeletedAt IS NULL;
	END

	ELSE IF @Type = 'RESTORE'
	BEGIN
		-- Restore a soft-deleted Course Module
		UPDATE CourseModules 
		SET DeletedAt = NULL 
		WHERE CourseModuleId = @CourseModuleId;
	END
END;


CREATE PROCEDURE sp_GetAll_CourseModules
(
	@CourseModuleId INT = 0,
	@CourseId INT = 0
)
AS
	BEGIN
		IF (@CourseModuleId <> 0)
			BEGIN
				SELECT 
					cm.CourseModuleId,
					cm.CourseId,
					c.CourseName,
					cc.CourseCategoryId,
					cc.CourseCategoryName,
					cm.ModuleName,
					cm.ModuleDescription,
					cm.ModuleOrder,
					cm.Status,
					CASE 
						WHEN cm.Status = 1 THEN 'Active' 
						WHEN cm.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM CourseModules cm
				JOIN Courses c ON c.CourseId = cm.CourseId 
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND cm.CourseModuleId = @CourseModuleId
				ORDER BY cm.ModuleOrder ASC
			END
		ELSE IF (@CourseId <> 0)
			BEGIN
				SELECT 
					cm.CourseModuleId,
					cm.CourseId,
					c.CourseName,
					cc.CourseCategoryId,
					cc.CourseCategoryName,
					cm.ModuleName,
					cm.ModuleDescription,
					cm.ModuleOrder,
					cm.Status,
					CASE 
						WHEN cm.Status = 1 THEN 'Active' 
						WHEN cm.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM CourseModules cm
				JOIN Courses c ON c.CourseId = cm.CourseId 
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND c.CourseId = @CourseId
				ORDER BY cm.ModuleOrder ASC
			END
		ELSE
			BEGIN
				SELECT 
					cm.CourseModuleId,
					cm.CourseId,
					c.CourseName,
					cc.CourseCategoryId,
					cc.CourseCategoryName,
					cm.ModuleName,
					cm.ModuleDescription,
					cm.ModuleOrder,
					cm.Status,
					CASE 
						WHEN cm.Status = 1 THEN 'Active' 
						WHEN cm.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM CourseModules cm
				JOIN Courses c ON c.CourseId = cm.CourseId 
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
				ORDER BY cm.ModuleOrder ASC

			END
	END;



CREATE PROCEDURE sp_CreateUpdateDeleteRestore_CourseModuleContents
(
	@Type VARCHAR(10),
	@CourseModuleContentId INT = 0,
	@CourseModuleId INT = 0,
    @ContentName VARCHAR(250) = '',
    @ContentDescription NVARCHAR(MAX) = '',
	@DurationInHrs TINYINT = 1,
	@ContentOrder INT = 1,
	@Status BIT = 0
)
AS
BEGIN
	IF @Type = 'INSERT'
	BEGIN
		-- Check if CourseId exists and is not deleted
		IF NOT EXISTS (SELECT 1 FROM CourseModules WHERE CourseModuleId = @CourseModuleId AND DeletedAt IS NULL)
		BEGIN
			RAISERROR('Invalid CourseModuleId. The specified CourseModuleId does not exist.', 16, 1);
			RETURN;
		END
		
		-- Insert a new Course Module
		INSERT INTO CourseModuleContents(CourseModuleId, ContentName, ContentDescription, DurationInHrs, ContentOrder, Status) 
		VALUES (@CourseModuleId, @ContentName, @ContentDescription, @DurationInHrs, @ContentOrder, @Status);
	END

	ELSE IF @Type = 'UPDATE'
		BEGIN
			-- Check if CourseModuleId exists and is not deleted
			IF NOT EXISTS (SELECT 1 FROM CourseModuleContents WHERE CourseModuleContentId = @CourseModuleContentId AND DeletedAt IS NULL)
			BEGIN
					RAISERROR('Invalid CourseModuleContentId. The specified CourseModuleContentId does not exist.', 16, 1);
				RETURN;
			END
		
			-- Update an existing Course Module
			UPDATE CourseModuleContents 
			SET CourseModuleId = @CourseModuleId,
				ContentName = @ContentName,
				ContentDescription = @ContentDescription,
				DurationInHrs = @DurationInHrs,
				Status = @Status
			WHERE CourseModuleContentId = @CourseModuleContentId AND DeletedAt IS NULL;
		END

	ELSE IF @Type = 'DELETE'
	BEGIN
		-- Check if CourseModuleId exists and is not deleted
				IF NOT EXISTS (SELECT 1 FROM CourseModuleContents WHERE CourseModuleContentId = @CourseModuleContentId AND DeletedAt IS NULL)
			BEGIN
					RAISERROR('Invalid CourseModuleContentId. The specified CourseModuleContentId does not exist.', 16, 1);
				RETURN;
			END


		-- Soft delete the Course Module
		UPDATE CourseModuleContents 
		SET DeletedAt = GETDATE() 
		WHERE CourseModuleContentId = @CourseModuleContentId AND DeletedAt IS NULL;
	END

	ELSE IF @Type = 'RESTORE'
	BEGIN
		-- Restore a soft-deleted Course Module
		UPDATE CourseModuleContents 
		SET DeletedAt = NULL 
		WHERE CourseModuleContentId = @CourseModuleContentId;
	END

END;


CREATE PROCEDURE sp_GetAll_CourseModuleContents
(
	@CourseModuleContentId INT = 0,
	@CourseModuleId INT = 0
)
AS
	BEGIN
		IF (@CourseModuleContentId <> 0)
			BEGIN
				SELECT 
					cmc.CourseModuleContentId,
					cmc.CourseModuleId,
					cm.ModuleName,
					c.CourseId,
					c.CourseName,
					cmc.ContentName,
					cmc.ContentDescription,
					cmc.DurationInHrs,
					cmc.ContentOrder,
					cmc.Status,
					CASE 
						WHEN cmc.Status = 1 THEN 'Active' 
						WHEN cmc.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM CourseModuleContents cmc
				JOIN CourseModules cm ON cm.CourseModuleId = cmc.CourseModuleId
				JOIN Courses c ON c.CourseId = cm.CourseId
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cmc.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND cmc.CourseModuleContentId= @CourseModuleContentId
				ORDER BY cmc.ContentOrder ASC
			END
		ELSE IF (@CourseModuleId <> 0)
			BEGIN
				SELECT 
					cmc.CourseModuleContentId,
					cmc.CourseModuleId,
					cm.ModuleName,
					c.CourseId,
					c.CourseName,
					cmc.ContentName,
					cmc.ContentDescription,
					cmc.DurationInHrs,
					cmc.ContentOrder,
					cmc.Status,
					CASE 
						WHEN cmc.Status = 1 THEN 'Active' 
						WHEN cmc.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM CourseModuleContents cmc
				JOIN CourseModules cm ON cm.CourseModuleId = cmc.CourseModuleId
				JOIN Courses c ON c.CourseId = cm.CourseId
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cmc.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
					AND cmc.CourseModuleId= @CourseModuleId
				ORDER BY cmc.ContentOrder ASC
			END
		ELSE
			BEGIN
				SELECT 
					cmc.CourseModuleContentId,
					cmc.CourseModuleId,
					cm.ModuleName,
					c.CourseId,
					c.CourseName,
					cmc.ContentName,
					cmc.ContentDescription,
					cmc.DurationInHrs,
					cmc.ContentOrder,
					cmc.Status,
					CASE 
						WHEN cmc.Status = 1 THEN 'Active' 
						WHEN cmc.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column

				FROM CourseModuleContents cmc
				JOIN CourseModules cm ON cm.CourseModuleId = cmc.CourseModuleId
				JOIN Courses c ON c.CourseId = cm.CourseId
				JOIN CourseCategories cc ON cc.CourseCategoryId = c.CourseCategoryId
				WHERE 
					c.DeletedAt IS NULL 
					AND cmc.DeletedAt IS NULL 
					AND cm.DeletedAt IS NULL 
					AND cc.DeletedAt IS NULL 
				ORDER BY cmc.ContentOrder ASC
			END
	END;

