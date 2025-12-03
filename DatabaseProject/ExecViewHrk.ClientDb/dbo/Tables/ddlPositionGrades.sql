CREATE TABLE [dbo].[ddlPositionGrades] (
    [PositionGradeID] INT     IDENTITY (1, 1) NOT NULL,
    [description]     VARCHAR (50) NOT NULL,
    [code]            VARCHAR (10) NOT NULL,
    [active]          BIT          CONSTRAINT [DF_vPositionGrades_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPositionGrades] PRIMARY KEY CLUSTERED ([PositionGradeID] ASC)
);

