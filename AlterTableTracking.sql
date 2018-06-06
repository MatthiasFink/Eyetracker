USE [Eyetracker]
GO

/****** Object:  Table [dbo].[Tracking]    Script Date: 06.06.2018 20:24:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
DROP TABLE [dbo].[Tracking]
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


