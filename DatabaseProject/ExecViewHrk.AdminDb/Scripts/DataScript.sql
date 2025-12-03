SET IDENTITY_INSERT [dbo].[ClientConfiguration] ON
INSERT INTO [dbo].[ClientConfiguration] ([ClientConfigId], [EmployerId], [ConfigurationName], [ConfigurationValue]) VALUES (1, 3, N'Enable College Time Contracts', 1)
INSERT INTO [dbo].[ClientConfiguration] ([ClientConfigId], [EmployerId], [ConfigurationName], [ConfigurationValue]) VALUES (2, 3, N'Fiscal Month', 10)
SET IDENTITY_INSERT [dbo].[ClientConfiguration] OFF