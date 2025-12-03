CREATE TABLE [dbo].[DdlEducationLevels] (
    [EducationLevelId] INT      IDENTITY (1, 1) NOT NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [Code]             VARCHAR (10) NOT NULL,
    [Active]           BIT          CONSTRAINT [DF_DdlEducationLevels_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vLevelAchieved] PRIMARY KEY CLUSTERED ([EducationLevelId] ASC)
);

