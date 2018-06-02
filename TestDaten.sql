USE [Eyetracker]
GO
SET IDENTITY_INSERT [dbo].[Candidate] ON 

INSERT [dbo].[Candidate] ([id], [personal_code], [gender], [age_range_low], [age_range_high]) VALUES (1, N'xyz123', N'M', 0, 25)
SET IDENTITY_INSERT [dbo].[Candidate] OFF
SET IDENTITY_INSERT [dbo].[Test_Definition] ON 

INSERT [dbo].[Test_Definition] ([Id], [language], [Title], [Version], [Description]) VALUES (1, N'de', N'Verständlichkeit von Prozessdarstellungen', 1, N'Test bestehend aus demogtafischem Fragebogen, Eyetracker Experiment zu Prozessdarstellungen und nachgelagertem Fragebogen')
INSERT [dbo].[Test_Definition] ([Id], [language], [Title], [Version], [Description]) VALUES (2, N'en', N'Understanding of Process Charts', 1, NULL)
SET IDENTITY_INSERT [dbo].[Test_Definition] OFF
SET IDENTITY_INSERT [dbo].[Test] ON 

INSERT [dbo].[Test] ([id], [test_definition_id], [candidate_id], [start_time], [end_time], [test_group], [comment], [status_cd]) VALUES (1, 1, 1, NULL, NULL, NULL, NULL, N'N')
SET IDENTITY_INSERT [dbo].[Test] OFF
