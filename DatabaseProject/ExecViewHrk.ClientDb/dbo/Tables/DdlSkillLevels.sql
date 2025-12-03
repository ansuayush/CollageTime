CREATE TABLE [dbo].[DdlSkillLevels] (
    [SkillLevelId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]  VARCHAR (50) NOT NULL,
    [Code]         VARCHAR (10) NOT NULL,
    [Active]       BIT          CONSTRAINT [DF_DdlSkillLevels_Active] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_vSkillLevels] PRIMARY KEY CLUSTERED ([SkillLevelId] ASC)
);

