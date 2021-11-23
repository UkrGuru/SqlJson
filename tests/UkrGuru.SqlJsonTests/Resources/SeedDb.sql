IF NOT  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Regions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Regions](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [Name] [varchar](50) NOT NULL,
     CONSTRAINT [PK_Regions] PRIMARY KEY NONCLUSTERED 
    (
	    [Id] ASC
    )
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END

SET IDENTITY_INSERT [dbo].[Regions] ON 
INSERT [dbo].[Regions] ([Id], [Name]) VALUES (1, N'Eastern')
INSERT [dbo].[Regions] ([Id], [Name]) VALUES (2, N'Western')
INSERT [dbo].[Regions] ([Id], [Name]) VALUES (3, N'Northern')
INSERT [dbo].[Regions] ([Id], [Name]) VALUES (4, N'Southern')
SET IDENTITY_INSERT [dbo].[Regions] OFF

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Regions_Lst] 
AS
SELECT Id, Name
FROM Regions
FOR JSON PATH';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Regions_Get] 
    @Data varchar(20)
AS
SELECT Id, Name
FROM Regions
WHERE Id = JSON_VALUE(@Data, ''$.Id'')
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Regions_Upd] 
    @Data varchar(200)
AS
UPDATE Regions
SET Name = JSON_VALUE(@Data, ''$.Name'')
WHERE (Id = JSON_VALUE(@Data, ''$.Id''))';

EXEC dbo.sp_executesql @statement = N'
CREATE OR ALTER PROCEDURE [dbo].[Regions_Del] 
    @Data varchar(20)
AS
DELETE FROM Regions
WHERE (Id = JSON_VALUE(@Data, ''$.Id''))';
