USE [RentalPropertyManagement]
GO
/****** Object:  Table [dbo].[GeneralLedger]    Script Date: 9/25/2020 10:44:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RentInformation2](
	[ID] INT IDENTITY(1,1) Primary Key,
	[Year] int null,
	[Month] [varchar](50) NULL,
	[Room] [varchar](50) NULL,
	[Amount] [money] NULL,
	[LastUpdated] [datetime] NOT NULL
) ON [PRIMARY]
GO