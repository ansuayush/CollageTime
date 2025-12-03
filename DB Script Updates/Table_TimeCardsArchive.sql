

/****** Object:  Table [dbo].[TimeCardsArchive]    Script Date: 11/25/2017 3:49:40 PM ******/
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('TimeCardsArchive'))
BEGIN

CREATE TABLE [dbo].[TimeCardsArchive](
	[TimeCardsArchiveId] [int] NOT NULL,
	[CompanyCodeId] [int] NOT NULL,
	[FileNumber] [nvarchar](50) NOT NULL,
	[ActualDate] [smalldatetime] NOT NULL,
	[ProjectNumber] [int] NOT NULL,
	[DailyHours] [float] NULL,
	[TimeIn] [datetime] NULL,
	[LunchOut] [datetime] NULL,
	[LunchBack] [datetime] NULL,
	[TimeOut] [datetime] NULL,
	[HoursCodeId] [int] NULL,
	[Hours] [float] NULL,
	[HoursCodeReasonId] [int] NULL,
	[EarningsCodeId] [smallint] NULL,
	[EarningsAmount] [float] NULL,
	[DepartmentId] [int] NULL,
	[JobId] [int] NULL,
	[TempDeptId] [int] NULL,
	[TempJobId] [int] NULL,
	[Project] [varchar](70) NULL,
	[Task] [varchar](70) NULL,
	[OT] [float] NULL,
	[MealsTaken] [int] NULL,
	[Rate] [float] NULL,
	[HoursCodeRate] [float] NULL,
	[EnteredBy] [varchar](50) NULL,
	[EnteredDate] [datetime] NULL,
	[IsApproved] [bit] NULL,
	[ApprovedBy] [varchar](50) NULL,
 CONSTRAINT [PK_TimeCardsArchive] PRIMARY KEY CLUSTERED 
(
	[TimeCardsArchiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
End

GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_CompanyCodes'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_CompanyCodes] FOREIGN KEY([CompanyCodeId])
REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_CompanyCodes]
end
GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_Departments'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_Departments]
end
GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_Departments1'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Departments1] FOREIGN KEY([TempDeptId])
REFERENCES [dbo].[Departments] ([DepartmentId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_Departments1]
End
GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_EarningsCodes'))
BEGIN

ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_EarningsCodes] FOREIGN KEY([EarningsCodeId])
REFERENCES [dbo].[EarningsCodes] ([EarningsCodeId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_EarningsCodes]
end
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_HoursCodes'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_HoursCodes] FOREIGN KEY([HoursCodeId])
REFERENCES [dbo].[HoursCodes] ([HoursCodeId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_HoursCodes]
end
GO


GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_Jobs'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Jobs] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_Jobs]
end
GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_Jobs1'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Jobs1] FOREIGN KEY([TempJobId])
REFERENCES [dbo].[Jobs] ([JobId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_Jobs1]
end
GO

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardsArchive_TimeCardsArchive'))
BEGIN
ALTER TABLE [dbo].[TimeCardsArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_TimeCardsArchive] FOREIGN KEY([TimeCardsArchiveId])
REFERENCES [dbo].[TimeCardsArchive] ([TimeCardsArchiveId])
ALTER TABLE [dbo].[TimeCardsArchive] CHECK CONSTRAINT [FK_TimeCardsArchive_TimeCardsArchive]
end


