USE [WorldCartoon]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*
DROP TABLE [dbo].[CartoonCharacter]
DROP TABLE [dbo].[Cartoon]
DROP PROC INS_Cartoon
DROP PROC SEL_Cartoon
DROP PROC INS_CartoonCharacter
DROP PROC SEL_CartoonCharacter
*/

CREATE TABLE [dbo].[Cartoon](
	[Cartoon_Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED ,
	[Name_Cartoon] [nvarchar](80) NOT NULL,
	[Country] [nvarchar](20) NULL)
GO

INSERT INTO Cartoon(Name_Cartoon,Country)
VALUES('The Simpsons','USA')

INSERT INTO Cartoon(Name_Cartoon,Country)
VALUES('The Flintstones','USA')


CREATE TABLE [dbo].[CartoonCharacter](
	[Cartoon_Id] [int] NOT NULL,
	[Cartoon_Character_Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED ,
	[Name_Character] [nvarchar](80) NOT NULL,
	[Description] [nvarchar](50) NULL,
	FOREIGN KEY (Cartoon_Id) REFERENCES Cartoon(Cartoon_Id))
GO

INSERT INTO CartoonCharacter(Cartoon_Id,Name_Character, Description)
VALUES(1,'Homer Simpson','He is the patriarch of the Simpson family')


INSERT INTO CartoonCharacter(Cartoon_Id,Name_Character, Description)
VALUES(2,'Fred Flintstone','He is the patriarch of the Flintstone family')

GO

IF EXISTS(SELECT TOP 1 1  FROM SYS.OBJECTS WHERE TYPE='P' AND NAME='INS_Cartoon')
BEGIN
	DROP PROC INS_Cartoon
END
GO
CREATE PROC INS_Cartoon(
@Name [nvarchar](80),
@Country [nvarchar](20),
@CartoonId INT OUTPUT)
AS
BEGIN

	INSERT INTO Cartoon(Name_Cartoon,Country)
	VALUES(@Name,@Country)

	SELECT @CartoonId = SCOPE_IDENTITY()

END
GO

--DROP PROC dbo.SEL_Cartoon
CREATE PROC dbo.SEL_Cartoon(
@Name_Cartoon [nvarchar](80) =  NULL,
@Country [nvarchar](20) =  NULL)
AS
BEGIN

	SELECT	Cartoon_Id,
			Name_Cartoon,
			Country
	FROM	Cartoon
	WHERE   (@Name_Cartoon IS NULL OR  Name_Cartoon like '%' + @Name_Cartoon) AND
			(@Country IS NULL OR  Country = @Country)

END
GO

--DROP PROC dbo.INS_CartoonCharacter
CREATE PROC dbo.INS_CartoonCharacter(
@Cartoon_Id [int],
@Name [nvarchar](80),
@Description [nvarchar](50) = NULL)
AS
BEGIN

	INSERT INTO CartoonCharacter(Cartoon_Id,Name_Character, Description)
	VALUES(@Cartoon_Id, @Name,@Description)

	SELECT SCOPE_IDENTITY()

END
GO

--DROP PROC dbo.SEL_CartoonCharacter
CREATE PROC dbo.SEL_CartoonCharacter(
@Cartoon_Id [int] = NULL,
@Name [nvarchar](80) = NULL)
AS
BEGIN

	SELECT	CC.* ,
			C.Name_Cartoon,
			C.Country
	FROM	CartoonCharacter CC
	INNER JOIN
			Cartoon C
	ON
			CC.Cartoon_Id = C.Cartoon_Id
	WHERE   (@Cartoon_Id IS NULL OR  C.Cartoon_Id = @Cartoon_Id) AND
			(@Name IS NULL OR  Name_Character LIKE  @Name + '%')

END
GO

