IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlPositionGrades'))
BEGIN

CREATE TABLE [dbo].[ddlPositionGrades](
	[id] [smallint] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL,
)

END

GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('DF_vPositionGrades_active'))
BEGIN

	ALTER TABLE [dbo].[ddlPositionGrades] ADD  CONSTRAINT [DF_vPositionGrades_active]  DEFAULT ((1)) FOR [active]

END

GO

