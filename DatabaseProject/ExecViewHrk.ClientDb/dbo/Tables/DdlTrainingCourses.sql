CREATE TABLE [dbo].[DdlTrainingCourses] (
    [TrainingCourseId] TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [Code]             VARCHAR (10) NOT NULL,
    [Active]           BIT          CONSTRAINT [DF_vTrainingCourses_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vTrainingCourses] PRIMARY KEY CLUSTERED ([TrainingCourseId] ASC)
);

