CREATE TABLE [dbo].[DdlSalaryGradeHistories] (
    [ID]             INT           IDENTITY (1, 1) NOT NULL,
    [salaryGradeID]  INT      NOT NULL,
    [validFrom]      DATETIME NOT NULL,
    [salaryMinimum]  MONEY         NOT NULL,
    [salaryMidpoint] MONEY         NOT NULL,
    [salaryMaximum]  MONEY         NOT NULL,
    CONSTRAINT [PK_ddlSalaryGradeHistory] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ddlSalaryGradeHistory_ddlSalaryGrades] FOREIGN KEY ([salaryGradeID]) REFERENCES [dbo].[DdlSalaryGrades] ([SalaryGradeID]),
    CONSTRAINT [IX_ddlSalaryGradeHistory] UNIQUE NONCLUSTERED ([salaryGradeID] ASC, [validFrom] ASC)
);

