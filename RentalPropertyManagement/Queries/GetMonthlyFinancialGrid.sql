select Month1, aaa.Year, Income_47, Expenditure_47, Net_47, Income_198, Expenditure_198, Net_198 from
(
select M.Month as Month1, m.monthNumber, Income.Year, Income.month,COALESCE(expenditure,0) As expenditure_47,COALESCE(Income,0) As Income_47, COALESCE(Income-Expenditure,0) As Net_47
from
MonthNameNumber M
left join
(
select PropertyId, Year, Month, sum(Amount) As Income from RentInformation where propertyid=1 group by year,month, PropertyId
) Income on M.Month=Income.month
left join
(
select PropertyId, Year,Month, isnull([Property Management],0)+isnull([Labor],0)+isnull([Material],0)+isnull([Capital Improvements],0)+isnull([Utilities],0)+isnull([Taxes],0)+isnull([Insurance],0)+isnull([Misc],0) As Expenditure from vwPropertyExpenses where propertyid=1
) Expense on Income.Month=Expense.month
and Expense.year=Income.year and Expense.propertyId=Income.PropertyId
) AAA,
(
select M.Month As Month2, m.monthNumber, Income.Year, Income.month,COALESCE(expenditure,0) As expenditure_198,COALESCE(Income,0) As Income_198, COALESCE(Income-Expenditure,0) As Net_198
from
MonthNameNumber M
left join
(
select PropertyId, Year, Month, sum(Amount) As Income from RentInformation where propertyid=2 group by year,month, PropertyId
) Income on M.Month=Income.month
left join
(
select PropertyId, Year,Month, isnull([Property Management],0)+isnull([Labor],0)+isnull([Material],0)+isnull([Capital Improvements],0)+isnull([Utilities],0)+isnull([Taxes],0)+isnull([Insurance],0)+isnull([Misc],0) As Expenditure from vwPropertyExpenses where propertyid=2
) Expense on Income.Month=Expense.month
and Expense.year=Income.year and Expense.propertyId=Income.PropertyId
) BBB
WHERE AAA.Month1=BBB.Month2
--and aaa.year=2021
order by aaa.monthNumber asc