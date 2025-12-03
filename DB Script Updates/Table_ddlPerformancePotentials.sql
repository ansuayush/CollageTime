
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ddlPerformancePotentials'))
BEGIN

CREATE TABLE [dbo].[ddlPerformancePotentials](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_vPerformancePotentials] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO 
