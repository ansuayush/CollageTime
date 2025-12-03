
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlWorkersCompensations') and type in (N'U'))
CREATE TABLE [dbo].[ddlWorkersCompensations](
	[workersCompensationID] [smallint] IDENTITY(1,1) NOT NULL,
	[code] [char](10) NULL,
	[description] [varchar](50) NULL,
 CONSTRAINT [PK_ddlHRWorkersCompensation] PRIMARY KEY CLUSTERED 
(
	[workersCompensationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
