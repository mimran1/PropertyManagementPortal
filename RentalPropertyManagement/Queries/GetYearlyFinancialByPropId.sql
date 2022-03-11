select p.ShortName, xx.year,sum(xx.Expenditure) Expenditure, sum(xx.Income) Income, sum(xx.Net) Net from
(
select PropertyId, c.Year,c.Month, Expenditure,Income, Income-Expenditure As Net
from
(
select a.PropertyId , isnull(a.year,b.Year) as Year,isnull(a.month,b.month) As Month, expenditure,Income
from
(
select PropertyId, Year,Month, isnull([Property Manager],0)+isnull([Labor],0)+isnull([Material],0)+isnull([Capital Improvements],0)+isnull([Utilities],0)+isnull([Taxes],0)+isnull([Insurance],0)+isnull([Misc],0) As Expenditure from vwPropertyExpenses
) a full outer join
(
select PropertyId, Year, Month, sum(Amount) As Income from RentInformation group by year,month, PropertyId
) b 
on a.year=b.year and a.month = b.month and a.propertyId=b.PropertyId
) c, monthNameNumber m
where c.month=m.month
and c.Year =@Year
and PropertyId=@PropertyId
) xx, RentalProperties p  where xx.PropertyId=p.ID group by xx.year,p.ShortName