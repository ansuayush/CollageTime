CREATE TABLE [dbo].[ddlBusinessLevelTypes] (
    [BusinessLevelTypeNbr] TINYINT      IDENTITY (1, 1) NOT NULL,
    [description]          VARCHAR (50) NOT NULL,
    [code]                 VARCHAR (10) NOT NULL,
    [active]               BIT          CONSTRAINT [DF_ddlBusinessLevelTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vBusinessLevelTypes] PRIMARY KEY CLUSTERED ([BusinessLevelTypeNbr] ASC)
);

