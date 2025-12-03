CREATE TABLE [dbo].[ddlPositionTypes] (
    [PositionTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [description]    VARCHAR (50) NOT NULL,
    [code]           VARCHAR (10) NOT NULL,
    [active]         BIT          CONSTRAINT [DF_vPositionTypes_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPositionTypes] PRIMARY KEY CLUSTERED ([PositionTypeId] ASC)
);

