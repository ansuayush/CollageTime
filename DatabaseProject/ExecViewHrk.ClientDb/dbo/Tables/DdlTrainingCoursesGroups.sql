CREATE TABLE [dbo].[DdlTrainingCoursesGroups] (
    [TrainingCourseGroupId] INT          IDENTITY (1, 1) NOT NULL,
    [GroupName]             VARCHAR (50) NULL,
    CONSTRAINT [PK_ddlTrainingCoursesGroups] PRIMARY KEY CLUSTERED ([TrainingCourseGroupId] ASC),
    CONSTRAINT [IX_ddlTrainingCoursesGroups] UNIQUE NONCLUSTERED ([GroupName] ASC)
);

