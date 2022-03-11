select PropertyId, v.ID, v.Year,v.Month,v.Room,v.Amount, v.Description, m.MonthNumber from RentInformation v, MonthNameNumber m where v.Month=m.Month 
and PropertyId=@PropertyId
order by v.year, m.MonthNumber