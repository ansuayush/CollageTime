CREATE TABLE [dbo].[DdlEmployeeTypes] (
    [EmployeeTypeId] INT     IDENTITY (1, 1) NOT NULL,
    [Description]    VARCHAR (50) NOT NULL,
    [Code]           VARCHAR (10) NOT NULL,
    [Active]         BIT          CONSTRAINT [DF_DdlEmployeeTypes_Active] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_prsEmployeeTypes] PRIMARY KEY CLUSTERED ([EmployeeTypeId] ASC)
);

