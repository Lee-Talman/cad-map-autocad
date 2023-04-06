USE [CADDB]
GO

/****** Object:  Table [dbo].[Groups]    Script Date: 4/6/2023 6:56:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Groups](
	[id] [bigint] IDENTITY(1,1) NOT NULL,
	[GroupName] [varchar](50) NOT NULL,
	[Alpha] [varchar](50) NULL,
	[InsPtX] [decimal](18, 8) NOT NULL,
	[InsPtY] [decimal](18, 8) NOT NULL,
	[ExtX] [decimal](18, 8) NULL,
	[ExtY] [decimal](18, 8) NULL,
	[Rotation] [decimal](18, 0) NULL,
	[Other] [nvarchar](max) NULL,
	[Created] [datetime] NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

