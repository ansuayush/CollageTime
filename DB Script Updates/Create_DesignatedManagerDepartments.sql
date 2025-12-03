USE [DUBlankDB_ForNewImport318New]
GO

/****** Object:  Table [dbo].[DesignatedManagerDepartments]    Script Date: 5/15/2018 5:29:15 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DesignatedManagerDepartments](
	[ManagerPersonId] [int] NOT NULL,
	[ManagerDepartmentId] [int] NOT NULL,
 CONSTRAINT [PK_DesignatedManagerDepartments] PRIMARY KEY CLUSTERED 
(
	[ManagerPersonId] ASC,
	[ManagerDepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

