CREATE TABLE [dbo].[PersonTests] (
    [PersonTestId]     INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]         INT           NOT NULL,
    [EvaluationTestId] INT      NULL,
    [TestDate]         SMALLDATETIME NULL,
    [Score]            VARCHAR (10)  NULL,
    [Grade]            VARCHAR (10)  NULL,
    [Administrator]    VARCHAR (50)  NULL,
    [Notes]            TEXT          NULL,
    [EnteredBy]        VARCHAR (50)  NOT NULL,
    [EnteredDate]      SMALLDATETIME NOT NULL,
    CONSTRAINT [PK_pTests] PRIMARY KEY CLUSTERED ([PersonTestId] ASC),
    CONSTRAINT [FK_PersonTests_DdlEvaluationTests] FOREIGN KEY ([EvaluationTestId]) REFERENCES [dbo].[DdlEvaluationTests] ([EvaluationTestId]),
    CONSTRAINT [FK_PersonTests_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

