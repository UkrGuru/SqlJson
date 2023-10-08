IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[InputVar]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[InputVar](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Data] [sql_variant] NULL,
	 CONSTRAINT [PK_InputData] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[InputVarBin]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[InputVarBin](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Data] [varbinary](max) NULL,
	 CONSTRAINT [PK_InputVarBin] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[InputVarChar]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[InputVarChar](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Data] [nvarchar](max) NULL,
	 CONSTRAINT [PK_InputVarChar] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

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

TRUNCATE TABLE [dbo].[InputVar]
TRUNCATE TABLE [dbo].[InputVarBin]
TRUNCATE TABLE [dbo].[InputVarChar]

TRUNCATE TABLE [dbo].[TestItems]

TRUNCATE TABLE [dbo].[WJbFiles]

TRUNCATE TABLE [dbo].[WJbLogs]


EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[CalcErr] 
	@Data nvarchar(200) = NULL 
AS 
IF LEN(JSON_VALUE(@Data,''$.LogLevel'')) > 0 BEGIN
	SELECT COUNT(*) 
	FROM WJbLogs 
	WHERE Title LIKE ''% '' + JSON_VALUE(@Data,''$.Title'') + '' %''
	AND LogLevel = JSON_VALUE(@Data, ''$.LogLevel'')
END
	SELECT COUNT(*) 
	FROM WJbLogs 
	WHERE Title LIKE ''% '' + JSON_VALUE(@Data, ''$.Title'') + '' %'' 
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[CalcErr_Api]
	@Data nvarchar(200) = NULL 
AS
EXEC CalcErr @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[DelErr] 
	@Data nvarchar(200) = NULL 
AS 
DELETE FROM WJbLogs 
WHERE Title LIKE @Data;
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[DelErr_Api] 
	@Data nvarchar(200) = NULL 
AS
EXEC DelErr @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Del
	@Data int = NULL
AS
DELETE TestItems
WHERE Id = @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Del_Api
	@Data int = NULL
AS
EXEC TestItems_Del @Data
';


EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Get
	@Data int = NULL
AS
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Get_Api
	@Data int = NULL
AS
EXEC TestItems_Get @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Ins
	@Data nvarchar(500) = NULL
AS
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name	nvarchar(50))

SELECT SCOPE_IDENTITY()
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Ins_Api
	@Data nvarchar(500) = NULL
AS
EXEC TestItems_Ins @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Upd
	@Data nvarchar(500) = NULL  
AS
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Upd_Api
	@Data nvarchar(500) = NULL  
AS
EXEC TestItems_Upd @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec0] 
AS
DECLARE @Num0 int = 0
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec0_Api] 
AS
EXEC Exec0
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec1] 
AS
DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Exec1_Api] 
AS
EXEC Exec1
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcNull] 
AS
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcNull_Api] 
AS
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj] 
	@Data varchar(100) = NULL 
AS 
SELECT JSON_VALUE(@Data, ''$.Name'');
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj_Api] 
	@Data varchar(100) = NULL 
AS 
EXEC ProcObj @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER  PROCEDURE [dbo].[ProcObj1] 
AS 
SELECT 1 Id, ''John'' Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER  PROCEDURE [dbo].[ProcObj1_Api] 
AS 
EXEC ProcObj1
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj2] 
AS 
SELECT 1 Id, ''John'' Name UNION ALL SELECT 2 Id, ''Mike'' Name FOR JSON PATH
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcObj2_Api] 
AS
EXEC ProcObj2
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVar] 
	@Data sql_variant = NULL 
AS 
SELECT @Data;
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVar_Api] 
	@Data sql_variant = NULL 
AS
EXEC ProcVar @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarBin] 
	@Data varbinary(max) = NULL 
AS 
SELECT @Data;
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarBin_Api] 
	@Data varbinary(max) = NULL 
AS
EXEC ProcVarBin @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarChar] 
	@Data nvarchar(max) = NULL 
AS 
SELECT @Data;
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[ProcVarChar_Api] 
	@Data nvarchar(max) = NULL 
AS 
EXEC ProcVarChar @Data
';