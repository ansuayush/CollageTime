CREATE TABLE [dbo].[DdlEvaluationTests] (
    [EvaluationTestId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [Code]             VARCHAR (10) NOT NULL,
    [Active]           BIT          CONSTRAINT [DF_vEvaluationTests_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vEvaluationTests] PRIMARY KEY CLUSTERED ([EvaluationTestId] ASC)
);

