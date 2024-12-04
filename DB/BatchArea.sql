CREATE TABLE ClassRooms
(
    ClassRoomId INT IDENTITY PRIMARY KEY,
    FirmId INT NOT NULL,
    BranchId INT NOT NULL,
    ClassRoomName VARCHAR(30) NOT NULL,
    Status BIT NOT NULL DEFAULT 1,
    DeletedAt DATETIME DEFAULT NULL
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_ClassRooms
(
	@Type VARCHAR(10),
	@ClassRoomId INT = 0,
    @BranchId INT,
    @FirmId INT,
    @ClassRoomName VARCHAR(30),
    @Status BIT
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO ClassRooms(FirmId, BranchId, ClassRoomName, Status) values (@FirmId, @BranchId, @ClassRoomName, @Status)
			END

		    IF @Type = 'UPDATE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM ClassRooms WHERE ClassRoomId = @ClassRoomId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid ClassRoomId. The specified ClassRoomId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE ClassRooms SET 
					BranchId = @BranchId,ClassRoomName = @ClassRoomName,Status = @Status
					WHERE 
						ClassRoomId = @ClassRoomId 
						AND  FirmId = @FirmId
						AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM ClassRooms WHERE ClassRoomId = @ClassRoomId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid ClassRoomId. The specified ClassRoomId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE ClassRooms SET DeletedAt = GETDATE() 
				WHERE 
					ClassRoomId = @ClassRoomId 
					AND FirmId = @FirmId
					AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'
			BEGIN
				UPDATE ClassRooms SET DeletedAt = NULL 
				WHERE 
				ClassRoomId = @ClassRoomId
				AND FirmId = @FirmId
			END	
END;


CREATE PROCEDURE sp_GetAll_ClassRooms
(
	@ClassRoomId INT = 0,
    @FirmId INT
)
AS
	BEGIN
		IF (@ClassRoomId <> 0)
			BEGIN
				SELECT 
					ClassRoomId,
                    BranchId, ClassRoomName, Status,
                    
		CASE 
			WHEN Status = 1 THEN 'Active' 
			WHEN Status = 0 THEN 'Inactive' 
			ELSE 'Unknown'
		END AS StatusLabel  -- Custom StatusLabel column

				FROM ClassRooms
				WHERE 
					DeletedAt IS NULL 
					AND  FirmId = @FirmId
					AND ClassRoomId = @ClassRoomId
				ORDER BY ClassRoomId desc
			END
		ELSE
			BEGIN
				SELECT 
					ClassRoomId,
                    BranchId, ClassRoomName, Status,
                    
		CASE 
			WHEN Status = 1 THEN 'Active' 
			WHEN Status = 0 THEN 'Inactive' 
			ELSE 'Unknown'
		END AS StatusLabel  -- Custom StatusLabel column

				FROM ClassRooms
				WHERE 
					DeletedAt IS NULL 
					AND  FirmId = @FirmId
				ORDER BY ClassRoomId desc
			END
	END;



CREATE TABLE Batches
(
	BatchId INT IDENTITY PRIMARY KEY,
	FirmId INT NOT NULL,
	BatchCode	VARCHAR(30) NOT NULL UNIQUE,
    BranchId    INT NULL DEFAULT NULL,
	CourseModuleId INT NOT NULL,
	TrainerId NVARCHAR(MAX) NULL DEFAULT NULL,
	ClassRoomId INT NULL DEFAULT NULL,
	StartDate DATE NULL DEFAULT NULL,
	ActualStartDate DATE NULL DEFAULT NULL,
	EndDate DATE NULL DEFAULT NULL,
	ActualEndDate DATE NULL DEFAULT NULL,
	StartTime TIME(0) NULL DEFAULT NULL,
	BatchDurationInHr TINYINT,
	Status BIT DEFAULT 0,
	DeletedAt DATETIME NULL DEFAULT NULL
);

CREATE TABLE BatchWeekDays
(
	BatchWeekDayId INT IDENTITY PRIMARY KEY,
	BatchId INT NOT NULL,
	WeekDayCode TINYINT,
	WeekDayName VARCHAR(10),
	DeletedAt DATETIME NULL DEFAULT NULL
);

CREATE type BatchWeekDays_type AS TABLE
(
	WeekDayCode TINYINT NOT NULL,
	WeekDayName VARCHAR(10) NOT NULL
);


CREATE TABLE BatchSchedules
(
   BatchScheduleId  INT IDENTITY PRIMARY KEY,
   BranchId INT NOT NULL,
   BatchId INT NOT NULL,
   ExpectedDateTime DATETIME,
   ActualDateTime DATETIME,
   ExpectedTrainerId INT,
   ActualTrainerId INT DEFAULT NULL,
   Remark NVARCHAR(300) NULL DEFAULT NULL,
   Status VARCHAR(20) DEFAULT 'Pending',      -- (Pending, Attend, Postpond, Cancelled)
   DeletedAt DATETIME NULL DEFAULT NULL
);

CREATE TABLE BatchScheduleContents
(
   BatchScheduleContentId  INT IDENTITY PRIMARY KEY,
   BatchScheduleId INT NOT NULL,
   ContentId INT NOT NULL,
   DeletedAt DATETIME NULL DEFAULT NULL
);


CREATE TYPE BatchScheduleContents_type AS TABLE
(
    BatchScheduleId INT,
    ContentId INT
);



CREATE TYPE BatchSchedules_type AS TABLE
(
    ExpectedDateTime DATETIME,
    ExpectedTrainerId INT,
	ContentIds VARCHAR(MAX) -- This column will store the comma-separated content IDs.
);


CREATE PROCEDURE sp_Create_BatchSchedules
(
    @BranchId INT,
    @BatchId INT,
    @BatchSchedules BatchSchedules_type READONLY
)
AS
BEGIN
    -- Step 1: Declare a table variable to store inserted BatchSchedule records
    DECLARE @InsertedBatchSchedules TABLE
    (
        BatchScheduleId INT,
        ExpectedDateTime DATETIME,
        ExpectedTrainerId INT
    );

    -- Step 2: Insert into BatchSchedules and capture inserted BatchScheduleIds
    INSERT INTO BatchSchedules (BranchId, BatchId, ExpectedDateTime, ExpectedTrainerId, Status)
    OUTPUT INSERTED.BatchScheduleId, INSERTED.ExpectedDateTime, INSERTED.ExpectedTrainerId
    INTO @InsertedBatchSchedules (BatchScheduleId, ExpectedDateTime, ExpectedTrainerId)
    SELECT @BranchId, @BatchId, ExpectedDateTime, ExpectedTrainerId, 'Pending'
    FROM @BatchSchedules;

    -- Step 3: For each inserted row in BatchSchedules, insert into BatchScheduleContents
    DECLARE @BatchScheduleId INT, @ExpectedDateTime DATETIME, @ExpectedTrainerId INT, @ContentIds VARCHAR(MAX);
    
    -- Step 4: Loop through the inserted BatchSchedules to insert into BatchScheduleContents
    DECLARE schedule_cursor CURSOR FOR
    SELECT BatchScheduleId, ExpectedDateTime, ExpectedTrainerId
    FROM @InsertedBatchSchedules;

    OPEN schedule_cursor;
    FETCH NEXT FROM schedule_cursor INTO @BatchScheduleId, @ExpectedDateTime, @ExpectedTrainerId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Step 5: Get the ContentIds for the current BatchScheduleId
        SELECT @ContentIds = BS.ContentIds
        FROM @BatchSchedules BS
        WHERE BS.ExpectedDateTime = @ExpectedDateTime AND BS.ExpectedTrainerId = @ExpectedTrainerId;

        -- Step 6: Split ContentIds and insert into BatchScheduleContents
        INSERT INTO BatchScheduleContents (BatchScheduleId, ContentId)
        SELECT @BatchScheduleId, value
        FROM STRING_SPLIT(@ContentIds, ',');

        FETCH NEXT FROM schedule_cursor INTO @BatchScheduleId, @ExpectedDateTime, @ExpectedTrainerId;
    END;

    CLOSE schedule_cursor;
    DEALLOCATE schedule_cursor;
END;



CREATE PROCEDURE sp_Create_Batches
(
	@BatchId INT OUTPUT,
	@BatchCode VARCHAR(30),
	@FirmId INT,
	@BranchId INT,
	@CourseModuleId INT,
	@TrainerId NVARCHAR(MAX),
	@ClassRoomId INT,
	@StartDate DATE,
	@EndDate DATE,
	@StartTime TIME(0),
	@BatchDurationInHr TINYINT,	
	@WeekDaysList BatchWeekDays_type READONLY
)
AS
BEGIN TRY
	BEGIN TRANSACTION
		INSERT INTO Batches(BatchCode, FirmId, BranchId, CourseModuleId, TrainerId, ClassRoomId, StartDate, EndDate, StartTime, BatchDurationInHr)
		VALUES (
			@BatchCode,
			@FirmId,
			@BranchId,
			@CourseModuleId,
			@TrainerId,
			@ClassRoomId,
			@StartDate,
			@EndDate,
			@StartTime,
			@BatchDurationInHr
		)
		SET @BatchId = SCOPE_IDENTITY();

		INSERT INTO BatchWeekDays (BatchId,WeekDayCode, WeekDayName)
		SELECT @BatchId, WeekDayCode, WeekDayName FROM @WeekDaysList

	COMMIT TRANSACTION;

		-- Return the last inserted BatchId
		RETURN @BatchId;

END TRY
BEGIN CATCH
		RAISERROR('Invalid WeekDays. Please check given Week Days".', 16, 1)
END CATCH;


CREATE PROCEDURE sp_GetAll_Batches
(
	@BatchId INT = 0,
	@FirmId INT
)
AS
	BEGIN
		IF (@BatchId <> 0)
		BEGIN
			SELECT 
				b.BatchId,
				b.BatchCode,
				b.BranchId,
				b.CourseModuleId,
				c.ModuleName,
				b.TrainerId,
				u.Name as TrainerName,
				b.ClassRoomId,
				b.StartDate,
				b.ActualStartDate,
				b.EndDate,
				b.ActualEndDate,
				b.StartTime,
				b.BatchDurationInHr,
				b.Status,
				CASE 
					WHEN b.Status = 1 THEN 'Active' 
					WHEN b.Status = 0 THEN 'Inactive' 
					ELSE 'Unknown'
				END AS StatusLabel  -- Custom StatusLabel column

			FROM Batches b
			JOIN CourseModules c ON c.CourseModuleId = b.CourseModuleId
			JOIN AspNetUsers u ON u.Id= b.TrainerId
			WHERE 
			b.DeletedAt is NULL
			AND b.FirmId = @FirmId
			AND c.DeletedAt is NULL
			AND b.BatchId = @BatchId
		END
		ELSE
			BEGIN
			SELECT 
				b.BatchId,
				b.BatchCode,
				b.BranchId,
				b.CourseModuleId,
				c.ModuleName,
				b.TrainerId,
				u.Name as TrainerName,
				b.ClassRoomId,
				b.StartDate,
				b.ActualStartDate,
				b.EndDate,
				b.ActualEndDate,
				b.StartTime,
				b.BatchDurationInHr,
				b.Status,
				CASE 
					WHEN b.Status = 1 THEN 'Active' 
					WHEN b.Status = 0 THEN 'Inactive' 
					ELSE 'Unknown'
				END AS StatusLabel  -- Custom StatusLabel column

			FROM Batches b
			JOIN CourseModules c ON c.CourseModuleId = b.CourseModuleId
			JOIN AspNetUsers u ON u.Id= b.TrainerId

			WHERE 
			b.DeletedAt is NULL
			AND b.FirmId = @FirmId
			AND c.DeletedAt is NULL
		END
	END
