USE [RentalPropertyManagement]
GO

/****** Object:  View [dbo].[vwPropertyExpenses]    Script Date: 9/28/2020 8:48:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create view [dbo].[vwPropertyExpenses]
As 
select * 
from
(
-- Eg.g: 2020-11-30 18:00:00.000 DATEPART returns the month number of dateofExpense so thats 11 in this case
-- DateAdd function adds month, the number of months to add is 11 and the date to add from is 1900-01-01 (-1 defaults to this date). Returns DateTime so in our case that evaluates to 1900-11-01
-- DateName gets month name of 1900-11-01. Returns month name so thats November
select DATEPART(YEAR, DateOfExpense) As Year, DateName( month , DateAdd( month , DATEPART(month, DateOfExpense) , -1 ) ) As Month, Amount, Category from generalLedger
) as myUp
pivot
(
	sum(Amount) for
	[category] in ([Property Manager],[Labor],[Material],[Capital Improvements],[Utilities],[Taxes],[Insurance],[Misc])
) as p
GO


