CREATE TABLE [dbo].[ManagerLockouts] (
    [ManagerLockoutsId] INT            IDENTITY (1, 1) NOT NULL,
    [PayPeriodId]       INT            NOT NULL,
    [ManagerUserName]   NVARCHAR (256) NULL,
    CONSTRAINT [PK_ManagerLockouts] PRIMARY KEY CLUSTERED ([ManagerLockoutsId] ASC),
    CONSTRAINT [FK_ManagerLockouts_PayPeriods] FOREIGN KEY ([PayPeriodId]) REFERENCES [dbo].[PayPeriods] ([PayPeriodId])
);

