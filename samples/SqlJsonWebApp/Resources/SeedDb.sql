CREATE TABLE [dbo].[Contacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Notes] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET IDENTITY_INSERT [dbo].[Contacts] ON 
INSERT [dbo].[Contacts] ([Id], [FullName], [Email], [Notes]) VALUES (1, N'Oleksandr Viktor', N'ukrguru@gmail.com', N'.NET 5 / C# / SQL Server / Telerik Blazor UI / Bootstrap')
SET IDENTITY_INSERT [dbo].[Contacts] OFF

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Contacts_Del]
    @Data nvarchar(50)
AS
DELETE FROM Contacts
WHERE (Id = JSON_VALUE(@Data, ''$.Id''))
';

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Contacts_Ins]
	@Data nvarchar(max) 
AS
INSERT INTO Contacts (FullName, Email, Notes)
SELECT * FROM OPENJSON(@Data) 
WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))
';

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Contacts_Get]
    @Data nvarchar(50)
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
WHERE Id = JSON_VALUE(@Data, ''$.Id'')
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
';

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Contacts_Lst] 
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
FOR JSON PATH
';

EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[Contacts_Upd]
	@Data nvarchar(max) 
AS
UPDATE C
SET C.FullName = D.FullName, C.Email = D.Email, C.Notes = D.Notes
FROM Contacts C
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))) D
WHERE C.Id = JSON_VALUE(@Data,''$.Id'')
';
