CREATE TABLE [dbo].[PersonImages] (
    [PersonImageId]       INT             IDENTITY (1, 1) NOT NULL,
    [PersonId]            INT             NOT NULL,
    [PersonImageData]     VARBINARY (MAX) NOT NULL,
    [PersonImageMimeType] VARCHAR (50)    NOT NULL,
    CONSTRAINT [PK_PersonImages] PRIMARY KEY CLUSTERED ([PersonImageId] ASC),
    CONSTRAINT [FK_PersonImages_Persons] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Persons] ([PersonId])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PersonImages]
    ON [dbo].[PersonImages]([PersonId] ASC);

