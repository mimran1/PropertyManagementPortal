select propertyId, Year,v.Month,[Property Management] As PropertyMangement, Labor, Material, [Capital Improvements] As CapitalImprovements, Utilities, Taxes, Insurance, Misc,
isnull([Property Management],0)+isnull([Labor],0)+isnull([Material],0)+isnull([Capital Improvements],0)+isnull([Utilities],0)+isnull([Taxes],0)+isnull([Insurance],0)+isnull([Misc],0) As Total
from vwPropertyExpenses v, MonthNameNumber m 
where v.Month=m.Month 
order by year, m.MonthNumber 