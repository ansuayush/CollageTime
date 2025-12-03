CREATE TABLE [dbo].[DdlSalaryGrades] (
    [SalaryGradeID] INT     IDENTITY (1, 1) NOT NULL,
    [description]   VARCHAR (50) NOT NULL,
    [code]          VARCHAR (10) NOT NULL,
    [active]        BIT          CONSTRAINT [DF_ddlSalaryGrades_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSalaryGrades] PRIMARY KEY CLUSTERED ([SalaryGradeID] ASC),
    CONSTRAINT [IX_ddlSalaryGrades] UNIQUE NONCLUSTERED ([code] ASC, [description] ASC)
);

