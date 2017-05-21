USE [Database of your Choice]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 21/05/2017 22:10:06 ******/
DROP TABLE [dbo].[Person]
GO

/****** Object:  Table [dbo].[Person]    Script Date: 21/05/2017 22:10:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Person](
	[Id] [BIGINT] IDENTITY(1,1) NOT NULL,
	[FirstName] [TEXT] NULL,
	[LastName] [TEXT] NULL,
	[Age] [INT] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


