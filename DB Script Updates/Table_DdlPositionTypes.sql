
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlPositionTypes') and type in (N'U'))
begin
CREATE TABLE [dbo].[ddlPositionTypes](
	[id] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_vPositionTypes] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end
GO 


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('DF_vPositionTypes_active'))
BEGIN

	ALTER TABLE [dbo].[ddlPositionTypes] ADD  CONSTRAINT [DF_vPositionTypes_active]  DEFAULT ((1)) FOR [active]

END

GO

