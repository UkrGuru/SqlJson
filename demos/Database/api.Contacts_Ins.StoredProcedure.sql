SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [api].[Contacts_Ins]
	@Data nvarchar(max) 
AS
INSERT INTO Contacts (FullName, Email, Notes)
SELECT * FROM OPENJSON(@Data) 
WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))

SELECT SCOPE_IDENTITY()
GO
