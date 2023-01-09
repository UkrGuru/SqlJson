BEGIN /***** Init Tables *****/

BEGIN /*** Init WJbFiles ***/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbFiles]') AND type in (N'U'))
BEGIN
	CREATE TABLE [WJbFiles](
		[Id] [uniqueidentifier] NOT NULL,
		[Created] [datetime] NOT NULL,
		[FileName] [nvarchar](100) NULL,
		[FileContent] [varbinary](max) NULL,
	 CONSTRAINT [PK_WJbFiles] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbFiles]') AND name = N'IX_WJbFiles_CreatedDesc')
	CREATE NONCLUSTERED INDEX [IX_WJbFiles_CreatedDesc] ON [WJbFiles]
	(
		[Created] DESC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'WJbFiles') AND name = N'SHA1') 
    ALTER TABLE WJbFiles ADD SHA1 [binary](20)

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbFiles]') AND name = N'IX_WJbFiles_SHA1_FileName')
	CREATE NONCLUSTERED INDEX [IX_WJbFiles_SHA1_FileName] ON [dbo].[WJbFiles]
	(
		[SHA1] ASC,
		[FileName] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbFiles_Id]') AND type = 'D')
	ALTER TABLE [WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Id]  DEFAULT (newid()) FOR [Id]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbFiles_Created]') AND type = 'D')
	ALTER TABLE [WJbFiles] ADD  CONSTRAINT [DF_WJbFiles_Created]  DEFAULT (getdate()) FOR [Created]

END

BEGIN /*** Init WJbLogs ***/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND type in (N'U'))
	CREATE TABLE [WJbLogs](
		[Logged] [datetime] NOT NULL,
		[LogLevel] [tinyint] NOT NULL,
		[Title] [nvarchar](100) NOT NULL,
		[LogMore] [nvarchar](max) NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[WJbLogs]') AND name = N'IX_WJbLogs_LoggedDesc')
	CREATE NONCLUSTERED INDEX [IX_WJbLogs_LoggedDesc] ON [WJbLogs]
	(
		[Logged] DESC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[DF_WJbLogs_Created]') AND type = 'D')
ALTER TABLE [WJbLogs] ADD  CONSTRAINT [DF_WJbLogs_Created]  DEFAULT (getdate()) FOR [Logged]
END

END

BEGIN /*** WJbFiles Procs ***/
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Get]
	@Data uniqueidentifier = NULL
AS
SELECT Id, Created, FileName, FileContent, 
	CAST(CASE WHEN Created = CAST(Created as smalldatetime) THEN 1 ELSE 0 END AS bit) Safe
FROM WJbFiles
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Get_api]
	@Data uniqueidentifier = NULL
AS
EXEC WJbFiles_Get @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Ins]
    @Data nvarchar(max)
AS
DECLARE @Id uniqueidentifier = NULL
DECLARE @SHA1 binary(20), @FileName nvarchar(100), @FileContent varbinary(max), @Safe bit, @Created datetime

SELECT TOP 1 @SHA1 = HashBytes(''SHA1'', FileContent), @FileName = [FileName], @FileContent = FileContent, @Safe = Safe
FROM OPENJSON(@Data)
WITH ([FileName] nvarchar(100), FileContent varbinary(max), Safe bit)

SELECT @Id = Id FROM WJbFiles WHERE SHA1 = @SHA1 AND FileName = @FileName

IF @Id IS NULL BEGIN
	SET @Id = NEWID();
	IF ISNULL(@Safe, 0) = 1 SET @Created = CAST(GETDATE() AS smalldatetime);

	INSERT WJbFiles (Id, Created, FileName, FileContent, SHA1)
	SELECT @Id, ISNULL(@Created, GETDATE()), @FileName, @FileContent, @SHA1 
END
ELSE IF @Safe = 1
	UPDATE WJbFiles SET Created = CAST(Created as smalldatetime) WHERE Id = @Id   

SELECT CAST(@Id as varchar(50)) Id
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Ins_api]
    @Data nvarchar(max)
AS
EXEC WJbFiles_Ins @Data
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Del]
	@Data uniqueidentifier = NULL
AS
DELETE WJbFiles
WHERE Id = @Data
';
END
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbFiles_Del_api]
	@Data uniqueidentifier = NULL
AS
EXEC WJbFiles_Del @Data
';

BEGIN /*** WJbLogs Procs ***/
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbLogs_Ins]
    @Data nvarchar(max)
AS
INSERT INTO [WJbLogs] ([Logged], [LogLevel], [Title], [LogMore])
VALUES (GETDATE(),JSON_VALUE(@Data, ''$.LogLevel''), JSON_VALUE(@Data, ''$.Title''), 
    ISNULL(JSON_QUERY(@Data, ''$.LogMore''), JSON_VALUE(@Data, ''$.LogMore'')))
';
EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [WJbLogs_Ins_api]
    @Data nvarchar(max)
AS
EXEC WJbLogs_Ins @Data
';
END
