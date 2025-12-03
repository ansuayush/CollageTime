
Drop table TimeCardsArchive

GO
/****** Object:  Table [dbo].[TimeCardsArchives]    Script Date: 2/26/2018 2:45:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimeCardsArchives](
	[TimeCardsArchiveId] [int] IDENTITY(1,1) NOT NULL,
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
	[EarningsCodeId] [int] NULL,
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
	[EmployeeId] [int] NULL,
 CONSTRAINT [PK_TimeCardsArchive] PRIMARY KEY CLUSTERED 
(
	[TimeCardsArchiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_CompanyCodes] FOREIGN KEY([CompanyCodeId])
REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_CompanyCodes]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_Departments]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Departments1] FOREIGN KEY([TempDeptId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_Departments1]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_EarningsCodes] FOREIGN KEY([EarningsCodeId])
REFERENCES [dbo].[EarningsCodes] ([EarningsCodeId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_EarningsCodes]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_HoursCodes] FOREIGN KEY([HoursCodeId])
REFERENCES [dbo].[HoursCodes] ([HoursCodeId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_HoursCodes]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Jobs] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_Jobs]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_Jobs1] FOREIGN KEY([TempJobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_Jobs1]
GO

ALTER TABLE [dbo].[TimeCardsArchives]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardsArchive_TimeCardsArchive] FOREIGN KEY([TimeCardsArchiveId])
REFERENCES [dbo].[TimeCardsArchives] ([TimeCardsArchiveId])
GO

ALTER TABLE [dbo].[TimeCardsArchives] CHECK CONSTRAINT [FK_TimeCardsArchive_TimeCardsArchive]
GO


