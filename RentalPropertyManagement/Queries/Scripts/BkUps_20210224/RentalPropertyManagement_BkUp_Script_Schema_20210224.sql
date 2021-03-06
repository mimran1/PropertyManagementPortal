USE [master]
GO
/****** Object:  Database [RentalPropertyManagement]    Script Date: 2/24/2021 5:28:36 PM ******/
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
/****** Object:  Table [dbo].[GeneralLedger]    Script Date: 2/24/2021 5:28:36 PM ******/
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
	[PropertyId] [int] NOT NULL,
 CONSTRAINT [PK__GeneralL__3214EC27DFA77B57] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vwPropertyExpenses]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[vwPropertyExpenses]
As 
select * 
from
(
select DATEPART(YEAR, DateOfExpense) As Year, DateName( month , DateAdd( month , DATEPART(month, DateOfExpense) , -1 ) ) As Month, Amount, Category, PropertyId from generalLedger
) as myUp
pivot
(
	sum(Amount) for
	[category] in ([Property Manager],[Labor],[Material],[Capital Improvements],[Utilities],[Taxes],[Insurance],[Misc])
) as p
GO
/****** Object:  View [dbo].[vwPropertyExpensesByCategory]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [dbo].[vwPropertyExpensesByCategory]
As 
select PropertyId, DATEPART(YEAR, DateOfExpense) AS Year, 
DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)) As Month, Category, sum(Amount) As Amount from GeneralLedger 
group by DATEPART(YEAR, DateOfExpense), DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)), Category, PropertyId
GO
/****** Object:  Table [dbo].[RentInformation]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RentInformation](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [varchar](50) NULL,
	[Room] [varchar](50) NULL,
	[Amount] [money] NULL,
	[Description] [nvarchar](500) NULL,
	[LastUpdated] [datetime] NOT NULL,
	[PropertyId] [int] NOT NULL,
 CONSTRAINT [PK__RentInfo__3214EC27829DA78E] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vwRentInformationByRoom]    Script Date: 2/24/2021 5:28:36 PM ******/
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
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](450) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[Email] [nvarchar](256) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MonthNameNumber]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MonthNameNumber](
	[Month] [varchar](20) NULL,
	[MonthNumber] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RentalProperties]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RentalProperties](
	[ID] [int] NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[ShortName] [nchar](10) NOT NULL,
	[DateOfPurchase] [datetime] NOT NULL,
	[PurchasePrice] [money] NOT NULL,
	[NumBeds] [smallint] NULL,
	[NumBaths] [smallint] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [RoleNameIndex]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [EmailIndex]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UserNameIndex]    Script Date: 2/24/2021 5:28:36 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[GeneralLedger] ADD  CONSTRAINT [myConstraint]  DEFAULT ((1)) FOR [PropertyId]
GO
ALTER TABLE [dbo].[RentInformation] ADD  CONSTRAINT [myConstraint2]  DEFAULT ((1)) FOR [PropertyId]
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserTokens]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserTokens] CHECK CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
GO
/****** Object:  StoredProcedure [dbo].[spDeleteGeneralLedger]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create proc [dbo].[spDeleteGeneralLedger]
@ID int
As
Begin
	delete from GeneralLedger where ID=@ID
End
GO
/****** Object:  StoredProcedure [dbo].[spDeleteRentInformation]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create proc [dbo].[spDeleteRentInformation]
@ID int
As
Begin
	delete from RentInformation where ID=@ID
End
GO
/****** Object:  StoredProcedure [dbo].[spGetAllLedgerItems]    Script Date: 2/24/2021 5:28:36 PM ******/
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
/****** Object:  StoredProcedure [dbo].[spGetAllPropertyExpenses]    Script Date: 2/24/2021 5:28:36 PM ******/
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
/****** Object:  StoredProcedure [dbo].[spGetAllRentInformation]    Script Date: 2/24/2021 5:28:36 PM ******/
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
/****** Object:  StoredProcedure [dbo].[spInsertIntoGeneralLedger]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spInsertIntoGeneralLedger]
@PropertyId int,
@Vendor varchar(50),
@DateOfExpense datetime,
@Category varchar(50),
@Amount money,
@Description varchar(500)
As
Begin
	insert into GeneralLedger ([PropertyId], [Vendor], [DateOfExpense], [Category], [Amount], [Description], [LastUpdated]) Values (@PropertyId, @Vendor, @DateOfExpense, @Category, @Amount, @Description, CURRENT_TIMESTAMP)
End
GO
/****** Object:  StoredProcedure [dbo].[spInsertIntoRentInformation]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[spInsertIntoRentInformation]
@PropertyId int,
@Year int,
@Month varchar(50),
@Room varchar(50),
@Amount money,
@Description varchar(500)
As
Begin
	insert into RentInformation ([PropertyId], [Year],[Month],[Room], [Amount], [Description], [LastUpdated]) Values (@PropertyId, @Year,@Month,@Room, @Amount, @Description, CURRENT_TIMESTAMP)
End
GO
/****** Object:  StoredProcedure [dbo].[spUpdateGeneralLedger]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE proc [dbo].[spUpdateGeneralLedger]
@ID int,
@Vendor varchar(50),
@DateOfExpense datetime,
@Category varchar(50),
@Amount money,
@Description varchar(500)
As
Begin
	update GeneralLedger set Vendor=@Vendor, DateOfExpense=@DateOfExpense, Category=@Category, Amount=@Amount, Description=@Description, LastUpdated=CURRENT_TIMESTAMP where id=@ID
End
GO
/****** Object:  StoredProcedure [dbo].[spUpdateRentInformation]    Script Date: 2/24/2021 5:28:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE proc [dbo].[spUpdateRentInformation]
@ID int,
@Year int,
@Month varchar(50),
@Room varchar(50),
@Amount money,
@Description varchar(500)
As
Begin
	update RentInformation set Year=@Year, Month=@Month, Room=@Room,Amount=@Amount, Description=@Description, LastUpdated=CURRENT_TIMESTAMP where ID=@ID
End
GO
USE [master]
GO
ALTER DATABASE [RentalPropertyManagement] SET  READ_WRITE 
GO
