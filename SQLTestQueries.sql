/*
SELECT TOP (1000) [Id]
      ,[ProcessName]
      ,[StartTime]
      ,[EndTime]
      ,[Status]
  FROM [AdventureWorks2019].[dbo].[AsyncProcesses]
  ORDER BY [StartTime] ASC
*/

  -- EXEC ShortRunningProcedure
  -- DELETE FROM [AdventureWorks2019].[dbo].[AsyncProcesses] WHERE [StartTime] <= '2023-05-27 16:31:53.963'

  SELECT TOP (1000)
    [Id],
    [ProcessName],
    [StartTime],
    [EndTime],
    [Status],
    CASE
        WHEN [EndTime] IS NULL THEN DATEDIFF(second, [StartTime], GETDATE())
        ELSE DATEDIFF(second, [StartTime], [EndTime])
    END AS [SecondsElapsed],
    CASE
        WHEN [EndTime] IS NULL THEN DATEDIFF(minute, [StartTime], GETDATE())
        ELSE DATEDIFF(minute, [StartTime], [EndTime])
    END AS [MinutesElapsed]
FROM [AdventureWorks2019].[dbo].[AsyncProcesses]
ORDER BY [StartTime] ASC;
