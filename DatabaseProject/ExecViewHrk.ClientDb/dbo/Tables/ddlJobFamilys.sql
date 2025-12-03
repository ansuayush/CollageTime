CREATE TABLE [dbo].[ddlJobFamilys] (
    [JobFamilyId] SMALLINT     IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [Active]      BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_ddlJobFamily] PRIMARY KEY CLUSTERED ([JobFamilyId] ASC)
);

