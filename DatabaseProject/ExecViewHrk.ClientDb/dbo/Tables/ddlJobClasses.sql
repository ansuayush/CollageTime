CREATE TABLE [dbo].[ddlJobClasses] (
    [jobClassID]  SMALLINT     IDENTITY (1, 1) NOT NULL,
    [description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [active]      BIT          CONSTRAINT [DF_vJobClasses_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vJobClasses] PRIMARY KEY CLUSTERED ([jobClassID] ASC)
);

