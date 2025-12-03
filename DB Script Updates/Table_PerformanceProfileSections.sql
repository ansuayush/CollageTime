
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('PerformanceProfileSections'))
BEGIN

CREATE TABLE [dbo].[PerformanceProfileSections](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SectionName] [varchar](50) NOT NULL,
	[Header] [varchar](200) NOT NULL,
	[PerProfileID] [int] NOT NULL,
	[NumRows] [int] NOT NULL,
	[MaxCharacters] [int] NOT NULL,
	[Weight] [decimal](18, 2) NOT NULL,
	[Position] [int] NOT NULL,
	[EnteredBy] [varchar](50) NULL,
	[EnteredDate] [smalldatetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedDate] [smalldatetime] NULL,
 CONSTRAINT [PK_PerformanceProfileSections] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_PerformanceProfileSections] UNIQUE NONCLUSTERED 
(
	[PerProfileID] ASC,
	[SectionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PerformanceProfileSections_NumRows]'))
BEGIN
ALTER TABLE [dbo].[PerformanceProfileSections] ADD  CONSTRAINT [DF_PerformanceProfileSections_NumRows]  DEFAULT ((5)) FOR [NumRows]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PerformanceProfileSections_MaxCharacters]'))
BEGIN
ALTER TABLE [dbo].[PerformanceProfileSections] ADD  CONSTRAINT [DF_PerformanceProfileSections_MaxCharacters]  DEFAULT ((200)) FOR [MaxCharacters]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PerformanceProfileSections_Weight]'))
BEGIN
ALTER TABLE [dbo].[PerformanceProfileSections] ADD  CONSTRAINT [DF_PerformanceProfileSections_Weight]  DEFAULT ((100.0)) FOR [Weight]
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PerformanceProfileSections_PerformanceProfile]'))
BEGIN
ALTER TABLE [dbo].[PerformanceProfileSections]  WITH CHECK ADD  CONSTRAINT [FK_PerformanceProfileSections_PerformanceProfile] FOREIGN KEY([PerProfileID])
REFERENCES [dbo].[PerformanceProfiles] ([PerProfileID])
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PerformanceProfileSections_PerformanceProfile]'))
BEGIN
ALTER TABLE [dbo].[PerformanceProfileSections] CHECK CONSTRAINT [FK_PerformanceProfileSections_PerformanceProfile]
END
GO 


