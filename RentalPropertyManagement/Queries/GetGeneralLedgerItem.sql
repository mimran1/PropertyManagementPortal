SELECT Vendor, Year, Month, Category, Amount, Description, DateOfExpense, PropertyId,r.ShortName
  FROM [GeneralLedger] g, RentalProperties r
  where g.PropertyId=r.ID
  order by DateOfExpense desc