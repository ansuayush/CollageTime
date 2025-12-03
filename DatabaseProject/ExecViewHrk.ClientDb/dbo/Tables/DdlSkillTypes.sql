CREATE TABLE [dbo].[DdlSkillTypes] (
    [SkillTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          CONSTRAINT [DF_vSkillTypes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vSkillTypes] PRIMARY KEY CLUSTERED ([SkillTypeId] ASC)
);

