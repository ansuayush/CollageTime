
IF NOT exists (select 1 from sys.objects where object_id = object_id(N'dbo.PositionBudgets') and type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PositionBudgets](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PositionID] [smallint] NOT NULL,
	[BudgetYear] [int] NOT NULL,
	[BudgetMonth] [tinyint] NOT NULL,
	[BudgetAmount] [decimal](18, 2) NOT NULL,
	[BurdenPercent] [decimal](18, 2) NOT NULL,
	[EmployeeID] [int] NULL,
	[FTE] [decimal](18, 9) NOT NULL,
 CONSTRAINT [PK_PositionBudgets] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_PositionBudgets] UNIQUE NONCLUSTERED 
(
	[PositionID] ASC,
	[BudgetYear] ASC,
	[BudgetMonth] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_PositionBudgets_1] UNIQUE NONCLUSTERED 
(
	[PositionID] ASC,
	[BudgetYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

end
go

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PositionBudgets_BudgetMonth]'))
BEGIN
	ALTER TABLE [dbo].[PositionBudgets] ADD  CONSTRAINT [DF_PositionBudgets_BudgetMonth]  DEFAULT ('') FOR [BudgetMonth]
END 
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PositionBudgets_BurdenPercent]'))
BEGIN
	ALTER TABLE [dbo].[PositionBudgets] ADD  CONSTRAINT [DF_PositionBudgets_BurdenPercent]  DEFAULT ((0.0)) FOR [BurdenPercent]
END 
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[DF_PositionBudgets_FTE]'))
BEGIN
	ALTER TABLE [dbo].[PositionBudgets] ADD  CONSTRAINT [DF_PositionBudgets_FTE]  DEFAULT ((0.0)) FOR [FTE]
END 
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBudgets_Employees]'))
BEGIN
	ALTER TABLE [dbo].[PositionBudgets]  WITH CHECK ADD  CONSTRAINT [FK_PositionBudgets_Employees] FOREIGN KEY([EmployeeID])
	REFERENCES [dbo].[Employees] ([EmployeeId])
END 
GO



IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('[FK_PositionBudgets_Positions]'))
BEGIN
	ALTER TABLE [dbo].[PositionBudgets]  WITH CHECK ADD  CONSTRAINT [FK_PositionBudgets_Positions] FOREIGN KEY([PositionID])
	REFERENCES [dbo].[Positions] ([PositionId])
END 
GO
