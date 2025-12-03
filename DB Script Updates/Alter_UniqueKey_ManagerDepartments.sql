
/****** Object:  Index [uq_ManagerDepartments]    Script Date: 5/16/2018 6:50:55 PM ******/
ALTER TABLE [dbo].[ManagerDepartments] DROP CONSTRAINT [uq_ManagerDepartments]
GO

/****** Object:  Index [uq_ManagerDepartments]    Script Date: 5/16/2018 6:50:55 PM ******/
ALTER TABLE [dbo].[ManagerDepartments] ADD  CONSTRAINT [uq_ManagerDepartments] UNIQUE NONCLUSTERED 
(
	[ManagerId] ASC,
	[DepartmentId] ASC,
	[IsDesignated]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


