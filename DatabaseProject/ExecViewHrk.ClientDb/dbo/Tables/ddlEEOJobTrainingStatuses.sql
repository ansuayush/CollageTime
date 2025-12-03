CREATE TABLE [dbo].[ddlEEOJobTrainingStatuses] (
    [eeoJobTrainingStatusID] SMALLINT     IDENTITY (1, 1) NOT NULL,
    [description]            VARCHAR (50) NOT NULL,
    [code]                   VARCHAR (10) NOT NULL,
    [Active]                 BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_vhEEOJobTrainingCodes] PRIMARY KEY CLUSTERED ([eeoJobTrainingStatusID] ASC)
);

