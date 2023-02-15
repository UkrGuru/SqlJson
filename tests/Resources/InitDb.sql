IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[TestItems]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[TestItems](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[Name] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_TestItems] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

TRUNCATE TABLE [dbo].[TestItems]

TRUNCATE TABLE [dbo].[WJbFiles]

TRUNCATE TABLE [dbo].[WJbLogs]

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcNull 
AS
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcInt 
	@Data int = NULL 
AS 
SELECT @Data;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcStr 
	@Data varchar(100) = NULL 
AS 
SELECT @Data;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcObj 
	@Data varchar(100) = NULL 
AS 
SELECT JSON_VALUE(@Data, ''$.Name'');
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Ins
	@Data nvarchar(500)  
AS
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name	nvarchar(50))
SELECT CAST(SCOPE_IDENTITY() AS int)
';