

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('ManagerLockouts'))
BEGIN

CREATE TABLE [dbo].[ManagerLockouts](
	[ManagerLockoutsId] [int] IDENTITY(1,1) NOT NULL,
	[PayPeriodId] [int] NOT NULL,
	[ManagerUserName] [nvarchar](256) NULL,
 CONSTRAINT [PK_ManagerLockouts] PRIMARY KEY CLUSTERED 
(
	[ManagerLockoutsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

GO 

IF NOT EXISTS (SELECT 1 FROM dbo.sysobjects WHERE id = OBJECT_ID('FK_ManagerLockouts_PayPeriods'))
BEGIN

ALTER TABLE [dbo].[ManagerLockouts]  WITH CHECK ADD  CONSTRAINT [FK_ManagerLockouts_PayPeriods] FOREIGN KEY([PayPeriodId])
REFERENCES [dbo].[PayPeriods] ([PayPeriodId])



ALTER TABLE [dbo].[ManagerLockouts] CHECK CONSTRAINT [FK_ManagerLockouts_PayPeriods]

END

GO