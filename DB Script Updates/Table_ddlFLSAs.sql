
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlFLSAs'))
BEGIN

CREATE TABLE ddlFLSAs(
	[FLSAID] [smallint] IDENTITY(1,1) PRIMARY KEY  NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_ddlFLSA_active]  DEFAULT (1),
)

END

GO

