CREATE TABLE [dbo].[PositionSalaryGrades] (
    [id]            INT      IDENTITY (1, 1) NOT NULL,
    [PositionId]    INT      NOT NULL,
    [salaryGradeID] INT      NOT NULL,
    [enteredBy]     VARCHAR (50)  NOT NULL,
    [enteredDate]   SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_posPositionGrades] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Positions_ddlSalaryGrades] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Positions] ([PositionId]),
    CONSTRAINT [FK_positionSalaryGrades_ddlSalaryGrades] FOREIGN KEY ([salaryGradeID]) REFERENCES [dbo].[DdlSalaryGrades] ([SalaryGradeID])
);

