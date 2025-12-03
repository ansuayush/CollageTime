
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlBusinessLevelTypes'))
BEGIN

CREATE TABLE dbo.ddlEEOJobCodes(
	[eeoJobCodeID] [smallint] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[eeoFileStatusID] [tinyint] NOT NULL,
)

END

GO

