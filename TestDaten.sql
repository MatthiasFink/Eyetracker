USE [Eyetracker]
GO
DELETE FROM [dbo].[Settings]
GO
DELETE FROM [dbo].[Tracking]
GO
DELETE FROM [dbo].[Slide_Answer]
GO
DELETE FROM [dbo].[Slide_Choice]
GO
DELETE FROM [dbo].[Slide]
GO
DELETE FROM [dbo].[Answer]
GO
DELETE FROM [dbo].[Choice]
GO
DELETE FROM [dbo].[Question]
GO
DELETE FROM [dbo].[Questionnaire]
GO
DELETE FROM [dbo].[Test]
GO
DELETE FROM [dbo].[Test_Definition]
GO
DELETE FROM [dbo].[Candidate]
GO
SET IDENTITY_INSERT [dbo].[Candidate] ON 

INSERT [dbo].[Candidate] ([id], [personal_code], [gender], [age_range_low], [age_range_high]) VALUES (1, N'xyz123', N'M', 0, 25)
INSERT [dbo].[Candidate] ([id], [personal_code], [gender], [age_range_low], [age_range_high]) VALUES (2, N'6K4N4I0N14', N'M', 46, 55)
SET IDENTITY_INSERT [dbo].[Candidate] OFF
SET IDENTITY_INSERT [dbo].[Test_Definition] ON 

INSERT [dbo].[Test_Definition] ([Id], [language], [Title], [Version], [Description]) VALUES (1, N'de', N'Verständlichkeit von Prozessdarstellungen', 1, N'Test bestehend aus demogtafischem Fragebogen, Eyetracker Experiment zu Prozessdarstellungen und nachgelagertem Fragebogen')
INSERT [dbo].[Test_Definition] ([Id], [language], [Title], [Version], [Description]) VALUES (2, N'en', N'Understanding of Process Charts', 1, NULL)
SET IDENTITY_INSERT [dbo].[Test_Definition] OFF
SET IDENTITY_INSERT [dbo].[Test] ON 

INSERT [dbo].[Test] ([id], [test_definition_id], [candidate_id], [start_time], [end_time], [test_group], [comment], [status_cd]) VALUES (1, 1, 1, CAST(N'2018-05-01T13:30:00.000' AS DateTime), NULL, NULL, NULL, N'NEW')
INSERT [dbo].[Test] ([id], [test_definition_id], [candidate_id], [start_time], [end_time], [test_group], [comment], [status_cd]) VALUES (2, 1, 2, NULL, NULL, NULL, NULL, N'NEW')
INSERT [dbo].[Test] ([id], [test_definition_id], [candidate_id], [start_time], [end_time], [test_group], [comment], [status_cd]) VALUES (3, 1, 2, NULL, NULL, NULL, NULL, N'NEW')
SET IDENTITY_INSERT [dbo].[Test] OFF
SET IDENTITY_INSERT [dbo].[Questionnaire] ON 

INSERT [dbo].[Questionnaire] ([Id], [Test_Definition_Id], [Title], [Description], [Help]) VALUES (1, 1, N'Demografischer Fragebogen ', N'Erhebung allgemeiner statistischer Daten zum Kandidaten', NULL)
INSERT [dbo].[Questionnaire] ([Id], [Test_Definition_Id], [Title], [Description], [Help]) VALUES (2, 1, N'Post Questionnaire', NULL, NULL)
SET IDENTITY_INSERT [dbo].[Questionnaire] OFF
SET IDENTITY_INSERT [dbo].[Question] ON 

INSERT [dbo].[Question] ([Id], [questionnaire_id], [type_cd], [num], [question], [correct_answer]) VALUES (1, 1, N'O', CAST(0.1 AS Numeric(2, 1)), N'Allgemeiner Hinweis, nicht zu beantworten', NULL)
INSERT [dbo].[Question] ([Id], [questionnaire_id], [type_cd], [num], [question], [correct_answer]) VALUES (2, 1, N'T', CAST(1.0 AS Numeric(2, 1)), N'Abfrage mit Text-Antwort (richtige Antwort: 42', N'42')
INSERT [dbo].[Question] ([Id], [questionnaire_id], [type_cd], [num], [question], [correct_answer]) VALUES (3, 1, N'C', CAST(2.0 AS Numeric(2, 1)), N'Abfrage mit Auswahl', NULL)
INSERT [dbo].[Question] ([Id], [questionnaire_id], [type_cd], [num], [question], [correct_answer]) VALUES (4, 1, N'Y', CAST(3.0 AS Numeric(2, 1)), N'Abfrage Ja / Nein', NULL)
SET IDENTITY_INSERT [dbo].[Question] OFF
SET IDENTITY_INSERT [dbo].[Choice] ON 

INSERT [dbo].[Choice] ([id], [question_id], [num], [shortcut], [choice], [is_correct]) VALUES (1, 3, 1, N'1', N'Auswahl 1', NULL)
INSERT [dbo].[Choice] ([id], [question_id], [num], [shortcut], [choice], [is_correct]) VALUES (2, 3, 2, N'2', N'Auswahl 2', NULL)
INSERT [dbo].[Choice] ([id], [question_id], [num], [shortcut], [choice], [is_correct]) VALUES (3, 3, 3, N'3', N'Auswahl 3', NULL)
SET IDENTITY_INSERT [dbo].[Choice] OFF
