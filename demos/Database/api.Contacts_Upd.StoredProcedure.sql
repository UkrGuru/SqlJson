SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [api].[Contacts_Upd]
	@Data nvarchar(max) 
AS
UPDATE C
SET C.FullName = D.FullName, C.Email = D.Email, C.Notes = D.Notes
FROM Contacts C
CROSS JOIN (SELECT * FROM OPENJSON(@Data) 
    WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))) D
WHERE C.Id = JSON_VALUE(@Data,'$.Id')
GO
