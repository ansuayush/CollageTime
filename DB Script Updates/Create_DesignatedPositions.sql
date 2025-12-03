USE [DUBlankDB_ForNewImport318New]
GO

/****** Object:  Table [dbo].[DesignatedPositions]    Script Date: 5/11/2018 3:35:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DesignatedPositions](
	[ManagerPersonId] [int] NOT NULL,
	[E_PositionId] [int] NOT NULL,
 CONSTRAINT [PK_DesignatedPositions] PRIMARY KEY CLUSTERED 
(
	[ManagerPersonId] ASC,
	[E_PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

