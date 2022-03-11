select PropertyId, Year, b.Month, [Room1A], [Room1B],[Room1C], [Room1D],[Room2A], [Room2B],[Room2C], [Room2D], [Room1A] + [Room1B] + [Room1C] + [Room1D] + [Room2A] + [Room2B] + [Room2C] + [Room2D] As Total from
(
select PropertyId, Year, Month, COALESCE([Room1A],0) As Room1A, COALESCE([Room1B],0) As Room1B, COALESCE([Room1C],0) As Room1C, COALESCE([Room1D],0) As Room1D, COALESCE([Room2A],0) As Room2A, COALESCE([Room2B],0) As Room2B,COALESCE([Room2C],0) As Room2C, COALESCE([Room2D],0) As Room2D
from 
(select PropertyId, year,month,room,amount from RentInformation) as a
pivot 
(
	sum(amount)
	for room in ([Room1A], [Room1B],[Room1C], [Room1D],[Room2A], [Room2B],[Room2C], [Room2D])
) as pivott
) b
inner join MonthNameNumber c on b.Month = c.Month
order by b.year, c.MonthNumber asc