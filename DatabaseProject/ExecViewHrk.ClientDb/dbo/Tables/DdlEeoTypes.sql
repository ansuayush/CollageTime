CREATE TABLE [dbo].[DdlEeoTypes] (
    [EeoTypeId]   INT      IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NULL,
    [Code]        VARCHAR (10) NULL,
    [Active]      BIT          DEFAULT ((1)) NULL,
    CONSTRAINT [PK_vhEEOTypes] PRIMARY KEY CLUSTERED ([EeoTypeId] ASC),
    CONSTRAINT [FK_DdlEeoTypes_DdlEeoTypes] FOREIGN KEY ([EeoTypeId]) REFERENCES [dbo].[DdlEeoTypes] ([EeoTypeId])
);

