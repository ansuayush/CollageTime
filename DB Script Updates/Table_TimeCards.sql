USE [DUBlankDB]
GO

/****** Object:  Table [dbo].[TimeCards]    Script Date: 3/8/2018 2:15:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TimeCards](
	[TimeCardId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyCodeId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[ActualDate] [date] NOT NULL,
	[ProjectNumber] [int] NOT NULL,
	[DailyHours] [float] NULL,
	[HoursCodeId] [int] NULL,
	[Hours] [float] NULL,
	[EarningsCodeId] [int] NULL,
	[EarningsAmount] [float] NULL,
	[TempDeptId] [int] NULL,
	[TempJobId] [int] NULL,
	[TimeIn] [datetime] NULL,
	[TimeOut] [datetime] NULL,
	[LunchOut] [datetime] NULL,
	[LunchBack] [datetime] NULL,
	[IsApproved] [bit] NOT NULL,
	[ApprovedBy] [varchar](50) NULL,
	[HoursCodeReasonId] [int] NULL,
	[FileNumber] [nvarchar](50) NULL,
	[Project] [varchar](70) NULL,
	[Task] [varchar](70) NULL,
	[OT] [float] NULL,
	[MealsTaken] [int] NULL,
	[Rate] [float] NULL,
	[HoursCodeRate] [float] NULL,
	[EnteredBy] [varchar](50) NULL,
	[EnteredDate] [datetime] NULL,
	[DepartmentId] [int] NULL,
	[JobId] [int] NULL,
	[FundsId] [int] NULL,
	[ProjectsId] [int] NULL,
	[TaskId] [int] NULL,
	[PositionId] [int] NULL,
	[UserId] [varchar](20) NULL,
	[LastModifiedDate] [datetime] NULL,
 CONSTRAINT [PK_TimeCards] PRIMARY KEY CLUSTERED 
(
	[TimeCardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uq_TimeCards] UNIQUE NONCLUSTERED 
(
	[CompanyCodeId] ASC,
	[EmployeeId] ASC,
	[ActualDate] ASC,
	[PositionId] ASC,
	[TimeIn] ASC,
	[ProjectNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TimeCards] ADD  CONSTRAINT [DF_vTimeCard_IsApproved]  DEFAULT ((0)) FOR [IsApproved]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_CompanyCodes] FOREIGN KEY([CompanyCodeId])
REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_CompanyCodes]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_Departments] FOREIGN KEY([TempDeptId])
REFERENCES [dbo].[Departments] ([DepartmentId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Departments]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_EarningsCode] FOREIGN KEY([EarningsCodeId])
REFERENCES [dbo].[EarningsCodes] ([EarningsCodeId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_EarningsCode]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_Employees] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Employees]
GO

ALTER TABLE [dbo].[TimeCards]  WITH CHECK ADD  CONSTRAINT [FK_TimeCards_Funds] FOREIGN KEY([FundsId])
REFERENCES [dbo].[Funds] ([ID])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Funds]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_HoursCodes] FOREIGN KEY([HoursCodeId])
REFERENCES [dbo].[HoursCodes] ([HoursCodeId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_HoursCodes]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_Jobs] FOREIGN KEY([TempJobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Jobs]
GO

ALTER TABLE [dbo].[TimeCards]  WITH NOCHECK ADD  CONSTRAINT [FK_TimeCards_Jobs1] FOREIGN KEY([JobId])
REFERENCES [dbo].[Jobs] ([JobId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Jobs1]
GO

ALTER TABLE [dbo].[TimeCards]  WITH CHECK ADD  CONSTRAINT [FK_TimeCards_Positions] FOREIGN KEY([PositionId])
REFERENCES [dbo].[Positions] ([PositionId])
GO

ALTER TABLE [dbo].[TimeCards] CHECK CONSTRAINT [FK_TimeCards_Positions]
GO


