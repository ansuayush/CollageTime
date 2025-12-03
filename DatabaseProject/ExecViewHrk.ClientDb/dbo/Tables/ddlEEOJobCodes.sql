CREATE TABLE [dbo].[ddlEEOJobCodes] (
    [eeoJobCodeID]    SMALLINT     IDENTITY (1, 1) NOT NULL,
    [description]     VARCHAR (50) NOT NULL,
    [code]            VARCHAR (10) NOT NULL,
    [eeoFileStatusID] TINYINT      NOT NULL,
    [Active]          BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_vhEEOJobCodes] PRIMARY KEY CLUSTERED ([eeoJobCodeID] ASC)
);

