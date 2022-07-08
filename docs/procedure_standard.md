# UkrGuru.SqlJson Procedure Standard 

## Procedure Parameters Standard 

SqlJson procedures can only have one specific @Data parameter or without parameter.

@Data parameter can be any simple sqltype or complex object in json format.

```sql
CREATE PROCEDURE [dbo].[Demo_Int_Param]
    @Data int
AS
```

```sql
CREATE PROCEDURE [dbo].[Demo_Str_Param]
    @Data varchar(100)
AS
```

```sql
CREATE PROCEDURE [dbo].[Demo_Json1_Param]
    @Data varchar(300)
AS
DECLARE @UserId int = JSON_VALUE(@Data, '$.UserId'), 
    @UserName varchar(100) = JSON_VALUE(@Data, '$.UserName')
```

```sql
CREATE PROCEDURE [dbo].[Demo_Json2_Param]
    @Data varchar(300)
AS
DECLARE @UserId int, @UserName varchar(100), @Birthday smalldatetime

SELECT @UserId = UserId, @UserName = UserName, @Birthday = Birthday
FROM OPENJSON(@Data) 
WITH (UserId int, UserName varchar(100), Birthday smalldatetime)
```

## Procedure Results Standard 

If used FromProcAsync then you need prepare result in json format with "FOR JSON PATH" for List<TEntity> or with "FOR JSON PATH, WITHOUT_ARRAY_WRAPPER" for TEntity

```sql
CREATE PROCEDURE [dbo].[Contacts_List] 
AS
SELECT Id, FullName, Email, Notes
FROM Contacts
FOR JSON PATH
```

```sql
ALTER PROCEDURE [dbo].[Contacts_Ins]
    @Data nvarchar(max) 
AS
INSERT INTO Contacts (FullName, Email, Notes)
SELECT * FROM OPENJSON(@Data) 
WITH (FullName nvarchar(50), Email nvarchar(100), Notes nvarchar(max))

DECLARE @Id int = SCOPE_IDENTITY()

SELECT Id, FullName, Email, Notes
FROM Contacts
WHERE Id = @Id
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
```