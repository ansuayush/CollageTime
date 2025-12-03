
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlJobFamilys') and type in (N'U'))
CREATE TABLE [dbo].[ddlJobFamilys](
	[JobFamilyId] [smallint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
 CONSTRAINT [PK_ddlJobFamily] PRIMARY KEY CLUSTERED 
(
	[JobFamilyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
