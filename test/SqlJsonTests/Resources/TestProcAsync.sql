EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[TestProcAsync] 
AS 
SELECT ''OK''
';