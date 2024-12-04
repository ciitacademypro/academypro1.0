CREATE TABLE Qualifications
(
	QualificationId SMALLINT IDENTITY PRIMARY KEY,
	QualificationName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);

	
CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Qualifications
(
	@Type VARCHAR(10),
	@QualificationId INT = 0,
    @QualificationName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Qualifications(QualificationName, Status) values ( @QualificationName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Qualifications WHERE QualificationId = @QualificationId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid QualificationId. The specified QualificationId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Qualifications SET 
					QualificationName = @QualificationName,
					Status = @Status
					WHERE QualificationId = @QualificationId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Qualifications WHERE QualificationId = @QualificationId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid QualificationId. The specified QualificationId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Qualifications SET DeletedAt = GETDATE() WHERE QualificationId = @QualificationId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Qualifications SET DeletedAt = NULL WHERE QualificationId = @QualificationId
			END	
END;


CREATE PROCEDURE sp_GetAll_Qualifications
(
	@QualificationId INT = 0
)
AS
	BEGIN
		IF (@QualificationId <> 0)
			BEGIN
				SELECT 
					QualificationId,
					QualificationName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Qualifications
				WHERE 
					DeletedAt IS NULL 
					AND QualificationId = @QualificationId
				ORDER BY QualificationId desc
			END
		ELSE
			BEGIN
				SELECT 
					QualificationId,
					QualificationName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Qualifications
				WHERE 
					DeletedAt IS NULL 
				ORDER BY QualificationId desc
			END
	END;


CREATE TABLE LeadSources
(
	LeadSourceId SMALLINT IDENTITY PRIMARY KEY,
	LeadSourceName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);



CREATE PROCEDURE sp_CreateUpdateDeleteRestore_LeadSources
(
	@Type VARCHAR(10),
	@LeadSourceId INT = 0,
    @LeadSourceName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO LeadSources(LeadSourceName, Status) values ( @LeadSourceName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM LeadSources WHERE LeadSourceId = @LeadSourceId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid LeadSourceId. The specified LeadSourceId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE LeadSources SET 
					LeadSourceName = @LeadSourceName,
					Status = @Status
					WHERE LeadSourceId = @LeadSourceId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM LeadSources WHERE LeadSourceId = @LeadSourceId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid LeadSourceId. The specified LeadSourceId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE LeadSources SET DeletedAt = GETDATE() WHERE LeadSourceId = @LeadSourceId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE LeadSources SET DeletedAt = NULL WHERE LeadSourceId = @LeadSourceId
			END	
END;


CREATE PROCEDURE sp_GetAll_LeadSources
(
	@LeadSourceId INT = 0
)
AS
	BEGIN
		IF (@LeadSourceId <> 0)
			BEGIN
				SELECT 
					LeadSourceId,
					LeadSourceName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM LeadSources
				WHERE 
					DeletedAt IS NULL 
					AND LeadSourceId = @LeadSourceId
				ORDER BY LeadSourceId desc
			END
		ELSE
			BEGIN
				SELECT 
					LeadSourceId,
					LeadSourceName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM LeadSources
				WHERE 
					DeletedAt IS NULL 
				ORDER BY LeadSourceId desc
			END
	END;


CREATE TABLE EnquiryFors
(
	EnquiryForId SMALLINT IDENTITY PRIMARY KEY,
	EnquiryForName	VARCHAR(50) NOT NULL UNIQUE,
	Status 	BIT DEFAULT 0,
	DeletedAt	DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_EnquiryFors
(
	@Type VARCHAR(10),
	@EnquiryForId INT = 0,
    @EnquiryForName VARCHAR(50),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO EnquiryFors(EnquiryForName, Status) values ( @EnquiryForName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY EnquireForId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM EnquiryFors  WHERE EnquiryForId = @EnquiryForId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid LeadSource. The specified EnquiryForId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE EnquiryFors  SET 
					EnquiryForName = @EnquiryForName,
					Status = @Status
					WHERE EnquiryForId = @EnquiryForId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY EnquiryForId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM EnquiryFors  WHERE EnquiryForId = @EnquiryForId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid EnquiryFor. The specified EnquiryForId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE EnquiryFors  SET DeletedAt = GETDATE() WHERE EnquiryForId = @EnquiryForId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY EnquiryForId
			BEGIN
				UPDATE EnquiryFors  SET DeletedAt = NULL WHERE EnquiryForId = @EnquiryForId
			END	
END;



CREATE PROCEDURE sp_GetAll_EnquiryFors
(
	@EnquiryForId INT = 0
)
AS
	BEGIN
		IF (@EnquiryForId <> 0)
			BEGIN
				SELECT 
					EnquiryForId,
					EnquiryForName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM EnquiryFors
				WHERE 
					DeletedAt IS NULL 
					AND EnquiryForId = @EnquiryForId
				ORDER BY EnquiryForId desc
			END
		ELSE		
			BEGIN
				SELECT 
					EnquiryForId,
					EnquiryForName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM EnquiryFors
				WHERE 
					DeletedAt IS NULL 					
				ORDER BY EnquiryForId desc
			END
	END;



CREATE TABLE Holidays
(
    HolidayId INT IDENTITY PRIMARY KEY,            
    HolidayName VARCHAR(50) NOT NULL ,
    HolidayDate Date NOT NULL ,
    Description NVARCHAR(MAX) NOT NULL,
    DeletedAt DATETIME DEFAULT NULL
);


TRUNCATE TABLE Holidays

DECLARE @StartDate DATE = GETDATE();   -- Start from today
DECLARE @EndDate DATE = DATEADD(YEAR, 1, @StartDate); -- End date is 1 year from today
DECLARE @CurrentDate DATE = @StartDate;

WHILE @CurrentDate <= @EndDate
BEGIN
    -- Check if the day is Saturday (6) or Sunday (7)
    IF DATEPART(WEEKDAY, @CurrentDate) = 7 OR DATEPART(WEEKDAY, @CurrentDate) = 1
    BEGIN
        -- Check if the record already exists
        IF NOT EXISTS (
            SELECT 1 
            FROM Holidays 
            WHERE HolidayDate = @CurrentDate
        )
        BEGIN
            INSERT INTO Holidays (HolidayName, HolidayDate, Description)
            VALUES (
                CASE 
                    WHEN DATEPART(WEEKDAY, @CurrentDate) = 7 THEN 'Saturday Off'
                    WHEN DATEPART(WEEKDAY, @CurrentDate) = 1 THEN 'Sunday Off'
                END,
                @CurrentDate,
                'Weekend Holiday'
            );
        END;
    END;

    -- Move to the next day
    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END;




CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Holidays
(
	@Type VARCHAR(10),
	@HolidayId INT = 0,
    @HolidayName VARCHAR(50),
    @HolidayDate DateTime,
    @Description NVARCHAR(MAX)
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Holidays(HolidayName, HolidayDate, Description) values (@HolidayName, @HolidayDate, @Description)
			END

		    IF @Type = 'UPDATE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Holidays WHERE HolidayId = @HolidayId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid HolidayId. The specified HolidayId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Holidays SET 
					HolidayName = @HolidayName,HolidayDate = @HolidayDate,Description = @Description
					WHERE HolidayId = @HolidayId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Holidays WHERE HolidayId = @HolidayId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid HolidayId. The specified HolidayId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Holidays SET DeletedAt = GETDATE() WHERE HolidayId = @HolidayId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'
			BEGIN
				UPDATE Holidays SET DeletedAt = NULL WHERE HolidayId = @HolidayId
			END	
END;



CREATE PROCEDURE sp_GetAll_Holidays
(
	@HolidayId INT = 0
)
AS
	BEGIN
		IF (@HolidayId <> 0)
			BEGIN
				SELECT 
					HolidayId,
                    HolidayName,
					HolidayDate,
					Description
                    
				FROM Holidays
				WHERE 
					DeletedAt IS NULL 
					AND HolidayId = @HolidayId
				ORDER BY HolidayDate ASC
			END
		ELSE
			BEGIN
				SELECT 
					HolidayId,
                    HolidayName, HolidayDate, Description
                    
				FROM Holidays
				WHERE 
					DeletedAt IS NULL 
				ORDER BY HolidayDate ASC
			END
	END;




CREATE TABLE Countries
(
	CountryId SMALLINT IDENTITY PRIMARY KEY,
	CountryName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Countries
(
	@Type VARCHAR(10),
	@CountryId INT = 0,
    @CountryName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Countries(CountryName, Status) values ( @CountryName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Countries WHERE CountryId = @CountryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CountryId. The specified CountryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Countries SET 
					CountryName = @CountryName,
					Status = @Status
					WHERE CountryId = @CountryId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Countries WHERE CountryId = @CountryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CountryId. The specified CountryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Countries SET DeletedAt = GETDATE() WHERE CountryId = @CountryId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Countries SET DeletedAt = NULL WHERE CountryId = @CountryId
			END	
END;



CREATE PROCEDURE sp_GetAll_Countries
(
	@CountryId INT = 0
)
AS
	BEGIN
		IF (@CountryId <> 0)
			BEGIN
				SELECT 
					CountryId,
					CountryName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Countries
				WHERE 
					DeletedAt IS NULL 
					AND CountryId = @CountryId
				ORDER BY CountryId desc
			END
		ELSE
			BEGIN
				SELECT 
					CountryId,
					CountryName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Countries
				WHERE 
					DeletedAt IS NULL 
				ORDER BY CountryId desc
			END
	END;



CREATE TABLE States
(

	StateId INT IDENTITY PRIMARY KEY,
	CountryId SMALLINT,
	StateName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);

	
CREATE PROCEDURE sp_CreateUpdateDeleteRestore_States
(
	@Type VARCHAR(10),
	@StateId INT = 0,
	@CountryId SMALLINT = 0,
    @StateName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO States(CountryId,StateName, Status) values (@CountryId, @StateName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM States WHERE StateId = @StateId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid StateId. The specified StateId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE States SET 
					CountryId = @CountryId,
					StateName = @StateName,
					Status = @Status
					WHERE StateId = @StateId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM States WHERE StateId = @StateId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid StateId. The specified StateId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE States SET DeletedAt = GETDATE() WHERE StateId = @StateId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE States SET DeletedAt = NULL WHERE StateId = @StateId
			END	
END;



CREATE PROCEDURE sp_GetAll_States
(
	@StateId INT = 0,
	@CountryId INT = 0
)
AS
	BEGIN
		IF (@StateId <> 0)
			BEGIN
				SELECT
					c.CountryId,
					c.CountryName,
					s.StateId,
					s.StateName,
					s.Status,
					CASE 
						WHEN s.Status = 1 THEN 'Active' 
						WHEN s.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM States s
				JOIN Countries c ON c.CountryId = s.CountryId
				WHERE 
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND s.StateId = @StateId
				ORDER BY StateId desc
			END
		ELSE IF (@CountryId <> 0)
			BEGIN
				SELECT
					c.CountryId,
					c.CountryName,
					s.StateId,
					s.StateName,
					s.Status,
					CASE 
						WHEN s.Status = 1 THEN 'Active' 
						WHEN s.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM States s
				JOIN Countries c ON c.CountryId = s.CountryId
				WHERE 
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND c.CountryId = @CountryId
				ORDER BY StateId desc
			END
		ELSE
			BEGIN
				SELECT
					c.CountryId,
					c.CountryName,
					s.StateId,
					s.StateName,
					s.Status,
					CASE 
						WHEN s.Status = 1 THEN 'Active' 
						WHEN s.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM States s
				JOIN Countries c ON c.CountryId = s.CountryId
				WHERE 
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
				ORDER BY StateId desc
			END

	END;





CREATE TABLE Cities
(
	CityId INT IDENTITY PRIMARY KEY,
	StateId INT,
	CityName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Cities
(
	@Type VARCHAR(10),
	@CityId INT = 0,
	@StateId INT = 0,
    @CityName VARCHAR(100),
	@Status BIT = 0
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Cities(StateId,CityName, Status) values (@StateId, @CityName, @Status)
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Cities WHERE CityId = @CityId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CityId. The specified CityId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Cities SET 
					StateId = @StateId,
					CityName = @CityName,
					Status = @Status
					WHERE CityId = @CityId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Cities WHERE CityId = @CityId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid CityId. The specified CityId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Cities SET DeletedAt = GETDATE() WHERE CityId = @CityId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Cities SET DeletedAt = NULL WHERE CityId = @CityId
			END	
END;


CREATE PROCEDURE sp_GetAll_Cities
(
	@CityId INT = 0,
	@StateId INT = 0
)
AS
	BEGIN
		IF (@CityId <> 0)	
			BEGIN
				SELECT
					c.CityId,
					s.StateId,
					s.StateName,
					c.CityName,
					ct.CountryId,
					ct.CountryName,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Cities c
				JOIN States s ON c.StateId = s.StateId
				JOIN Countries ct ON ct.CountryId = s.CountryId
				WHERE
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND ct.DeletedAt IS NULL 
					AND c.CityId = @CityId
				ORDER BY c.CityId desc
			END
		ELSE IF (@StateId <> 0)
			BEGIN
				SELECT
					c.CityId,
					s.StateId,
					s.StateName,
					c.CityName,
					ct.CountryId,
					ct.CountryName,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Cities c
				JOIN States s ON c.StateId = s.StateId
				JOIN Countries ct ON ct.CountryId = s.CountryId
				WHERE
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND ct.DeletedAt IS NULL 
					AND s.StateId = @StateId
				ORDER BY c.CityId desc
			END
		ELSE
			BEGIN
				SELECT
					c.CityId,
					s.StateId,
					s.StateName,
					c.CityName,
					ct.CountryId,
					ct.CountryName,
					c.Status,
					CASE 
						WHEN c.Status = 1 THEN 'Active' 
						WHEN c.Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Cities c
				JOIN States s ON c.StateId = s.StateId
				JOIN Countries ct ON ct.CountryId = s.CountryId
				WHERE
					s.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND ct.DeletedAt IS NULL 
				ORDER BY c.CityId desc

			END
	END;


