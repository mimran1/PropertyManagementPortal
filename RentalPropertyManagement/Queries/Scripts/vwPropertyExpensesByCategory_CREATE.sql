USE [RentalPropertyManagement]
GO

/****** Object:  View [dbo].[vwPropertyExpenses]    Script Date: 9/28/2020 8:48:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[vwPropertyExpensesByCategory]
As 
select DATEPART(YEAR, DateOfExpense) AS Year, 
DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)) As Month, Category, sum(Amount) As Amount from GeneralLedger 
group by DATEPART(YEAR, DateOfExpense), DateName(month, DateAdd(month, DATEPART(month, DateOfExpense), - 1)), Category
GO
