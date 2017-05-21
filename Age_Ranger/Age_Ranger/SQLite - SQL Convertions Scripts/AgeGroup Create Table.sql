USE [Database of your Choice]
GO

/****** Object:  Table [dbo].[AgeGroup]    Script Date: 21/05/2017 22:08:52 ******/
DROP TABLE [dbo].[AgeGroup]
GO

/****** Object:  Table [dbo].[AgeGroup]    Script Date: 21/05/2017 22:08:52 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AgeGroup](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[MinAge] [INT] NULL,
	[MaxAge] [INT] NULL,
	[Description] [TEXT] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


