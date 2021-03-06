USE [master]
GO
/****** Object:  Database [RentalPropertyManagement]    Script Date: 10/29/2020 1:19:25 PM ******/
CREATE DATABASE [RentalPropertyManagement]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RentalPropertyManagement', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\RentalPropertyManagement.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'RentalPropertyManagement_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS\MSSQL\DATA\RentalPropertyManagement_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [RentalPropertyManagement] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RentalPropertyManagement].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RentalPropertyManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RentalPropertyManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RentalPropertyManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RentalPropertyManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RentalPropertyManagement] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RentalPropertyManagement] SET  MULTI_USER 
GO
ALTER DATABASE [RentalPropertyManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RentalPropertyManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RentalPropertyManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RentalPropertyManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RentalPropertyManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [RentalPropertyManagement] SET QUERY_STORE = OFF
GO
USE [RentalPropertyManagement]
GO
/****** Object:  Table [dbo].[GeneralLedger]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralLedger](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vendor] [varchar](50) NULL,
	[DateOfExpense] [datetime] NULL,
	[Category] [varchar](50) NULL,
	[Amount] [money] NULL,
	[Description] [varchar](500) NULL,
	[LastUpdated] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vwPropertyExpenses]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[vwPropertyExpenses]
As 
select * 
from
(
select DATEPART(YEAR, DateOfExpense) As Year, DateName( month , DateAdd( month , DATEPART(month, DateOfExpense) , -1 ) ) As Month, Amount, Category from generalLedger
) as myUp
pivot
(
	sum(Amount) for
	[category] in ([Property Manager],[Labor],[Material],[Capital Improvements],[Utilities],[Taxes],[Insurance],[Misc])
) as p
GO
/****** Object:  View [dbo].[vwPropertyExpensesByCategory]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[vwPropertyExpensesByCategory]
As 
select DATEPART(YEAR, DateOfExpense) AS Year, 
DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)) As Month, Category, sum(Amount) As Amount from GeneralLedger 
group by DATEPART(YEAR, DateOfExpense), DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)), Category
GO
/****** Object:  Table [dbo].[RentInformation]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RentInformation](
	[Year] [int] NULL,
	[Month] [varchar](20) NULL,
	[Room1A] [money] NULL,
	[Room1B] [money] NULL,
	[Room1C] [money] NULL,
	[Room1D] [money] NULL,
	[Room2A] [money] NULL,
	[Room2B] [money] NULL,
	[Room2C] [money] NULL,
	[Room2D] [money] NULL,
	[LastUpdated] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vwRentInformationByRoom]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[vwRentInformationByRoom]
As 
select u.Year, u.Month, u.Room,u.Amount
from RentInformation s
unpivot
(
  Amount
  for Room in (Room1A,Room1B,Room1C,Room1D,Room2A,Room2B,Room2C,Room2D)
) u;
GO
/****** Object:  Table [dbo].[MonthNameNumber]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthNameNumber](
	[Month] [varchar](20) NULL,
	[MonthNumber] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spGetAllLedgerItems]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure [dbo].[spGetAllLedgerItems]
As
Begin
	Select * from generalLedger
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAllPropertyExpenses]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spGetAllPropertyExpenses]
As
Begin
	select * from PropertyExpenses
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAllRentInformation]    Script Date: 10/29/2020 1:19:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spGetAllRentInformation]
As
Begin
	select * from RentInformation
End
GO
USE [master]
GO
ALTER DATABASE [RentalPropertyManagement] SET  READ_WRITE 
GO
