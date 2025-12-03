CREATE TABLE [dbo].[employeeSalaryComponents] (
    [id]                    INT           IDENTITY (1, 1) NOT NULL,
    [employeeID]            INT           NOT NULL,
    [SalaryComponentID]     INT      NOT NULL,
    [salaryComponentTypeID] INT      NULL,
    [payTypeID]             INT      NULL,
    [payFrequencyCode]      VARCHAR (10)  NULL,
    [PayFrequencyId]        INT      NOT NULL,
    [amount]                MONEY         NULL,
    [linkToPayroll]         BIT           CONSTRAINT [DF_eSalaryComponents_linkToPayroll] DEFAULT ((0)) NULL,
    [startDate]             DATETIME NULL,
    [expirationDate]        DATETIME NULL,
    [Base]                  BIT           NULL,
    [benefits]              BIT           NULL,
    [total]                 BIT           NULL,
    [enteredBy]             VARCHAR (50)  NOT NULL,
    [enteredDate]           DATETIME NOT NULL,
    CONSTRAINT [PK_eSalaryComponents] PRIMARY KEY CLUSTERED ([id] ASC)
);

