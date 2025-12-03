go

if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.ddlSemisters') and type in (N'U'))
begin
CREATE TABLE [dbo].[ddlSemisters](
	[SemisterID] [smallint] IDENTITY(1,1) NOT NULL,
	[code] [char](10) NULL,
	[description] [varchar](50) NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_ddlSemisters] PRIMARY KEY CLUSTERED 
(
	[SemisterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ALTER TABLE [dbo].[ddlSemisters] ADD  DEFAULT ((1)) FOR [Active]

end

GO
