SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Contacts_Del]
    @Data nvarchar(50)
AS
DELETE FROM Contacts
WHERE (Id = JSON_VALUE(@Data, '$.Id'))
GO
