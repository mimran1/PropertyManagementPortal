select PropertyId, v.Year,v.Month,v.Category,v.Amount, p.ShortName from vwPropertyExpensesByCategory v, MonthNameNumber m, RentalProperties p where v.Month=m.Month and v.propertyid=p.id 
and PropertyId=@PropertyId
order by m.MonthNumber, v.Category 