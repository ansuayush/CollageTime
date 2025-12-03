
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlJobClasses') and type in (N'U'))
CREATE TABLE [dbo].[ddlJobClasses](
	[jobClassID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_vJobClasses_active]  DEFAULT (1),
 CONSTRAINT [PK_vJobClasses] PRIMARY KEY CLUSTERED 
(
	[jobClassID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
