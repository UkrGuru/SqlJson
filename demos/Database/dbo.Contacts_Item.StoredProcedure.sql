SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[Contacts_Item]
    @Data nvarchar(50)
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
WHERE Id = JSON_VALUE(@Data, '$.Id')
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO
