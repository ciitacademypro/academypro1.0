select * from LeadSources;


CREATE TABLE Leads
(
	LeadId	INT IDENTITY PRIMARY KEY,
	FirmId INT NOT NULL,
	BranchId INT DEFAULT NULL,
	FirstName VARCHAR(25) NOT NULL,
	LastName VARCHAR(25) NOT NULL,
	EmailAddress VARCHAR(50),
	MobileNumber VARCHAR(15) NOT NULL,
	LeadSourceId INT NOT NULL,
	CourseId INT DEFAULT NULL,
	QualificationId INT DEFAULT NULL,
	PassoutYear	VARCHAR(4) DEFAULT NULL,
	Status	VARCHAR(20)	NOT NULL DEFAULT 'New',
	Remark NVARCHAR(2000)	 DEFAULT NULL,
	AssignedTo NVARCHAR(MAX) DEFAULT NULL,
	AssignedAt DATETIME DEFAULT NULL,
	AssignedBy NVARCHAR(MAX) DEFAULT NULL,

	CreatedBy NVARCHAR(MAX) DEFAULT NULL,
	CreatedAt DATETIME DEFAULT NULL,
	UpdatedBy NVARCHAR(MAX) DEFAULT NULL,
	UpdatedAt DATETIME DEFAULT NULL,

	DeletedAt DATETIME DEFAULT NULL,

	CONSTRAINT ENUM_Leads_Status CHECK (Status in ('New', 'Assigned', 'Contacted','Interested','Follow-Up','Enrolled','Not Interested','Pending','Closed'))
);


CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Leads
(
	@Type VARCHAR(10),
	@LeadId INT = 0,
	
	@FirmId INT,
	@BranchId INT,

    @FirstName VARCHAR(25),
    @LastName VARCHAR(25),
	@EmailAddress VARCHAR(50),
	@MobileNumber VARCHAR(15),
	@LeadSourceId INT,
	@CourseId INT = NULL,
	@QualificationId INT = NULL,
	@PassoutYear VARCHAR(4) = NULL,
	@Status	VARCHAR(20),
	@CreatedUpdatedBy NVARCHAR(MAX) = NULL,
	@AssignedTo NVARCHAR(MAX) = NULL
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Leads(FirmId, BranchId,  FirstName, LastName, EmailAddress, MobileNumber, LeadSourceId, CourseId, QualificationId, PassoutYear, CreatedBy, CreatedAt)
				VALUES ( @FirmId, @BranchId, @FirstName, @LastName, @EmailAddress, @MobileNumber, @LeadSourceId, @CourseId, @QualificationId, @PassoutYear, @CreatedUpdatedBy, GETDATE())
			END

		    IF @Type = 'UPDATE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Leads WHERE LeadId = @LeadId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid LeadId. The specified LeadId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Leads SET 
					BranchId = @BranchId,
					FirstName = @FirstName,
					LastName = @LastName,
					EmailAddress = @EmailAddress,
					MobileNumber = @MobileNumber,
					LeadSourceId = @LeadSourceId,
					CourseId = @CourseId,
					QualificationId = @QualificationId,
					PassoutYear = @PassoutYear,
					Status = @Status,
					UpdatedBy = @CreatedUpdatedBy,
					UpdatedAt = GETDATE()
					WHERE 
					LeadId = @LeadId 
					AND FirmId = @FirmId
					AND  DeletedAt IS NULL
				END
			END

		    IF @Type = 'DELETE'			-- DELETE BY CourseFeesId
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Leads WHERE LeadId = @LeadId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid LeadId. The specified LeadId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Leads SET DeletedAt = GETDATE() WHERE 
				LeadId = @LeadId 
				AND FirmId = @FirmId
				AND DeletedAt IS NULL
			END

		    IF @Type = 'RESTORE'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Leads SET DeletedAt = NULL WHERE LeadId = @LeadId
			END	

		    IF @Type = 'ASSIGNTO'			-- RESTORE BY CourseFeesId
			BEGIN
				UPDATE Leads SET 
					AssignedTo = @AssignedTo,
					AssignedAt = GETDATE(),
					AssignedBy = @CreatedUpdatedBy,
					UpdatedBy = @CreatedUpdatedBy,	
					UpdatedAt = GETDATE(),
					Status = 'Assigned'
				WHERE LeadId = @LeadId
				AND FirmId = @FirmId
			END
END;

CREATE PROCEDURE sp_GetAll_Leads
(
	@FirmId INT,
	@LeadId INT = 0,
	@BranchId INT = 0,
	@SearchByColumn VARCHAR(30) = NULL,
	@SearchByValue INT = NULL
)
AS
BEGIN
	-- If a specific LeadId is provided, return that record
	IF (@LeadId <> 0)
	BEGIN
		SELECT 
			l.LeadId,
			l.FirstName,
			l.LastName,
			l.EmailAddress,
			l.MobileNumber,
			l.LeadSourceId,
			ls.LeadSourceName,
			l.CourseId,
			c.CourseName,
			l.QualificationId,
			q.QualificationName,
			l.PassoutYear,
			l.Status,
			l.Remark,
			l.AssignedTo,
			e1.Name AssignedToName,
			l.AssignedBy,
			e2.Name AssignedByName,
			l.AssignedAt,
			l.CreatedBy,
			e3.Name CreatedByName,
			l.CreatedAt,
			l.UpdatedBy,
			e4.Name UpdatedByName,
			l.UpdatedAt
		FROM Leads l
		LEFT JOIN LeadSources ls ON ls.LeadSourceId = l.LeadSourceId
		LEFT JOIN Courses c ON c.CourseId = l.CourseId
		LEFT JOIN Qualifications q ON q.QualificationId = l.QualificationId
		LEFT JOIN AspNetUsers e1 ON e1.Id = l.AssignedTo
		LEFT JOIN AspNetUsers e2 ON e2.Id = l.AssignedBy
		LEFT JOIN AspNetUsers e3 ON e3.Id = l.CreatedBy
		LEFT JOIN AspNetUsers e4 ON e4.Id = l.UpdatedBy
		WHERE l.LeadId = @LeadId
			AND l.FirmId = @FirmId
			AND l.DeletedAt IS NULL
		ORDER BY l.LeadId DESC;
	END
	-- If searching by a column
	ELSE IF (@SearchByColumn IS NOT NULL)
	BEGIN
		DECLARE @SQL NVARCHAR(MAX);

		-- Build the base dynamic SQL
		SET @SQL = '
			SELECT 
				l.LeadId,
				l.FirstName,
				l.LastName,
				l.EmailAddress,
				l.MobileNumber,
				l.LeadSourceId,
				ls.LeadSourceName,
				l.CourseId,
				c.CourseName,
				l.QualificationId,
				q.QualificationName,
				l.PassoutYear,
				l.Status,
				l.Remark,
				l.AssignedTo,
				e1.Name AssignedToName,
				l.AssignedBy,
				e2.Name AssignedByName,
				l.AssignedAt,
				l.CreatedBy,
				e3.Name CreatedByName,
				l.CreatedAt,
				l.UpdatedBy,
				e4.Name UpdatedByName,
				l.UpdatedAt
			FROM Leads l
			LEFT JOIN LeadSources ls ON ls.LeadSourceId = l.LeadSourceId
			LEFT JOIN Courses c ON c.CourseId = l.CourseId
			LEFT JOIN Qualifications q ON q.QualificationId = l.QualificationId
			LEFT JOIN AspNetUsers e1 ON e1.Id = l.AssignedTo
			LEFT JOIN AspNetUsers e2 ON e2.Id = l.AssignedBy
			LEFT JOIN AspNetUsers e3 ON e3.Id = l.CreatedBy
			LEFT JOIN AspNetUsers e4 ON e4.Id = l.UpdatedBy
			WHERE l.DeletedAt IS NULL AND l.FirmId = @FirmId ';

		-- Handle AssignedTo column filtering
		IF(@SearchByColumn = 'AssignedTo')
		BEGIN
			IF (@SearchByValue IS NULL)
			BEGIN
				SET @SQL = @SQL + ' AND l.AssignedTo IS NULL ';
			END
			ELSE IF (@SearchByValue = 0)
			BEGIN
				SET @SQL = @SQL + ' AND l.AssignedTo IS NOT NULL ';
			END
			ELSE
			BEGIN
				SET @SQL = @SQL + ' AND l.AssignedTo = ' + CAST(@SearchByValue AS NVARCHAR(10));
			END
		END
		-- Handle other columns (ensure numeric comparison or string as needed)
		ELSE
		BEGIN
			SET @SQL = @SQL + ' AND l.' + @SearchByColumn + ' = ' + CAST(@SearchByValue AS NVARCHAR(10));
		END

		SET @SQL = @SQL + ' ORDER BY l.LeadId DESC';
		
		-- Execute the dynamic SQL query with parameter binding
		EXEC sp_executesql 
			@SQL, 
			N'@FirmId INT', 
			@FirmId = @FirmId;
	END
	-- Default behavior when no specific search criteria is provided
	ELSE
	BEGIN
		SELECT 
			l.LeadId,
			l.FirstName,
			l.LastName,
			l.EmailAddress,
			l.MobileNumber,
			l.LeadSourceId,
			ls.LeadSourceName,
			l.CourseId,
			c.CourseName,
			l.QualificationId,
			q.QualificationName,
			l.PassoutYear,
			l.Status,
			l.Remark,
			l.AssignedTo,
			e1.Name AssignedToName,
			l.AssignedBy,
			e2.Name AssignedByName,
			l.AssignedAt,
			l.CreatedBy,
			e3.Name CreatedByName,
			l.CreatedAt,
			l.UpdatedBy,
			e4.Name UpdatedByName,
			l.UpdatedAt
		FROM Leads l
		LEFT JOIN LeadSources ls ON ls.LeadSourceId = l.LeadSourceId
		LEFT JOIN Courses c ON c.CourseId = l.CourseId
		LEFT JOIN Qualifications q ON q.QualificationId = l.QualificationId
		LEFT JOIN AspNetUsers e1 ON e1.Id = l.AssignedTo
		LEFT JOIN AspNetUsers e2 ON e2.Id = l.AssignedBy
		LEFT JOIN AspNetUsers e3 ON e3.Id = l.CreatedBy
		LEFT JOIN AspNetUsers e4 ON e4.Id = l.UpdatedBy
		WHERE l.DeletedAt IS NULL AND l.FirmId = @FirmId
		ORDER BY l.LeadId DESC;
	END
END;




CREATE TABLE Enquiries
(
    EnquiryId INT IDENTITY PRIMARY KEY,   
	FirmId INT NOT NULL,
    BranchId INT NOT NULL ,
    EnquiryDate DateTime NOT NULL ,
    CandidateName VARCHAR(50) NOT NULL ,
    EmailAddress VARCHAR(50) NOT NULL UNIQUE,
    MobileNumber VARCHAR(15) NOT NULL UNIQUE,
    CityId INT NOT NULL ,
    LocalAddress VARCHAR(150) NOT NULL ,
    Gender VARCHAR(10) NOT NULL ,
    QualificationId INT NOT NULL ,
    DateOfBirth DateTime NOT NULL ,
    LeadSourceId INT NOT NULL ,
    EnquiryForId INT NOT NULL ,
	Status	VARCHAR(20)	NOT NULL DEFAULT 'New',
    Remark VARCHAR(100) DEFAULT NULL ,
    DeletedAt DATETIME DEFAULT NULL,

	CONSTRAINT ENUM_Enquiries_Status CHECK (Status in ('New', 'From-Lead', 'Registered','Enrolled'))
);



CREATE PROCEDURE sp_CreateUpdateDeleteRestore_Enquiries
(
	@Type VARCHAR(10),
	@EnquiryId INT = 0,
	@FirmId INT,
    @BranchId INT,
    @EnquiryDate DateTime,
    @CandidateName VARCHAR(50),
    @EmailAddress VARCHAR(50),
    @MobileNumber VARCHAR(15),
    @CityId INT,
    @LocalAddress VARCHAR(150),
    @Gender VARCHAR(10),
    @QualificationId INT,
    @DateOfBirth DateTime,
    @LeadSourceId INT,
    @EnquiryForId INT,
	@Status VARCHAR(20),
    @Remark VARCHAR(100),
	@LastInsertedId INT OUTPUT
)
AS
BEGIN
		    IF @Type = 'INSERT'
			BEGIN
				INSERT INTO Enquiries(FirmId, BranchId, EnquiryDate, CandidateName, EmailAddress, MobileNumber, CityId, LocalAddress, Gender, QualificationId, DateOfBirth, LeadSourceId, EnquiryForId, Remark, Status) values (@FirmId, @BranchId, @EnquiryDate, @CandidateName, @EmailAddress, @MobileNumber, @CityId, @LocalAddress, @Gender, @QualificationId, @DateOfBirth, @LeadSourceId, @EnquiryForId, @Remark, @Status);
				
				SET @LastInsertedId = SCOPE_IDENTITY();

			END

		    IF @Type = 'UPDATE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Enquiries WHERE EnquiryId = @EnquiryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid EnquiryId. The specified EnquiryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				BEGIN
					UPDATE Enquiries SET 
					EnquiryDate = @EnquiryDate,CandidateName = @CandidateName,EmailAddress = @EmailAddress,MobileNumber = @MobileNumber,CityId = @CityId,LocalAddress = @LocalAddress,Gender = @Gender,QualificationId = @QualificationId,DateOfBirth = @DateOfBirth,LeadSourceId = @LeadSourceId,EnquiryForId = @EnquiryForId,BranchId = @BranchId,Remark = @Remark, Status = @Status
					WHERE 
						EnquiryId = @EnquiryId 
						AND  DeletedAt IS NULL
						AND FirmId = @FirmId

				END
			END

		    IF @Type = 'DELETE'
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM Enquiries WHERE EnquiryId = @EnquiryId AND DeletedAt IS NULL)
					BEGIN
						RAISERROR('Invalid EnquiryId. The specified EnquiryId does not exist.', 16, 1)
						RETURN
					END
				ELSE
				UPDATE Enquiries SET DeletedAt = GETDATE() WHERE 
				EnquiryId = @EnquiryId 
				AND DeletedAt IS NULL
				AND FirmId = @FirmId

			END

		    IF @Type = 'RESTORE'
			BEGIN
				UPDATE Enquiries SET DeletedAt = NULL 
				WHERE 
					EnquiryId = @EnquiryId
					AND FirmId = @FirmId
			END	
END;


CREATE PROCEDURE sp_GetAll_Enquiries
(
	@FirmId INT,
	@BranchId INT = 0,
	@EnquiryId INT = 0
)
AS
	BEGIN
		IF (@EnquiryId <> 0)
			BEGIN
				SELECT 
					e.EnquiryId,
                    e.EnquiryDate, 
					e.CandidateName, 
					e.EmailAddress, 
					e.MobileNumber, 
					e.CityId, 
					c.CityName,
					e.LocalAddress, 
					e.Gender,
					e.QualificationId,
					q.QualificationName, 
					e.DateOfBirth, 
					e.LeadSourceId,
					l.LeadSourceName, 
					e.EnquiryForId,
					ef.EnquiryForName, 					
					e.BranchId,
					b.BranchName, 
					e.Remark,
					e.Status
                    
				FROM Enquiries e 
				JOIN Cities c ON e.CityId=c.CityId 
				JOIN Qualifications q ON e.QualificationId=q.QualificationId 
				JOIN LeadSources l ON e.LeadSourceId=l.LeadSourceId 
				JOIN Branches b	ON e.BranchId=b.BranchId
				JOIN EnquiryFors ef ON e.EnquiryForId=ef.EnquiryForId
				WHERE 
					e.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND q.DeletedAt IS NULL
					AND l.DeletedAt IS NULL
					AND b.DeletedAt IS NULL
					AND ef.DeletedAt IS NULL
					AND (e.Status != 'Registered' AND e.Status != 'Enrolled')
					AND EnquiryId = @EnquiryId
					AND e.FirmId = @FirmId
				ORDER BY EnquiryId desc
			END
		ELSE IF(@BranchId <> 0)
			BEGIN
				SELECT 
					e.EnquiryId,
                    e.EnquiryDate, 
					e.CandidateName, 
					e.EmailAddress, 
					e.MobileNumber, 
					e.CityId, 
					c.CityName,
					e.LocalAddress, 
					e.Gender,
					e.QualificationId,
					q.QualificationName, 
					e.DateOfBirth, 
					e.LeadSourceId,
					l.LeadSourceName, 
					e.EnquiryForId, 
					ef.EnquiryForName,
					e.BranchId,
					b.BranchName, 
					e.Remark,
					e.Status
                    
				FROM Enquiries e 
				JOIN Cities c ON e.CityId=c.CityId 
				JOIN Qualifications q ON e.QualificationId=q.QualificationId 
				JOIN LeadSources l ON e.LeadSourceId=l.LeadSourceId 
				JOIN Branches b	ON e.BranchId=b.BranchId   
				JOIN EnquiryFors ef ON e.EnquiryForId=ef.EnquiryForId				
				
				WHERE 
					e.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND q.DeletedAt IS NULL
					AND l.DeletedAt IS NULL
					AND b.DeletedAt IS NULL
					AND ef.DeletedAt IS NULL
					AND (e.Status != 'Registered' AND e.Status != 'Enrolled')
					AND e.FirmId = @FirmId
					AND e.BranchId = @BranchId

				ORDER BY EnquiryId desc
			END
		ELSE
			BEGIN
				SELECT 
					e.EnquiryId,
                    e.EnquiryDate, 
					e.CandidateName, 
					e.EmailAddress, 
					e.MobileNumber, 
					e.CityId, 
					c.CityName,
					e.LocalAddress, 
					e.Gender,
					e.QualificationId,
					q.QualificationName, 
					e.DateOfBirth, 
					e.LeadSourceId,
					l.LeadSourceName, 
					e.EnquiryForId, 
					ef.EnquiryForName,
					e.BranchId,
					b.BranchName, 
					e.Remark,
					e.Status
                    
				FROM Enquiries e 
				JOIN Cities c ON e.CityId=c.CityId 
				JOIN Qualifications q ON e.QualificationId=q.QualificationId 
				JOIN LeadSources l ON e.LeadSourceId=l.LeadSourceId 
				JOIN Branches b	ON e.BranchId=b.BranchId   
				JOIN EnquiryFors ef ON e.EnquiryForId=ef.EnquiryForId				
				
				WHERE 
					e.DeletedAt IS NULL 
					AND c.DeletedAt IS NULL 
					AND q.DeletedAt IS NULL
					AND l.DeletedAt IS NULL
					AND b.DeletedAt IS NULL
					AND ef.DeletedAt IS NULL
					AND (e.Status != 'Registered' AND e.Status != 'Enrolled')
					AND e.FirmId = @FirmId

				ORDER BY EnquiryId desc
			END
	END;








CREATE TABLE Enrollments(
	StudentEnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
	FirmId INT NOT NULL,
    BranchId INT NOT NULL,
	StudentId NVARCHAR NULL,
	EnrollmentDate DATE NULL,
	CourseId INT NULL,
	EnrollmentType VARCHAR(50) NULL DEFAULT NULL,
	PaymentStatus VARCHAR(50) NULL DEFAULT NULL,
	CourseFeeId INT NULL DEFAULT NULL,
	DiscountCode VARCHAR(30) NULL DEFAULT NULL,
	DiscountAmount FLOAT NULL DEFAULT NULL,
	PaidAmount FLOAT NULL DEFAULT NULL,
	Status BIT DEFAULT 0,
	StartDate DATE NULL DEFAULT NULL,
	DroppedDate DATE NULL,
	Remarks VARCHAR(50) NULL,
	DeletedAt DATETIME NULL,

	CONSTRAINT ENUM_Enrollments_EnrollmentType CHECK (EnrollmentType in ('Regular', 'Trial', 'Transfer','Special')),
	CONSTRAINT ENUM_Enrollments_PaymentStatus CHECK (PaymentStatus in ('Paid', 'Pending', 'Partially Paid','Overdue','Refunded','Cancelled'))
);

