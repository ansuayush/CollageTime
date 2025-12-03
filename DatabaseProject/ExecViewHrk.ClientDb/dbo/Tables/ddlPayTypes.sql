CREATE TABLE [dbo].[ddlPayTypes] (
    [payTypeID]   INT     IDENTITY (1, 1) NOT NULL,
    [description] VARCHAR (50) NOT NULL,
    [code]        VARCHAR (10) NOT NULL,
    [active]      BIT          CONSTRAINT [DF_ddlPayTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vValueTypes] PRIMARY KEY CLUSTERED ([payTypeID] ASC)
);

