CREATE TABLE [dbo].[DdlPositionCategories] (
    [PositionCategoryID] INT          IDENTITY (1, 1) NOT NULL,
    [description]        VARCHAR (50) NOT NULL,
    [code]               VARCHAR (10) NOT NULL,
    [active]             BIT          CONSTRAINT [DF_vPositionCategories_active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_vPositionCategories] PRIMARY KEY CLUSTERED ([PositionCategoryID] ASC)
);

