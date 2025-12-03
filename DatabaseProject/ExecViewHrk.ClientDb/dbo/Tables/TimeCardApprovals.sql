CREATE TABLE [dbo].[TimeCardApprovals] (
    [TimeCardApprovalId] INT IDENTITY (1, 1) NOT NULL,
    [EmployeeId]         INT NOT NULL,
    [PayPeriodId]        INT NOT NULL,
    [Approved]           BIT CONSTRAINT [DF_vTimeCard_approved] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_vTimeCardApprovals] PRIMARY KEY CLUSTERED ([TimeCardApprovalId] ASC),
    CONSTRAINT [FK_TimeCardApprovals_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]),
    CONSTRAINT [FK_TimeCardApprovals_PayPeriods] FOREIGN KEY ([PayPeriodId]) REFERENCES [dbo].[PayPeriods] ([PayPeriodId]),
    CONSTRAINT [uq_TimeCardApprovals] UNIQUE NONCLUSTERED ([EmployeeId] ASC, [PayPeriodId] ASC)
);

