CREATE TABLE [dbo].[PersonExaminations] (
    [PersonExaminationId]      INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]                 INT           NOT NULL,
    [MedicalExaminationTypeId] INT      NOT NULL,
    [ExaminationDate]          DATETIME NULL,
    [Examiner]                 VARCHAR (50)  NULL,
    [NextScheduledExamination] DATETIME NULL,
    [Notes]                    TEXT          NULL,
    [EnteredBy]                VARCHAR (50)  NULL,
    [EnteredDate]              DATETIME NULL,
    [ModifiedBy]               VARCHAR (50)  NULL,
    [ModifiedDate]             DATETIME NULL,
    CONSTRAINT [PK_pExaminations] PRIMARY KEY CLUSTERED ([PersonExaminationId] ASC),
    CONSTRAINT [FK_PersonExaminations_DdlMedicalExaminationTypes] FOREIGN KEY ([MedicalExaminationTypeId]) REFERENCES [dbo].[DdlMedicalExaminationTypes] ([MedicalExaminationTypeId]),
    CONSTRAINT [FK_PersonExaminations_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonExaminations]
    ON [dbo].[PersonExaminations]([PersonId] ASC, [MedicalExaminationTypeId] ASC);

