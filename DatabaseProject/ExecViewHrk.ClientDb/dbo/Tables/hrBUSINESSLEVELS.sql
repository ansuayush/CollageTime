CREATE TABLE [dbo].[hrBUSINESSLEVELS] (
    [id]                    INT           IDENTITY (1, 1) NOT NULL,
    [parentBusinessLevelID] INT           NOT NULL,
    [title]                 VARCHAR (50)  NOT NULL,
    [code]                  VARCHAR (50)  NULL,
    [EINID]                 SMALLINT      NULL,
    [eeoFileStatusID]       SMALLINT      NULL,
    [locationID]            SMALLINT      NULL,
    [businessLevelTypeID]   SMALLINT      NULL,
    [notes]                 TEXT          NULL,
    [scheduledHours]        REAL          NULL,
    [active]                BIT           CONSTRAINT [DF_hrBUSINESSLEVELS_active] DEFAULT ((1)) NULL,
    [enteredBy]             VARCHAR (50)  NULL,
    [enteredDate]           SMALLDATETIME NULL,
    [payFrequency]          VARCHAR (10)  NULL,
    [budgetReportingType]   VARCHAR (20)  CONSTRAINT [DF_hrBUSINESSLEVELS_budgetReporting] DEFAULT ('Gross only') NULL,
    CONSTRAINT [PK_hrBUSINESSLEVELS] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [IX_hrBUSINESSLEVELS_UniqueCode] UNIQUE NONCLUSTERED ([code] ASC)
);

