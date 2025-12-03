
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('SalaryMatrix'))
BEGIN

CREATE TABLE SalaryMatrix(
	[SalYear] [int] NOT NULL,
	[Rating] [decimal](18, 2) NOT NULL,
	[RangeHigh] [decimal](18, 2) NOT NULL,
	[RangeLow] [decimal](18, 2) NOT NULL,
	[Min] [decimal](18, 2) NOT NULL,
	[Max] [decimal](18, 2) NOT NULL,
	[DisplayRow] [tinyint] NOT NULL,
	[DisplayCol] [tinyint] NOT NULL,
 CONSTRAINT [PK_salaryMatrix] PRIMARY KEY CLUSTERED 
(
	[SalYear] ASC,
	[Rating] ASC,
	[RangeHigh] ASC,
	[RangeLow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO