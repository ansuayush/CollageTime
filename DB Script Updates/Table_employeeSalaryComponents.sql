IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('employeeSalaryComponents'))
BEGIN
CREATE TABLE [dbo].[employeeSalaryComponents](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[employeeID] [int] NOT NULL,
	[SalaryComponentID] [smallint] NOT NULL,
	[salaryComponentTypeID] [smallint] NULL,
	[payTypeID] [smallint] NULL,
	[payFrequencyCode] [varchar](10) NULL,
	[PayFrequencyId] [smallint] NOT NULL,
	[amount] [money] NULL,
	[linkToPayroll] [bit] NULL,
	[startDate] [smalldatetime] NULL,
	[expirationDate] [smalldatetime] NULL,
	[Base] [bit] NULL,
	[benefits] [bit] NULL,
	[total] [bit] NULL,
	[enteredBy] [varchar](50) NOT NULL,
	[enteredDate] [smalldatetime] NOT NULL,
 CONSTRAINT [PK_eSalaryComponents] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END
GO