
CREATE TABLE Branches
(
	BranchId INT IDENTITY PRIMARY KEY,
	FirmId INT,
	BranchName VARCHAR(50),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Branches
(
	@Type VARCHAR(10),
	@BranchId INT = 0,
	@FirmId INT = 0,
    @BranchName VARCHAR(100),
	@Status BIT = 0,
	@LastInsertedId INT OUTPUT
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Branches(FirmId, BranchName, Status) values ( @FirmId, @BranchName, @Status)
				SET @LastInsertedId = SCOPE_IDENTITY();
			END

		    IF @Type = 'UPDATE'			-- UPDATE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Branches WHERE BranchId = @BranchId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid BranchId. The specified BranchId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Branches SET 
					BranchName = @BranchName,
					Status = @Status
					WHERE BranchId = @BranchId AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Branches WHERE BranchId = @BranchId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid BranchId. The specified BranchId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Branches SET DeletedAt = GETDATE() WHERE BranchId = @BranchId AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Branches SET DeletedAt = NULL WHERE BranchId = @BranchId
			END	
END;


ALTER PROCEDURE sp_GetAll_Branches
(
	@BranchId INT = 0,
	@FirmId INT = 0
)
AS
	BEGIN
		IF (@BranchId <> 0)
			BEGIN
				SELECT 
					BranchId,
					FirmId,
					BranchName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Branches
				WHERE 
					DeletedAt IS NULL 
					AND FirmId = @FirmId
					AND BranchId = @BranchId
				ORDER BY BranchName asc
			END
		ELSE
			BEGIN
				SELECT 
					BranchId,
					BranchName,
					Status,
					CASE 
						WHEN Status = 1 THEN 'Active' 
						WHEN Status = 0 THEN 'Inactive' 
						ELSE 'Unknown'
					END AS StatusLabel  -- Custom StatusLabel column
				FROM Branches
				WHERE 
					DeletedAt IS NULL 
					AND FirmId = @FirmId
				ORDER BY BranchId desc
			END
	END;
