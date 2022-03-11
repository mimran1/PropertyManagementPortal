select x.Year, x.PropertyId, p.ShortName, sum(x.Net) As NetIncome from
(
select PropertyId, c.Year,c.Month, Expenditure,Income, Income-Expenditure As Net
from
(
select COALESCE(a.PropertyId,b.propertyId) as PropertyId , isnull(a.year,b.Year) as Year,isnull(a.month,b.month) As Month, COALESCE(expenditure,0) As expenditure,COALESCE(Income,0) As Income
from
(
select PropertyId, Year,Month, isnull([Property Management],0)+isnull([Labor],0)+isnull([Material],0)+isnull([Capital Improvements],0)+isnull([Utilities],0)+isnull([Taxes],0)+isnull([Insurance],0)+isnull([Misc],0) As Expenditure from vwPropertyExpenses
) a full outer join
(
select PropertyId, Year, Month, sum(Amount) As Income from RentInformation group by year,month, PropertyId
) b 
on a.year=b.year and a.month = b.month and a.propertyId=b.PropertyId
) c, monthNameNumber m
where c.month=m.month
) x, RentalProperties p where x.PropertyId=p.id group by x.year, x.PropertyId, p.ShortName