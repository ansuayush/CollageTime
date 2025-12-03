
CREATE TABLE [dbo].[TimeCardsAudit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TimeCardId] [int] NULL,
	[OperationName] [varchar](20) NULL,
	[OperationDate] [datetime] NOT NULL,
	[UserId] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TimeCardsAudit] ADD  DEFAULT (getdate()) FOR [OperationDate]
GO


