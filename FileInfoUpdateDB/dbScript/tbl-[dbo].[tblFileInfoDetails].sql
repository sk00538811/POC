USE [ApplicationController]
GO

/****** Object:  Table [dbo].[tblFileInfoDetails]    Script Date: 7/4/2018 10:28:44 AM ******/
DROP TABLE [dbo].[tblFileInfoDetails]
GO

/****** Object:  Table [dbo].[tblFileInfoDetails]    Script Date: 7/4/2018 10:28:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblFileInfoDetails](
	[FileInfoDetailsId] [bigint] IDENTITY(1,1) NOT NULL,
	[FullFilePath] [varchar](2000) NULL,
	[FileName] [varchar](500) NULL,
	[FileExtension] [varchar](255) NULL,
	[FileSize] [bigint] NULL,
	[FileLastModifiedDate] [datetime] NULL,
	[InUse] [bit] NULL CONSTRAINT [DF_tblFileInfoDetails_InUse]  DEFAULT ((0))
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

