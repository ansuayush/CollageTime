
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ManagersPositions'))
BEGIN
CREATE TABLE [dbo].[ManagersPositions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ManagerID] [int] NOT NULL,
	[PositionID] [smallint] NOT NULL,
 CONSTRAINT [PK_ManagersPositions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ManagersPositions] UNIQUE NONCLUSTERED 
(
	[ManagerID] ASC,
	[PositionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ManagersPositions_Managers]'))
BEGIN
ALTER TABLE [dbo].[ManagersPositions]  WITH CHECK ADD  CONSTRAINT [FK_ManagersPositions_Managers] FOREIGN KEY([ManagerID])
REFERENCES [dbo].[Managers] ([ManagerId])
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ManagersPositions_Managers]'))
BEGIN
ALTER TABLE [dbo].[ManagersPositions] CHECK CONSTRAINT [FK_ManagersPositions_Managers]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ManagersPositions_Positions]'))
BEGIN
ALTER TABLE [dbo].[ManagersPositions]  WITH CHECK ADD  CONSTRAINT [FK_ManagersPositions_Positions] FOREIGN KEY([PositionID])
REFERENCES [dbo].[Positions] ([PositionId])
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_ManagersPositions_Positions]'))
BEGIN
ALTER TABLE [dbo].[ManagersPositions] CHECK CONSTRAINT [FK_ManagersPositions_Positions]
END
GO


