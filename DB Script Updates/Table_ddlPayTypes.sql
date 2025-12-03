IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlPayTypes'))
BEGIN
CREATE TABLE [dbo].[ddlPayTypes](
	[payTypeID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[active] [bit] NOT NULL,
 CONSTRAINT [PK_vValueTypes] PRIMARY KEY CLUSTERED 
(
	[payTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[ddlPayTypes] ADD  CONSTRAINT [DF_ddlPayTypes_active]  DEFAULT ((1)) FOR [active]
end
go