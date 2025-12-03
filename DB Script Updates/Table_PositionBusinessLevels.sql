
if not exists (select 1 from sys.objects where object_id = object_id(N'dbo.PositionBusinessLevels') and type in (N'U'))
begin
CREATE TABLE [dbo].[PositionBusinessLevels](
	[BusinessLevelNbr] [int] IDENTITY(1,1) NOT NULL,
	[BusinessLevelNotes] [nvarchar](350) NOT NULL,
	[BusinessLevelTitle] [nvarchar](250) NOT NULL,
	[BusinessLevelTypeNbr] [tinyint] NULL,
	[ParentBULevelNbr] [int] NULL,
	[LocationId] [smallint] NULL,
	[EEoFileStatusNbr] [int] NULL,
	[FedralEINNbr] [int] NULL,
	[PayFrequencyId] [tinyint] NULL,
	[SchedeuledHours] [int] NULL,
	[Active] [bit] NULL,
	[EnteredDate] [datetime] NULL,
	[EnteredBy] [nvarchar](250) NULL,
	[ModifiedDate] [datetime] NULL,
	[ModifiedBy] [nvarchar](250) NULL,
	[BusinessLevelCode] [nvarchar](500) NULL,
	[BudgetReported] [varchar](20) NULL,
 CONSTRAINT [PK_PositionBusinessLevels] PRIMARY KEY CLUSTERED 
(
	[BusinessLevelNbr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBusinessLevels_ddlBusinessLevelTypes]'))
BEGIN
	ALTER TABLE [dbo].[PositionBusinessLevels]  WITH CHECK ADD  CONSTRAINT [FK_PositionBusinessLevels_ddlBusinessLevelTypes] FOREIGN KEY([BusinessLevelTypeNbr])
	REFERENCES [dbo].[ddlBusinessLevelTypes] ([BusinessLevelTypeNbr])
END
GO



IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBusinessLevels_DdlEEOFileStatuses]'))
BEGIN
ALTER TABLE [dbo].[PositionBusinessLevels]  WITH CHECK ADD  CONSTRAINT [FK_PositionBusinessLevels_DdlEEOFileStatuses] FOREIGN KEY([EEoFileStatusNbr])
REFERENCES [dbo].[ddlEEOFileStatuses] ([EEoFileStatusNbr])
END
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBusinessLevels_ddlEINs]'))
BEGIN
ALTER TABLE [dbo].[PositionBusinessLevels]  WITH CHECK ADD  CONSTRAINT [FK_PositionBusinessLevels_ddlEINs] FOREIGN KEY([FedralEINNbr])
REFERENCES [dbo].[ddlEINs] ([FedralEINNbr])
END
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBusinessLevels_DdlPayFrequencies]'))
BEGIN
ALTER TABLE [dbo].[PositionBusinessLevels]  WITH CHECK ADD  CONSTRAINT [FK_PositionBusinessLevels_DdlPayFrequencies] FOREIGN KEY([PayFrequencyId])
REFERENCES [dbo].[DdlPayFrequencies] ([PayFrequencyId])
END
GO



IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBusinessLevels_Locations]'))
BEGIN
ALTER TABLE [dbo].[PositionBusinessLevels]  WITH CHECK ADD  CONSTRAINT [FK_PositionBusinessLevels_Locations] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Locations] ([LocationId])
END
GO

