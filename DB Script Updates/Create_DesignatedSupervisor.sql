USE [DUBlankDB_ForNewImport318New]
GO

/****** Object:  Table [dbo].[DesignatedSupervisors]    Script Date: 4/27/2018 12:12:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DesignatedSupervisors](
	[ManagerPersonId] [int] NOT NULL,
	[DesignatedManagerPersonId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_DesignatedSupervisors] PRIMARY KEY CLUSTERED 
(
	[ManagerPersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

