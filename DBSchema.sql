USE [Eyetracker]
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'duration'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'image_mime'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'image'
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'filepath'
GO
ALTER TABLE [dbo].[Tracking] DROP CONSTRAINT [FK_Tracking_Test]
GO
ALTER TABLE [dbo].[Tracking] DROP CONSTRAINT [FK_Tracking_Slide]
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [FK_Test_Test_Definition]
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [FK_Test_Candidate]
GO
ALTER TABLE [dbo].[Slide_Choice] DROP CONSTRAINT [FK_Slide_Choice_Slide]
GO
ALTER TABLE [dbo].[Slide_Answer] DROP CONSTRAINT [FK_Slide_Answer_Test]
GO
ALTER TABLE [dbo].[Slide_Answer] DROP CONSTRAINT [FK_Slide_Answer_Slide_Choice]
GO
ALTER TABLE [dbo].[Slide_Answer] DROP CONSTRAINT [FK_Slide_Answer_Slide]
GO
ALTER TABLE [dbo].[Slide] DROP CONSTRAINT [FK_Slide_Test_Definition]
GO
ALTER TABLE [dbo].[Questionnaire] DROP CONSTRAINT [FK_Questionnaire_Questionnaire]
GO
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [FK_Question_Questionnaire]
GO
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [FK_Choice_Question]
GO
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [FK_Answer_Test]
GO
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [FK_Answer_Question]
GO
ALTER TABLE [dbo].[Answer] DROP CONSTRAINT [FK_Answer_Choice]
GO
ALTER TABLE [dbo].[Test_Definition] DROP CONSTRAINT [DF_Test_Definition_EyeTrackerStep]
GO
ALTER TABLE [dbo].[Test_Definition] DROP CONSTRAINT [DF_Test_Definition_language]
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF_Test_last_step]
GO
ALTER TABLE [dbo].[Test] DROP CONSTRAINT [DF_Test_status]
GO
ALTER TABLE [dbo].[Slide_Choice] DROP CONSTRAINT [DF_Slide_Choice_num]
GO
ALTER TABLE [dbo].[Slide] DROP CONSTRAINT [DF_Slide_is_multiple_choice]
GO
ALTER TABLE [dbo].[Slide] DROP CONSTRAINT [DF_Slide_num]
GO
ALTER TABLE [dbo].[Questionnaire] DROP CONSTRAINT [DF_Questionnaire_Step]
GO
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [DF_Question_num]
GO
ALTER TABLE [dbo].[Question] DROP CONSTRAINT [DF_Question_type_cd]
GO
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [DF_Choice_is_correct]
GO
ALTER TABLE [dbo].[Choice] DROP CONSTRAINT [DF_Choice_num]
GO
ALTER TABLE [dbo].[Candidate] DROP CONSTRAINT [DF_Candidate_age_range_high]
GO
ALTER TABLE [dbo].[Candidate] DROP CONSTRAINT [DF_Table_1_age_range]
GO
ALTER TABLE [dbo].[Candidate] DROP CONSTRAINT [DF_Candidate_gender]
GO
/****** Object:  Index [IX_Slide_Choice_2]    Script Date: 06.06.2018 20:30:06 ******/
DROP INDEX [IX_Slide_Choice_2] ON [dbo].[Slide_Choice]
GO
/****** Object:  Index [IX_Slide_Choice_1]    Script Date: 06.06.2018 20:30:06 ******/
DROP INDEX [IX_Slide_Choice_1] ON [dbo].[Slide_Choice]
GO
/****** Object:  Table [dbo].[Tracking]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Tracking]
GO
/****** Object:  Table [dbo].[Test_Definition]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Test_Definition]
GO
/****** Object:  Table [dbo].[Test]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Test]
GO
/****** Object:  Table [dbo].[Slide_Choice]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Slide_Choice]
GO
/****** Object:  Table [dbo].[Slide_Answer]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Slide_Answer]
GO
/****** Object:  Table [dbo].[Slide]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Slide]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Settings]
GO
/****** Object:  Table [dbo].[Questionnaire]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Questionnaire]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Question]
GO
/****** Object:  Table [dbo].[Choice]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Choice]
GO
/****** Object:  Table [dbo].[Candidate]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Candidate]
GO
/****** Object:  Table [dbo].[Answer]    Script Date: 06.06.2018 20:30:06 ******/
DROP TABLE [dbo].[Answer]
GO
/****** Object:  Table [dbo].[Answer]    Script Date: 06.06.2018 20:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Answer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_id] [int] NOT NULL,
	[question_id] [int] NOT NULL,
	[answer] [nvarchar](200) NULL,
	[choice_id] [int] NULL,
	[is_correct] [bit] NULL,
	[answered] [datetime] NOT NULL,
 CONSTRAINT [PK_Answer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Candidate]    Script Date: 06.06.2018 20:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Candidate](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[personal_code] [nvarchar](50) NULL,
	[gender] [nvarchar](1) NOT NULL,
	[age_range_low] [int] NOT NULL,
	[age_range_high] [int] NOT NULL,
 CONSTRAINT [PK_Candidate] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Choice]    Script Date: 06.06.2018 20:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Choice](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[question_id] [int] NOT NULL,
	[num] [int] NOT NULL,
	[shortcut] [nvarchar](1) NULL,
	[choice] [nvarchar](150) NOT NULL,
	[is_correct] [bit] NULL,
 CONSTRAINT [PK_Choice] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Question]    Script Date: 06.06.2018 20:30:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Question](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[questionnaire_id] [int] NOT NULL,
	[type_cd] [nchar](1) NOT NULL,
	[num] [numeric](4, 1) NOT NULL,
	[question] [nvarchar](250) NOT NULL,
	[correct_answer] [nvarchar](80) NULL,
 CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Questionnaire]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Questionnaire](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Test_Definition_Id] [int] NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[Help] [nvarchar](1000) NULL,
	[Step] [int] NOT NULL,
 CONSTRAINT [PK_Questionnaire] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[value] [nvarchar](200) NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Slide]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Slide](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_definition_id] [int] NOT NULL,
	[num] [int] NOT NULL,
	[title] [nvarchar](80) NULL,
	[filepath] [nvarchar](250) NULL,
	[image] [varbinary](max) NULL,
	[image_mime] [nvarchar](50) NULL,
	[duration] [int] NULL,
	[is_multiple_choice] [bit] NOT NULL,
	[question] [nvarchar](250) NULL,
 CONSTRAINT [PK_Slide] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Slide_Answer]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Slide_Answer](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_id] [int] NOT NULL,
	[slide_id] [int] NOT NULL,
	[slide_choice_id] [int] NOT NULL,
	[slide_start_time] [datetime] NULL,
	[slide_end_time] [datetime] NULL,
	[answered] [datetime] NULL,
 CONSTRAINT [PK_Slide_Answer] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Slide_Choice]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Slide_Choice](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[slide_id] [int] NOT NULL,
	[shortcut] [nvarchar](1) NOT NULL,
	[num] [int] NOT NULL,
	[choice] [nvarchar](150) NULL,
 CONSTRAINT [PK_Slide_Choice] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_definition_id] [int] NOT NULL,
	[candidate_id] [int] NOT NULL,
	[start_time] [datetime] NULL,
	[end_time] [datetime] NULL,
	[test_group] [nvarchar](50) NULL,
	[comment] [nvarchar](max) NULL,
	[status_cd] [nvarchar](3) NOT NULL,
	[LastStep] [int] NOT NULL,
 CONSTRAINT [PK_Test] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Test_Definition]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Test_Definition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[language] [nchar](2) NOT NULL,
	[Title] [nvarchar](50) NOT NULL,
	[Version] [smallint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[EyeTrackerStep] [int] NOT NULL,
 CONSTRAINT [PK_Table] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tracking]    Script Date: 06.06.2018 20:30:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tracking](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[test_id] [int] NOT NULL,
	[slide_id] [int] NOT NULL,
	[occurred] [datetime] NOT NULL,
	[x] [numeric](9, 2) NOT NULL,
	[y] [numeric](9, 2) NOT NULL,
	[dia_x] [numeric](6, 2) NULL,
	[dia_y] [numeric](6, 2) NULL,
	[cr_x] [numeric](9, 2) NULL,
	[cr_y] [numeric](9, 2) NULL,
	[por_x] [numeric](9, 2) NULL,
	[por_y] [numeric](9, 2) NULL,
	[timing] [int] NULL,
	[trigger] [int] NULL,
 CONSTRAINT [PK_Tracking] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Slide_Choice_1]    Script Date: 06.06.2018 20:30:07 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Slide_Choice_1] ON [dbo].[Slide_Choice]
(
	[slide_id] ASC,
	[shortcut] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Slide_Choice_2]    Script Date: 06.06.2018 20:30:07 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Slide_Choice_2] ON [dbo].[Slide_Choice]
(
	[slide_id] ASC,
	[num] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Candidate] ADD  CONSTRAINT [DF_Candidate_gender]  DEFAULT (N'K') FOR [gender]
GO
ALTER TABLE [dbo].[Candidate] ADD  CONSTRAINT [DF_Table_1_age_range]  DEFAULT ((0)) FOR [age_range_low]
GO
ALTER TABLE [dbo].[Candidate] ADD  CONSTRAINT [DF_Candidate_age_range_high]  DEFAULT ((24)) FOR [age_range_high]
GO
ALTER TABLE [dbo].[Choice] ADD  CONSTRAINT [DF_Choice_num]  DEFAULT ((1)) FOR [num]
GO
ALTER TABLE [dbo].[Choice] ADD  CONSTRAINT [DF_Choice_is_correct]  DEFAULT ((0)) FOR [is_correct]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_type_cd]  DEFAULT (N'T') FOR [type_cd]
GO
ALTER TABLE [dbo].[Question] ADD  CONSTRAINT [DF_Question_num]  DEFAULT ((1.0)) FOR [num]
GO
ALTER TABLE [dbo].[Questionnaire] ADD  CONSTRAINT [DF_Questionnaire_Step]  DEFAULT ((1)) FOR [Step]
GO
ALTER TABLE [dbo].[Slide] ADD  CONSTRAINT [DF_Slide_num]  DEFAULT ((1)) FOR [num]
GO
ALTER TABLE [dbo].[Slide] ADD  CONSTRAINT [DF_Slide_is_multiple_choice]  DEFAULT ((0)) FOR [is_multiple_choice]
GO
ALTER TABLE [dbo].[Slide_Choice] ADD  CONSTRAINT [DF_Slide_Choice_num]  DEFAULT ((1)) FOR [num]
GO
ALTER TABLE [dbo].[Test] ADD  CONSTRAINT [DF_Test_status]  DEFAULT (N'NEW') FOR [status_cd]
GO
ALTER TABLE [dbo].[Test] ADD  CONSTRAINT [DF_Test_last_step]  DEFAULT ((0)) FOR [LastStep]
GO
ALTER TABLE [dbo].[Test_Definition] ADD  CONSTRAINT [DF_Test_Definition_language]  DEFAULT (N'de') FOR [language]
GO
ALTER TABLE [dbo].[Test_Definition] ADD  CONSTRAINT [DF_Test_Definition_EyeTrackerStep]  DEFAULT ((2)) FOR [EyeTrackerStep]
GO
ALTER TABLE [dbo].[Answer]  WITH CHECK ADD  CONSTRAINT [FK_Answer_Choice] FOREIGN KEY([choice_id])
REFERENCES [dbo].[Choice] ([id])
GO
ALTER TABLE [dbo].[Answer] CHECK CONSTRAINT [FK_Answer_Choice]
GO
ALTER TABLE [dbo].[Answer]  WITH CHECK ADD  CONSTRAINT [FK_Answer_Question] FOREIGN KEY([question_id])
REFERENCES [dbo].[Question] ([Id])
GO
ALTER TABLE [dbo].[Answer] CHECK CONSTRAINT [FK_Answer_Question]
GO
ALTER TABLE [dbo].[Answer]  WITH CHECK ADD  CONSTRAINT [FK_Answer_Test] FOREIGN KEY([test_id])
REFERENCES [dbo].[Test] ([id])
GO
ALTER TABLE [dbo].[Answer] CHECK CONSTRAINT [FK_Answer_Test]
GO
ALTER TABLE [dbo].[Choice]  WITH CHECK ADD  CONSTRAINT [FK_Choice_Question] FOREIGN KEY([question_id])
REFERENCES [dbo].[Question] ([Id])
GO
ALTER TABLE [dbo].[Choice] CHECK CONSTRAINT [FK_Choice_Question]
GO
ALTER TABLE [dbo].[Question]  WITH CHECK ADD  CONSTRAINT [FK_Question_Questionnaire] FOREIGN KEY([questionnaire_id])
REFERENCES [dbo].[Questionnaire] ([Id])
GO
ALTER TABLE [dbo].[Question] CHECK CONSTRAINT [FK_Question_Questionnaire]
GO
ALTER TABLE [dbo].[Questionnaire]  WITH CHECK ADD  CONSTRAINT [FK_Questionnaire_Questionnaire] FOREIGN KEY([Test_Definition_Id])
REFERENCES [dbo].[Test_Definition] ([Id])
GO
ALTER TABLE [dbo].[Questionnaire] CHECK CONSTRAINT [FK_Questionnaire_Questionnaire]
GO
ALTER TABLE [dbo].[Slide]  WITH CHECK ADD  CONSTRAINT [FK_Slide_Test_Definition] FOREIGN KEY([test_definition_id])
REFERENCES [dbo].[Test_Definition] ([Id])
GO
ALTER TABLE [dbo].[Slide] CHECK CONSTRAINT [FK_Slide_Test_Definition]
GO
ALTER TABLE [dbo].[Slide_Answer]  WITH CHECK ADD  CONSTRAINT [FK_Slide_Answer_Slide] FOREIGN KEY([slide_id])
REFERENCES [dbo].[Slide] ([id])
GO
ALTER TABLE [dbo].[Slide_Answer] CHECK CONSTRAINT [FK_Slide_Answer_Slide]
GO
ALTER TABLE [dbo].[Slide_Answer]  WITH CHECK ADD  CONSTRAINT [FK_Slide_Answer_Slide_Choice] FOREIGN KEY([slide_choice_id])
REFERENCES [dbo].[Slide_Choice] ([id])
GO
ALTER TABLE [dbo].[Slide_Answer] CHECK CONSTRAINT [FK_Slide_Answer_Slide_Choice]
GO
ALTER TABLE [dbo].[Slide_Answer]  WITH CHECK ADD  CONSTRAINT [FK_Slide_Answer_Test] FOREIGN KEY([test_id])
REFERENCES [dbo].[Test] ([id])
GO
ALTER TABLE [dbo].[Slide_Answer] CHECK CONSTRAINT [FK_Slide_Answer_Test]
GO
ALTER TABLE [dbo].[Slide_Choice]  WITH CHECK ADD  CONSTRAINT [FK_Slide_Choice_Slide] FOREIGN KEY([slide_id])
REFERENCES [dbo].[Slide] ([id])
GO
ALTER TABLE [dbo].[Slide_Choice] CHECK CONSTRAINT [FK_Slide_Choice_Slide]
GO
ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_Test_Candidate] FOREIGN KEY([candidate_id])
REFERENCES [dbo].[Candidate] ([id])
GO
ALTER TABLE [dbo].[Test] CHECK CONSTRAINT [FK_Test_Candidate]
GO
ALTER TABLE [dbo].[Test]  WITH CHECK ADD  CONSTRAINT [FK_Test_Test_Definition] FOREIGN KEY([test_definition_id])
REFERENCES [dbo].[Test_Definition] ([Id])
GO
ALTER TABLE [dbo].[Test] CHECK CONSTRAINT [FK_Test_Test_Definition]
GO
ALTER TABLE [dbo].[Tracking]  WITH CHECK ADD  CONSTRAINT [FK_Tracking_Slide] FOREIGN KEY([slide_id])
REFERENCES [dbo].[Slide] ([id])
GO
ALTER TABLE [dbo].[Tracking] CHECK CONSTRAINT [FK_Tracking_Slide]
GO
ALTER TABLE [dbo].[Tracking]  WITH CHECK ADD  CONSTRAINT [FK_Tracking_Test] FOREIGN KEY([test_id])
REFERENCES [dbo].[Test] ([id])
GO
ALTER TABLE [dbo].[Tracking] CHECK CONSTRAINT [FK_Tracking_Test]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pfad zur Bilddatei, nur verwendet, falls kein Bild in image geladen wurde' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'filepath'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Direkt in die DB geladenes Bild, alternativ zu Angabe Filepath' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'image'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Falls Bild in image geladen wurde, sollte hier der mime-Type stehen, z.B. image/png' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'image_mime'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Maximale Anzeigedauer' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Slide', @level2type=N'COLUMN',@level2name=N'duration'
GO
