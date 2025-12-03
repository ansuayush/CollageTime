
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlUnions') and type in (N'U'))
CREATE TABLE [dbo].[ddlUnions](
	[unionID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_vUnions_active]  DEFAULT (1),
 CONSTRAINT [PK_vUnions] PRIMARY KEY CLUSTERED 
(
	[unionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
