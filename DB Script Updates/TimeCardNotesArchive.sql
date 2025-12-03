
CREATE TABLE [dbo].[TimeCardNotesArchive](
	[TimeCardNotesArchiveId] [int] IDENTITY(1,1) NOT NULL,
	[CompanyCodeId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[FileNumber] [nvarchar](10) NOT NULL,
	[ActualDate] [smalldatetime] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[TimeCardId] [int],
PRIMARY KEY CLUSTERED 
(
	[TimeCardNotesArchiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TimeCardNotesArchive]  WITH CHECK ADD FOREIGN KEY([CompanyCodeId])
REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
GO

ALTER TABLE [dbo].[TimeCardNotesArchive]  WITH CHECK ADD FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employees] ([EmployeeId])
GO


