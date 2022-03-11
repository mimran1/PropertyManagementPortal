select PropertyId, v.ID, v.Year,v.Month,v.Room,v.Amount, v.Description, m.MonthNumber,r.ShortName from RentInformation v, MonthNameNumber m, RentalProperties r where v.Month=m.Month 
and v.propertyid=r.id
order by v.year, m.MonthNumber