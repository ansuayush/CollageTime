CREATE TABLE [dbo].[TimeOffRequests] (
    [TimeOffRequestId] INT           IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]    INT      NOT NULL,
    [EmployeeId]       INT           NOT NULL,
    [TimeOffDate]      SMALLDATETIME NOT NULL,
    [RequestStatus]    TINYINT       CONSTRAINT [DF_vTimeOffRequests_Status] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_TimeOffRequests] PRIMARY KEY CLUSTERED ([TimeOffRequestId] ASC),
    CONSTRAINT [FK_TimeOffRequests_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId]),
    CONSTRAINT [FK_TimeOffRequests_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]),
    CONSTRAINT [uq_TimeOffRequests] UNIQUE NONCLUSTERED ([CompanyCodeId] ASC, [EmployeeId] ASC, [TimeOffDate] ASC)
);

