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
CREATE OR ALTER PROCEDURE TestItems_Del
	@Data int
AS
DELETE TestItems
WHERE Id = @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Get
	@Data int
AS
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Ins
	@Data nvarchar(500)  
AS
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name	nvarchar(50))

SELECT SCOPE_IDENTITY()
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Upd
	@Data nvarchar(500)  
AS
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec0] 
AS
DECLARE @Num0 int = 0
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec1] 
AS
DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcNull] 
AS
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj] 
	@Data varchar(100) = NULL 
AS 
SELECT JSON_VALUE(@Data, ''$.Name'');
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER  PROCEDURE [dbo].[ProcObj1] 
AS 
SELECT 1 Id, ''John'' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj2] 
AS 
SELECT 1 Id, ''John'' Name UNION ALL SELECT 2 Id, ''Mike'' Name FOR JSON PATH
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVar] 
	@Data sql_variant = NULL 
AS 
SELECT @Data;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarBin] 
	@Data varbinary(max) = NULL 
AS 
SELECT @Data;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarChar] 
	@Data nvarchar(max) = NULL 
AS 
SELECT @Data;
';