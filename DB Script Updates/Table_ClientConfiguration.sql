IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ClientConfiguration'))
BEGIN
CREATE TABLE [dbo].[ClientConfiguration](
	[ClientConfigId] [int] IDENTITY(1,1) NOT NULL,
	[EmployerId] [int] NOT NULL,
	[ConfigurationName] [varchar](100) NULL,
	[ConfigurationValue] [int] NULL,
 CONSTRAINT [PK_ClientConfiguration] PRIMARY KEY CLUSTERED 
(
	[ClientConfigId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


END
GO


IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_ClientConfiguration_Employers'))
BEGIN

ALTER TABLE [dbo].[ClientConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_ClientConfiguration_Employers] FOREIGN KEY([EmployerId])
REFERENCES [dbo].[Employers] ([EmployerId])


ALTER TABLE [dbo].[ClientConfiguration] CHECK CONSTRAINT [FK_ClientConfiguration_Employers]

END

GO

SET IDENTITY_INSERT [dbo].[ClientConfiguration] ON 
INSERT [dbo].[ClientConfiguration] ([ClientConfigId], [EmployerId], [ConfigurationName], [ConfigurationValue]) VALUES (1, 3, N'EnableCollege Time Contracts', 1)
SET IDENTITY_INSERT [dbo].[ClientConfiguration] OFF