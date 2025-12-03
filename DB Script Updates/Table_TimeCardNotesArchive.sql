IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('TimeCardNotesArchive'))
BEGIN

CREATE TABLE [dbo].[TimeCardNotesArchive](
	[TimeCardNotesArchiveId] [int] NOT NULL,
	[CompanyCodeId] [int] NOT NULL,
	[FileNumber] [nvarchar](10) NOT NULL,
	[ActualDate] [smalldatetime] NOT NULL,
	[Notes] [nvarchar](500) NULL,
 CONSTRAINT [PK_TimeCardNotesArchive] PRIMARY KEY CLUSTERED 
(
	[TimeCardNotesArchiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

End

GO
IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_TimeCardNotesArchive_CompanyCodes'))
BEGIN
ALTER TABLE [dbo].[TimeCardNotesArchive]  WITH CHECK ADD  CONSTRAINT [FK_TimeCardNotesArchive_CompanyCodes] FOREIGN KEY([CompanyCodeId])
REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
ALTER TABLE [dbo].[TimeCardNotesArchive] CHECK CONSTRAINT [FK_TimeCardNotesArchive_CompanyCodes]
end
GO