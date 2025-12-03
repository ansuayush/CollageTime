CREATE TABLE [dbo].[PersonSkills] (
    [PersonSkillId]  INT           IDENTITY (1, 1) NOT NULL,
    [PersonId]       INT           NOT NULL,
    [SkillId]        INT           NOT NULL,
    [SkillLevelId]   INT      NULL,
    [AttainedDate]   DATETIME NULL,
    [ExpirationDate] DATETIME NULL,
    [LastUsedDate]   DATETIME NULL,
    [EffectiveDate]  DATETIME NULL,
    [Notes]          TEXT          NULL,
    [EnteredBy]      VARCHAR (50)  NOT NULL,
    [EnteredDate]    DATETIME NOT NULL,
    CONSTRAINT [PK_pSkills] PRIMARY KEY CLUSTERED ([PersonSkillId] ASC),
    CONSTRAINT [FK_PersonSkills_DdlSkillLevels] FOREIGN KEY ([SkillLevelId]) REFERENCES [dbo].[DdlSkillLevels] ([SkillLevelId]),
    CONSTRAINT [FK_PersonSkills_DdlSkills] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[DdlSkills] ([SkillId]),
    CONSTRAINT [FK_PersonSkills_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);

