CREATE TABLE [dbo].[Departments] (
    [DepartmentId]          INT     IDENTITY (1, 1) NOT NULL,
    [CompanyCodeId]         INT     NOT NULL,
    [DepartmentDescription] VARCHAR (50) NOT NULL,
    [DepartmentCode]        VARCHAR (10) NOT NULL,
    [IsDepartmentActive]    BIT          CONSTRAINT [DF_vDepartments_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vDepartments] PRIMARY KEY CLUSTERED ([DepartmentId] ASC),
    CONSTRAINT [FK_Departments_CompanyCodes] FOREIGN KEY ([CompanyCodeId]) REFERENCES [dbo].[CompanyCodes] ([CompanyCodeId])
);

