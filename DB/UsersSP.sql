
EXEC sp_GetAll_Users 3, 'ADMIN';



CREATE PROCEDURE sp_GetAll_Users
(
    @FirmId INT = 0, 
    @RoleList VARCHAR(MAX) = '', -- Comma-separated list of roles
    @Status BIT = NULL,
    @BranchId INT = 0,
    @Id INT = 0  -- Optional parameter for filtering by UserId
)
AS
BEGIN
    -- Split the roles into a table variable
    DECLARE @RoleTable TABLE (RoleName VARCHAR(50));
    IF (@RoleList <> '')
    BEGIN
        INSERT INTO @RoleTable (RoleName)
        SELECT value
        FROM STRING_SPLIT(@RoleList, ','); -- Split roles using STRING_SPLIT (SQL Server 2016+)
    END

    -- Fetch a specific user if @Id is provided
    IF (@Id <> 0)
    BEGIN
        SELECT 
            s.Id,
            s.FirmId,
            f.FirmName,
            s.BranchId,
            b.BranchName,
			r.Id RoleId,
			r.Name RoleName,
            s.Name,
            s.Uid1,
            s.PhoneNumber,
            s.Email,
            s.Status,
            CASE 
                WHEN s.Status = 1 THEN 'Active' 
                WHEN s.Status = 0 THEN 'Inactive' 
                ELSE 'Unknown'
            END AS StatusLabel,  -- Custom StatusLabel column
            s.DeletedAt
        FROM 
            AspNetUsers s
            JOIN AspNetUserRoles ur ON ur.UserId = s.Id
            JOIN AspNetRoles r ON r.Id = ur.RoleId
            JOIN Branches b ON b.BranchId = s.BranchId
            JOIN Firms f ON f.FirmId = s.FirmId
        WHERE 
            s.DeletedAt IS NULL 
            AND s.Id = @Id  -- Fetch a specific user
            AND (@FirmId = 0 OR s.FirmId = @FirmId) 
            AND (@BranchId = 0 OR s.BranchId = @BranchId)
            AND (@Status IS NULL OR s.Status = @Status)
            AND (NOT EXISTS (SELECT 1 FROM @RoleTable) OR r.Name IN (SELECT RoleName FROM @RoleTable))
        ORDER BY 
            s.Name ASC;
    END
    ELSE -- Fetch all users with optional filtering
    BEGIN
        SELECT 
            s.Id,
            s.FirmId,
            f.FirmName,
            s.BranchId,
            b.BranchName,
			r.Id RoleId,
			r.Name RoleName,
            s.Name,
            s.Uid1,
            s.PhoneNumber,
            s.Email,
            s.Status,
            CASE 
                WHEN s.Status = 1 THEN 'Active' 
                WHEN s.Status = 0 THEN 'Inactive' 
                ELSE 'Unknown'
            END AS StatusLabel,  -- Custom StatusLabel column
            s.DeletedAt
        FROM 
            AspNetUsers s
            JOIN AspNetUserRoles ur ON ur.UserId = s.Id
            JOIN AspNetRoles r ON r.Id = ur.RoleId
            JOIN Branches b ON b.BranchId = s.BranchId
            JOIN Firms f ON f.FirmId = s.FirmId
        WHERE 
            s.DeletedAt IS NULL  -- Fetch only non-deleted records
            AND (@FirmId = 0 OR s.FirmId = @FirmId) 
            AND (@BranchId = 0 OR s.BranchId = @BranchId)
            AND (@Status IS NULL OR s.Status = @Status)
            AND (NOT EXISTS (SELECT 1 FROM @RoleTable) OR r.Name IN (SELECT RoleName FROM @RoleTable))
        ORDER BY 
            s.Name ASC;
    END
END;
