

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('UserNamesPersons'))
BEGIN

CREATE TABLE [dbo].[UserNamesPersons](
	[UserName] [nvarchar](256) NOT NULL,
	[PersonID] [int] NOT NULL,
	[CreationDate] [datetime] NULL,
	[ModifiedDate] [datetime] NULL,
	[EnteredBy] [varchar](50) NULL,
 CONSTRAINT [PK_UserNamesPersons] PRIMARY KEY CLUSTERED 
(
	[UserName] ASC,
	[PersonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO 

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_UserNamesPersons_Persons'))
BEGIN

ALTER TABLE [dbo].[UserNamesPersons]  WITH CHECK ADD  CONSTRAINT [FK_UserNamesPersons_Persons] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Persons] ([PersonID])

ALTER TABLE [dbo].[UserNamesPersons] CHECK CONSTRAINT [FK_UserNamesPersons_Persons]

END

GO