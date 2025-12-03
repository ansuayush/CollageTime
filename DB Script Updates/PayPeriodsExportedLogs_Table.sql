USE [DUBlankDB_ForNewImport318New]
GO

/****** Object:  Table [dbo].[PayPeriodsExportedLogs]    Script Date: 7/18/2018 6:15:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PayPeriodsExportedLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExportedLogId] [int] NOT NULL,
	[CoCode] [varchar](10) NULL,
	[BatchID] [varchar](10) NULL,
	[FileNumber] [varchar](10) NULL,
	[PayNumber] [varchar](10) NULL,
	[RegHours] [varchar](10) NULL,
	[OTHours] [varchar](10) NULL,
	[Hours3Code] [varchar](10) NULL,
	[Hours3Amount] [varchar](10) NULL,
	[Hours4Code] [varchar](10) NULL,
	[Hours4Amount] [varchar](10) NULL,
	[RegEarnings] [varchar](10) NULL,
	[OTEarnings] [varchar](10) NULL,
	[Earnings3Code] [varchar](20) NULL,
	[Earnings3Amount] [varchar](10) NULL,
	[Earnings4Code] [varchar](20) NULL,
	[Earnings4Amount] [varchar](10) NULL,
	[Earnings5Code] [varchar](10) NULL,
	[Earnings5Amount] [varchar](10) NULL,
	[TempRate] [varchar](20) NULL,
	[MemoCode1] [varchar](10) NULL,
	[MemoAmount1] [varchar](20) NULL,
	[MemoCode2] [varchar](10) NULL,
	[MemoAmount2] [varchar](20) NULL,
	[CancelPay] [varchar](10) NULL,
	[CostNumber] [varchar](50) NULL,
	[Department] [varchar](10) NULL,
	[PayGroupId] [varchar](20) NULL,
	[StartDate] [varchar](10) NULL,
	[EndDate] [varchar](10) NULL,
	[EmployeeType] [varchar](10) NULL,
	[SalaryHistoryRate] [varchar](20) NULL,
 CONSTRAINT [PK_PayPeriodsExportedLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


