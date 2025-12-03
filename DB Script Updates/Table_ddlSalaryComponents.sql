IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlSalaryComponents'))
BEGIN
CREATE TABLE [dbo].[ddlSalaryComponents](
	[salaryComponentID] [smallint] IDENTITY(1,1) NOT NULL,
	[description] [varchar](50) NOT NULL,
	[code] [varchar](10) NOT NULL,
	[salaryComponentTypeID] [smallint] NOT NULL,
	[payTypeID] [smallint]  NULL,	
	[payFrequencyCode] [varchar](10) NOT NULL,
	[benefits] [bit] NOT NULL,
	[Base] [bit] NOT NULL,
	[total] [bit] NOT NULL,
	[active] [bit] NOT NULL CONSTRAINT [DF_vSalaryComponents_active]  DEFAULT (1),
 CONSTRAINT [PK_vSalaryComponents] PRIMARY KEY CLUSTERED 
(
	[salaryComponentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO