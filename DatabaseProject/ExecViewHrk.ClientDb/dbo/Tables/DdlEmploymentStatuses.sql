CREATE TABLE [dbo].[DdlEmploymentStatuses] (
    [EmploymentStatusId] INT      IDENTITY (1, 1) NOT NULL,
    [Description]        VARCHAR (50) NULL,
    [Code]               VARCHAR (10) NOT NULL,
    [Active]             BIT          CONSTRAINT [DF_DdlEmploymentStatuses_Active] DEFAULT ((1)) NOT NULL,
    [IsDefault]          BIT          DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_vhEmploymentStatuses] PRIMARY KEY CLUSTERED ([EmploymentStatusId] ASC)
);

