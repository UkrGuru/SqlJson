SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [api].[Contacts_Del]
    @Data varchar(10)
AS
DELETE FROM Contacts
WHERE Id = CAST(@Data AS int)
GO
