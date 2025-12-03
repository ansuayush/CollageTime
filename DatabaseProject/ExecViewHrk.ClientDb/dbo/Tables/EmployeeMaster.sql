CREATE TABLE [dbo].[EmployeeMaster] (
    [ID]                           INT           IDENTITY (1, 1) PRIMARY KEY NOT NULL,
    [LastName]                     VARCHAR (500) NULL,
    [FirstName]                    VARCHAR (500) NULL,
    [Birthdate_PersonalDta]        VARCHAR (500) NULL,
    [HireDate]                     VARCHAR (500) NULL,
    [TerminationDate]              VARCHAR (500) NULL,
    [PayGroup_JobDta]              VARCHAR (500) NULL,
    [AutolinkFileNumber_JobDta]    VARCHAR (500) NULL,
    [EmployeeStatus_JobDta]        VARCHAR (500) NULL,
    [CompensationRate_JobDta]      VARCHAR (500) NULL,
    [EffectiveDate_JobDta]         VARCHAR (500) NULL,
    [CompensationFrequency_JobDta] VARCHAR (500) NULL,
    [StandardHours_JobDta]         VARCHAR (500) NULL,
    [PayFrequency_PayGroupInfo]    VARCHAR (500) NULL,
    [LocationCode_JobDta]          VARCHAR (500) NULL,
    [JobCode_JobDta]               VARCHAR (500) NULL,
    [Description_JobCodeInfo]      VARCHAR (500) NULL
);

