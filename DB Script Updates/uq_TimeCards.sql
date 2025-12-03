USE [DUBlankDB]
GO

/****** Object:  Index [uq_TimeCards]    Script Date: 2/26/2018 5:18:43 PM ******/
ALTER TABLE [dbo].[TimeCards] DROP CONSTRAINT [uq_TimeCards]
GO

/****** Object:  Index [uq_TimeCards]    Script Date: 2/26/2018 5:18:43 PM ******/
ALTER TABLE [dbo].[TimeCards] ADD  CONSTRAINT [uq_TimeCards] UNIQUE NONCLUSTERED 
(
	[CompanyCodeId] ASC,
	[EmployeeId] ASC,
	[ActualDate] ASC,
	[PositionId] ASC,
	[ProjectNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

