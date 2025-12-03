CREATE TABLE [dbo].[DdlSkills] (
    [SkillId]     INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [SkillTypeId] INT     NULL,
    [Notes]       TEXT         NULL,
    [Active]      BIT          CONSTRAINT [DF_DdlSkills_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSkills] PRIMARY KEY CLUSTERED ([SkillId] ASC),
    CONSTRAINT [FK_DdlSkills_DdlSkillTypes] FOREIGN KEY ([SkillTypeId]) REFERENCES [dbo].[DdlSkillTypes] ([SkillTypeId])
);

