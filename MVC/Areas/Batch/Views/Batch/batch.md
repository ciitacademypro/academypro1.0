CREATE TABLE Batches
(
	BatchId INT IDENTITY PRIMARY KEY,
	BatchCode	VARCHAR(20) NOT NULL UNIQUE,
    BranchId    INT NULL DEFAULT NULL,
	CourseModuleId INT NOT NULL,
	TrainerId INT NULL DEFAULT NULL,
	ClassRoomId INT NULL DEFAULT NULL,
	StartDate DATE NULL DEFAULT NULL,
	ActualStartDate DATE NULL DEFAULT NULL,
	EndDate DATE NULL DEFAULT NULL,
	ActualEndDate DATE NULL DEFAULT NULL,
	StartTime TIME(0) NULL DEFAULT NULL,
	BatchDurationInHr TINYINT,
	WeekDays VARCHAR(15),
	Status BIT DEFAULT 0,
	DeletedAt DATETIME NULL DEFAULT NULL
);


CREATE TABLE BatchSchedules
(
   BatchScheduleId  INT IDENTITY PRIMARY KEY,
   BatchId INT NOT NULL,
   ExpectedDateTime DATETIME,
   ActualDateTime DATETIME,
   ExpectedTrainerId INT,
   ActualTrainerId INT DEFAULT NULL,
   Remark NVARCHAR(300) NULL DEFAULT NULL,
   Status VARCHAR(20),      -- (Pending, Attend, Postpond, Cancelled)
   DeletedAt DATETIME NULL DEFAULT NULL
);


CREATE TABLE [dbo].[tblbatch_schedule](
	[batch_schedule_id] [int] IDENTITY(1,1) NOT NULL,
	[batch_id] [int] NULL,

	[content_id] [int] NULL,
	[expected_date] [datetime] NULL,
	[actual_date] [datetime] NULL,ca
	[flag] [int] NULL,
