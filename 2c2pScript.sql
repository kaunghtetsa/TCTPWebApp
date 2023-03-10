USE [2C2PEServiceDB]
GO
/****** Object:  Table [dbo].[CallLogs]    Script Date: 12/30/2022 8:32:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CallLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserAgentName] [nvarchar](200) NULL,
	[IPAddress] [nvarchar](20) NULL,
	[FileType] [nvarchar](20) NULL,
	[TransactionCount] [int] NOT NULL,
	[IsSucceed] [bit] NOT NULL,
	[PushedOn] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
	[ServiceAccessRightID] [int] NOT NULL,
 CONSTRAINT [PK_CallLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceAccessRight]    Script Date: 12/30/2022 8:32:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceAccessRight](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AuthenticationKey] [nvarchar](200) NOT NULL,
	[PermissionLevel] [nchar](10) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_ServiceAccessRight] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionRecord]    Script Date: 12/30/2022 8:32:01 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionRecord](
	[TransactionID] [nvarchar](50) NOT NULL,
	[CallLogID] [int] NOT NULL,
	[Amount] [decimal](18, 0) NOT NULL,
	[CurrencyCode] [nchar](5) NOT NULL,
	[TransactionDateTime] [datetime] NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CallLogs]  WITH CHECK ADD  CONSTRAINT [FK_CallLogs_ServiceAccessRight] FOREIGN KEY([ServiceAccessRightID])
REFERENCES [dbo].[ServiceAccessRight] ([ID])
GO
ALTER TABLE [dbo].[CallLogs] CHECK CONSTRAINT [FK_CallLogs_ServiceAccessRight]
GO
ALTER TABLE [dbo].[TransactionRecord]  WITH CHECK ADD  CONSTRAINT [FK_Transactions_CallLogs] FOREIGN KEY([CallLogID])
REFERENCES [dbo].[CallLogs] ([ID])
GO
ALTER TABLE [dbo].[TransactionRecord] CHECK CONSTRAINT [FK_Transactions_CallLogs]
GO
