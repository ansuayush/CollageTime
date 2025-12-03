CREATE TABLE [dbo].[ddlWorkersCompensations] (
    [workersCompensationID] SMALLINT     IDENTITY (1, 1) NOT NULL,
    [code]                  CHAR (10)    NULL,
    [description]           VARCHAR (50) NULL,
    [Active]                BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_ddlHRWorkersCompensation] PRIMARY KEY CLUSTERED ([workersCompensationID] ASC)
);

