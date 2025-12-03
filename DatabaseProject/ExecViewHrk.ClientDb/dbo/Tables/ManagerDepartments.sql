CREATE TABLE [dbo].[ManagerDepartments] (
    [ManagerDepartmentId] INT      IDENTITY (1, 1) NOT NULL,
    [ManagerId]           INT      NOT NULL,
    [DepartmentId]        INT NOT NULL,
    CONSTRAINT [PK_ManagerDepartments] PRIMARY KEY CLUSTERED ([ManagerDepartmentId] ASC),
    CONSTRAINT [FK_ManagerDepartments_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([DepartmentId]),
    CONSTRAINT [FK_ManagerDepartments_Managers] FOREIGN KEY ([ManagerId]) REFERENCES [dbo].[Managers] ([ManagerId]),
    CONSTRAINT [uq_ManagerDepartments] UNIQUE NONCLUSTERED ([ManagerId] ASC, [DepartmentId] ASC)
);

