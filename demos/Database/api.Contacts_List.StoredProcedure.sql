SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [api].[Contacts_List] 
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
FOR JSON PATH
GO
