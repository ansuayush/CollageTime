IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlBusinessLevelTypes'))
BEGIN
   
CREATE TABLE [dbo].[ddlBusinessLevelTypes](
	[BusinessLevelTypeNbr] [tinyint] IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL,
 )

END

GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('DF_ddlBusinessLevelTypes_active'))
BEGIN
	
ALTER TABLE [dbo].[ddlBusinessLevelTypes] ADD  CONSTRAINT [DF_ddlBusinessLevelTypes_active]  DEFAULT ((1)) FOR [active]

END

GO
