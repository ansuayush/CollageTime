CREATE TABLE [dbo].[EmployeeAllocations] (
    [EmployeeAllocationId] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]           INT            NOT NULL,
    [DepartmentId]         INT       NOT NULL,
    [AllocationPercent]    DECIMAL (5, 2) NOT NULL,
    CONSTRAINT [PK_EmployeeAllocations] PRIMARY KEY CLUSTERED ([EmployeeAllocationId] ASC),
    CONSTRAINT [FK_EmployeeAllocations_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([DepartmentId]),
    CONSTRAINT [FK_EmployeeAllocations_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([EmployeeId]),
    CONSTRAINT [uq_EmployeeAllocations] UNIQUE NONCLUSTERED ([EmployeeId] ASC, [DepartmentId] ASC)
);

