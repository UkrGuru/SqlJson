EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[TestProc] 
AS 
SELECT ''OK''
';