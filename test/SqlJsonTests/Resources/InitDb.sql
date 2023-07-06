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
CREATE OR ALTER PROCEDURE ProcNull_Api 
AS
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcInt 
	@Data int = NULL 
AS 
SELECT @Data;
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcInt_Api 
	@Data int = NULL 
AS 
EXEC ProcInt @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcStr 
	@Data varchar(100) = NULL 
AS 
SELECT @Data;
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcStr_Api 
	@Data varchar(100) = NULL 
AS
EXEC ProcStr @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcObj 
	@Data varchar(100) = NULL 
AS 
SELECT JSON_VALUE(@Data, ''$.Name'');
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE ProcObj_Api
	@Data varchar(100) = NULL 
AS 
EXEC ProcObj @Data
';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Del
	@Data int
AS
DELETE TestItems
WHERE Id = @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE TestItems_Del_Api
	@Data int
AS
EXEC TestItems_Del @Data
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
CREATE OR ALTER PROCEDURE TestItems_Get_Api
	@Data int
AS
EXEC TestItems_Get @Data
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
CREATE OR ALTER PROCEDURE TestItems_Ins_Api
	@Data nvarchar(500)  
AS
EXEC TestItems_Ins @Data
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
CREATE OR ALTER PROCEDURE TestItems_Upd_Api
	@Data nvarchar(500)  
AS
EXEC TestItems_Upd @Data
';
