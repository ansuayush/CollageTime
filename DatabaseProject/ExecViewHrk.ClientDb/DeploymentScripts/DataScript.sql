
SET IDENTITY_INSERT [DdlEmploymentStatuses] ON

MERGE INTO [DdlEmploymentStatuses] AS Target
USING (VALUES
  (1,N'Active',N'A',1,1)
 ,(2,N'Terminated',N'T',1,1)
 ,(22,N'New Employee',N'N',1,1)
 ,(23,N'Separated',N'S',1,1)
 ,(24,N'On Leave',N'L',1,1)
 ,(25,N'On Long Term Disability',N'D',1,1)
 ,(26,N'No Longer Employed',N'G',1,1)
 ,(28,N'Retired',N'R',1,1)
 ,(29,N'Part Time',N'P',1,1)
 ,(31,N'Surviving Spouse',N'F',1,1)
 ,(32,N'On International Assignment',N'B',1,1)
 ,(33,N'Casual',N'C',1,1)
 ,(34,N'Sick Leave',N'E',1,1)
 ,(35,N'Blocked',N'H',1,1)
 ,(36,N'Inactive',N'I',1,1)
 ,(37,N'Transferred',N'J',1,1)
 ,(38,N'On Sabbatical',N'K',1,1)
 ,(39,N'Multiple Positions',N'M',1,1)
 ,(40,N'Lay Off',N'O',1,1)
 ,(41,N'Deaceased',N'Q',1,1)
) AS Source ([EmploymentStatusId],[Description],[Code],[Active],[IsDefault])
ON (Target.[EmploymentStatusId] = Source.[EmploymentStatusId])
WHEN MATCHED AND (
	NULLIF(Source.[Description], Target.[Description]) IS NOT NULL OR NULLIF(Target.[Description], Source.[Description]) IS NOT NULL OR 
	NULLIF(Source.[Code], Target.[Code]) IS NOT NULL OR NULLIF(Target.[Code], Source.[Code]) IS NOT NULL OR 
	NULLIF(Source.[Active], Target.[Active]) IS NOT NULL OR NULLIF(Target.[Active], Source.[Active]) IS NOT NULL OR 
	NULLIF(Source.[IsDefault], Target.[IsDefault]) IS NOT NULL OR NULLIF(Target.[IsDefault], Source.[IsDefault]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = Source.[Description], 
  [Code] = Source.[Code], 
  [Active] = Source.[Active], 
  [IsDefault] = Source.[IsDefault]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([EmploymentStatusId],[Description],[Code],[Active],[IsDefault])
 VALUES(Source.[EmploymentStatusId],Source.[Description],Source.[Code],Source.[Active],Source.[IsDefault])
;
GO

SET IDENTITY_INSERT [DdlEmploymentStatuses] OFF
GO
