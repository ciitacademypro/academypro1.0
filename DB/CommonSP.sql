CREATE TYPE ColumnValuePair_type AS TABLE
(
    ColumnName NVARCHAR(50),
    Value NVARCHAR(100) -- Adjust the size based on your needs
);


CREATE PROCEDURE sp_CheckExist_TableRecords
    @tableName1 NVARCHAR(256),
	@PrimaryKeyColumn NVARCHAR(256),
    @conditions ColumnValuePair_type READONLY
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX);
    DECLARE @columnName NVARCHAR(256);
    DECLARE @value NVARCHAR(256);
    DECLARE @primaryKey NVARCHAR(256) = @PrimaryKeyColumn; -- Replace with your actual primary key column name

    -- Initialize a temporary table to store results
    CREATE TABLE #Results (ColumnName NVARCHAR(50), PrimaryKeyValue INT);

    -- Process each condition
    DECLARE cur CURSOR FOR SELECT ColumnName, Value FROM @conditions;
    OPEN cur;

    FETCH NEXT FROM cur INTO @columnName, @value;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Check for matching records
        SET @sql = 'IF EXISTS (SELECT 1 FROM ' + QUOTENAME(@tableName1) + ' WHERE ' + QUOTENAME(@columnName) + ' = @value) ' +
                   'BEGIN ' +
                   'INSERT INTO #Results (ColumnName, PrimaryKeyValue) ' +
                   'SELECT ''' + @columnName + ''', ' + @primaryKey + ' ' +
                   'FROM ' + QUOTENAME(@tableName1) + ' WHERE ' + QUOTENAME(@columnName) + ' = @value; ' +
                   'END ' +
                   'ELSE ' +
                   'BEGIN ' +
                   'INSERT INTO #Results (ColumnName, PrimaryKeyValue) ' +
                   'VALUES (''' + @columnName + ''', 0); ' +
                   'END';

        -- Execute the constructed SQL
        EXEC sp_executesql @sql, N'@value NVARCHAR(256)', @value;

        FETCH NEXT FROM cur INTO @columnName, @value;
    END

    CLOSE cur;
    DEALLOCATE cur;

    -- Return results
    SELECT * FROM #Results;

    -- Clean up
    DROP TABLE #Results;
END

CREATE PROCEDURE sp_GetCount_tables
(
    @TableName VARCHAR(80),
    @Count INT OUTPUT
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Build the dynamic SQL query
    SET @SQL = N'SELECT @Count = COUNT(*) FROM ' + QUOTENAME(@TableName);

    -- Execute the dynamic SQL query
    EXEC sp_executesql @SQL, N'@Count INT OUTPUT', @Count OUTPUT;
END;


CREATE PROCEDURE sp_GetCount_FirmUsers
(
    @FirmId VARCHAR(80),
    @Count INT OUTPUT
)
AS
BEGIN
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Build the dynamic SQL query using parameters
    SET @SQL = N'SELECT @Count = COUNT(*) FROM AspNetUsers WHERE FirmId = @FirmId';

    -- Execute the dynamic SQL query with parameters
    EXEC sp_executesql @SQL, N'@FirmId VARCHAR(80), @Count INT OUTPUT', @FirmId, @Count OUTPUT;
END;


