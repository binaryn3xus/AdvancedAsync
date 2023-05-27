IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AsyncProcesses')
BEGIN
    CREATE TABLE AsyncProcesses
    (
        [Id] UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        [ProcessName] VARCHAR(100),
        [StartTime] DATETIME,
        [EndTime] DATETIME,
        [Status] VARCHAR(20)
    );
    PRINT 'AsyncProcesses table created successfully.';
END
ELSE
    PRINT 'AsyncProcesses table already exists.';
GO

-- Create or alter the ShortRunningProcedure
CREATE OR ALTER PROCEDURE ShortRunningProcedure
AS
BEGIN
    DECLARE @processId UNIQUEIDENTIFIER = NEWID();

    -- Insert record into AsyncProcesses table with start time and Id
    INSERT INTO AsyncProcesses (Id, ProcessName, StartTime, [Status])
    VALUES (@processId, 'ShortRunningProcedure', GETDATE(), 'Running');

    -- Delay execution
    WAITFOR DELAY '00:01:00';

    -- Update end time in AsyncProcesses table
    UPDATE AsyncProcesses
    SET EndTime = GETDATE(), [Status] = 'Completed'
    WHERE Id = @processId;
END
GO

-- Create or alter the MediumRunningProcedure
CREATE OR ALTER PROCEDURE MediumRunningProcedure
AS
BEGIN
    DECLARE @processId UNIQUEIDENTIFIER = NEWID();

    -- Insert record into AsyncProcesses table with start time and Id
    INSERT INTO AsyncProcesses (Id, ProcessName, StartTime, [Status])
    VALUES (@processId, 'MediumRunningProcedure', GETDATE(), 'Running');

    -- Delay execution
    WAITFOR DELAY '00:05:00';

    -- Update end time in AsyncProcesses table
    UPDATE AsyncProcesses
    SET EndTime = GETDATE(), [Status] = 'Completed'
    WHERE Id = @processId;
END
GO

-- Create or alter the LongRunningProcedure
CREATE OR ALTER PROCEDURE LongRunningProcedure
AS
BEGIN
    DECLARE @processId UNIQUEIDENTIFIER = NEWID();

    -- Insert record into AsyncProcesses table with start time and Id
    INSERT INTO AsyncProcesses (Id, ProcessName, StartTime, [Status])
    VALUES (@processId, 'LongRunningProcedure', GETDATE(), 'Running');

    -- Delay execution
    WAITFOR DELAY '00:20:00';

    -- Update end time in AsyncProcesses table
    UPDATE AsyncProcesses
    SET EndTime = GETDATE(), [Status] = 'Completed'
    WHERE Id = @processId;
END
GO
