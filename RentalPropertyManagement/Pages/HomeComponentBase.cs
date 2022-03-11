using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class HomeComponentBase : ComponentBase
    {
        public string Title = "47 Floral Ave.";
        
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public bool RetrivingData { get; set; } = true;
        public IEnumerable<int> YearList { get; set; }
        public IEnumerable<MonthlyFinancial> MonthlyFinancialsMaster { get; set; }
        public IEnumerable<MonthlyFinancial> MonthlyFinancials { get; set; }
        public Dictionary<int, int> TabNumToYearMapping { get; set; }

        
        public List<InfoCard> InfoCardList { get; set; }
        public int SelectedTab { get; set; }
        public List<string> InfoItemList { get; set; }
        public IEnumerable<ExpenseByYearMonth> Expenses { get; set; }
        public IEnumerable<RoomOccupancy> RoomOccupancies { get; set; }
        public IEnumerable<RoomOccupancy> RoomOccupanciesMaster { get; set; }
        public IEnumerable<ExpenseByYearMonth> ExpensesMaster { get; set; }
        
        public decimal OverAllNetIncome { get; set; }
        public decimal OverAllNetIncomeMonthly { get; set; }
        
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public bool PropertySet { get; set; } = false;
        public decimal AverageMonthlyExpense { get; set; }
        public string CurrentIncome { get; set; }
        public decimal AverageNetMonthlyIncome { get; set; }
        public int NumOfRoomsOccupied { get; set; }
        public IEnumerable<IncomeByRoom> IncomeByRooms { get; set; }
        public IEnumerable<PropertyTax> PropertyTaxes { get; set; }
        public decimal PropertyCityTax { get; set; }
        public decimal PropertySchoolTax { get; set; }
        public IEnumerable<ToDoItem> ToDoItemsNotDone { get; set; }
        public List<ToDoItem> ToDoItemsDone { get; set; }
        public String NewToDoItem { get; set; }
        public int AvgNetIncome47F { get; set; }
        public int AvgNetIncome198W { get; set; }
        public IEnumerable<ExpenseByCategory> ExpenseByCategoriesAll { get; set; }
        public IEnumerable<ExpenseByCategory> ExpenseByCategories { get; set; }
        public IEnumerable<MonthlyFinancial> OverAllIncomeByProperty { get; set; }
        public decimal OverAllIncomeAmount { get; set; }
        public IEnumerable<MonthlyFinancial> OverAllNetIncomeByProperty { get; set; }
        public decimal OverAllNetIncomeAmount { get; set; }
        public decimal OverAllExpenseAmount { get; set; }
        public IEnumerable<MonthlyFinancial> OverAllExpenseByProperty { get; set; }
        public DateTime Now { get; set; }
        public double LAT { get; set; }
        public double LNG { get; set; }
        public decimal OER { get; set; }
        [Inject]
        public IWebHostEnvironment ENV { get; set; }

        protected override void OnInitialized()
        {
            TabNumToYearMapping = new Dictionary<int, int>();
            //Infocardlist initialization must take place before rendering otherwise the info cards wont show once another tab is clicked
            InfoCardList = new List<InfoCard>();
            InfoItemList = new List<string>();
            InfoItemList.Add("Income");
            InfoItemList.Add("Expense");
            InfoItemList.Add("Net");
            SelectedTab = 0;
            Now = DateTime.Now;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                try
                {
                    Property = await SessionStorageService.GetItemAsync<Property>("Property");
                    //Set LastPage variable of the session to the current page. So when u change the property from the ChangePropertyComponent the LastPage variable will be used to navigate.
                    await SessionStorageService.SetItemAsync<string>("LastPage", NavigationManager.Uri);
                    //Set FirstVisit variable of the session to N. So when u change the propety from the ChangeProperComponent the FirstVisit will be N and the UI will display currently selected Property
                    await SessionStorageService.SetItemAsync<string>("FirstVisit", "N");
                    if (Property == null)
                    {
                        Property = PropertyService.Property; //use defualt (all properties) in the event the session storage has nothing
                    }
                    PropertySet = true;
                    StateHasChanged(); //Soon as the property is set change the Markup to display the Property Name on top. Then go ahead with Data fetch from DB.
                    RetrivingData = true;

                    YearList = DBService.GetDistinctYear(Property.ID).ToList();
                    TabNumToYearMapping.Add(0, -1);
                    int tabNum = 1;
                    foreach (var item in YearList)
                    {
                        TabNumToYearMapping.Add(tabNum, item);
                        tabNum++;
                    }
                    SelectedTab = 0;

                    MonthlyFinancialsMaster = (await DBService.GetMonthlyFinancialAll()).ToList();


                    Log.Information("Done MonthlyFinancialsMaster");

                    ExpenseByCategoriesAll = (await DBService.GetExpenseByCategory()).ToList();

                    if (Property.ID == -1)
                    {
                        OverAllIncomeByProperty = MonthlyFinancialsMaster.GroupBy(x => x.ShortName).Select(c => new MonthlyFinancial { ShortName = c.Key, Income = c.Sum(y => y.Income) });
                        OverAllIncomeAmount = OverAllIncomeByProperty.Select(c => c.Income).Sum();
                        ExpenseByCategories = ExpenseByCategoriesAll.GroupBy(x => x.Category).Select(s => new ExpenseByCategory { Category = s.Key, Amount = s.Sum(x => x.Amount) });
                        OverAllExpenseAmount = ExpenseByCategories.Sum(x => x.Amount);
                        OverAllNetIncomeByProperty = MonthlyFinancialsMaster.GroupBy(x => x.ShortName).Select(c => new MonthlyFinancial { ShortName = c.Key, Net = c.Sum(y => y.Net) });
                        OverAllNetIncomeAmount = OverAllNetIncomeByProperty.Select(c => c.Net).Sum();
                        int NumOfMonths = ((Now.Year - Constants.FirstRentStartDate.Year) * 12) + Now.Month - Constants.FirstRentStartDate.Month;
                        AverageNetMonthlyIncome = OverAllNetIncomeAmount / NumOfMonths;
                        Properties = DBService.GetAllProperties().ToList();
                        OverAllNetIncome = OverAllNetIncomeAmount;
                        CalculateAvgNetIncomeByProperty();

                    }


                    if (Property.ID != -1)
                    {
                        
                        OverAllIncomeByProperty = MonthlyFinancialsMaster.Where(w => w.PropertyId == Property.ID).GroupBy(g => g.ShortName).Select(s => new MonthlyFinancial { ShortName = s.Key, Income = s.Sum(sum => sum.Income) });
                        OverAllIncomeAmount = OverAllIncomeByProperty.Select(c => c.Income).Sum();
                       
                        OverAllNetIncomeByProperty = MonthlyFinancialsMaster.Where(w => w.PropertyId == Property.ID).GroupBy(g => g.ShortName).Select(s => new MonthlyFinancial { ShortName = s.Key, Net = s.Sum(sum => sum.Net) });
                        OverAllNetIncomeAmount = OverAllNetIncomeByProperty.Select(c => c.Net).Sum();
                        
                        ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.PropertyId == Property.ID).GroupBy(x => x.Category).Select(s => new ExpenseByCategory { Category = s.Key, Amount = s.Sum(x => x.Amount) });
                        OverAllExpenseAmount = ExpenseByCategories.Sum(x => x.Amount);
                        RoomOccupanciesMaster = (await DBService.GetRoomOccupany()).ToList();
                        ExpensesMaster = (await DBService.GetExpense()).ToList();
                        
                        IncomeByRooms = (await DBService.GetIncomeByRoom()).Where(w => w.PropertyId == Property.ID && w.Year == Now.Year && w.MonthNumber == Now.Month);
                        NumOfRoomsOccupied = IncomeByRooms.Count();
                        PropertyTaxes = (await DBService.GetPropertyTaxInfo(Property.ID)).ToList();
                        PropertyCityTax = PropertyTaxes.Where(x => x.PropertyId == Property.ID && x.TaxType == "City" && x.Year == Now.Year).SingleOrDefault().Amount;
                        PropertySchoolTax = PropertyTaxes.Where(x => x.PropertyId == Property.ID && x.TaxType == "School" && x.Year == Now.Year).SingleOrDefault().Amount;
                        ToDoItemsNotDone = (await DBService.GetToDoItems(Property.ID, 0)).ToList();
                        ToDoItemsDone = (await DBService.GetToDoItems(Property.ID, 1)).ToList();
                        int NumOfMonths = ((Now.Year - Property.DateOfPurchase.Year) * 12) + Now.Month - Property.DateOfPurchase.Month;
                        //DefaultIfEmpty - is used here. If return of LINQ is null, then return a new MonthlyFinancial object whose Income is set to 0
                        //See: https://docs.microsoft.com/en-us/dotnet/api/system.linq.enumerable.firstordefault?view=net-6.0
                        CurrentIncome = String.Format("{0:0.##}", MonthlyFinancialsMaster.Where(x => x.PropertyId == Property.ID).OrderByDescending(x => x.Year).ThenByDescending(x => x.MonthNumber).DefaultIfEmpty(new MonthlyFinancial() { Income = 0 }).First().Income);
                        int avgNetIncome = (int)MonthlyFinancialsMaster.Where(x => x.PropertyId == Property.ID).Sum(x => x.Net) / NumOfMonths;
                        //not used should get rid of it
                        AverageNetMonthlyIncome = avgNetIncome;
                        int avgExpense = (int)MonthlyFinancialsMaster.Where(x => x.PropertyId == Property.ID).Sum(x => x.Expenditure) / NumOfMonths;
                        AverageMonthlyExpense = avgExpense;
                    }

                    SetInfoCards();
                    OER = (OverAllExpenseAmount / OverAllIncomeAmount)*100;

                    Log.Information("Done generateMonthlyExpenseChart_");
                    
                }
                catch (Exception e)
                {
                    Log.Error("Exception in OnAfterRenderAsync in HomeComponentBase.\n" + e.Message);
                }
                finally
                {
                    RetrivingData = false;
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("removeUnderline");
                    //Set Google Maps Coordinates
                    if (Property.ID != -1)
                    {
                        if (Property.ID == 1)
                        {
                            LAT = 42.10914364410624;
                            LNG = -75.94791438872407;
                        }
                        else if (Property.ID == 2)
                        {
                            LAT = 42.10951341171027;
                            LNG = -75.94589527550427;
                        }
                        else if (Property.ID == 3)
                        {
                            LAT = 42.093341326166886;
                            LNG = -75.93115220875633;
                        }

                        Log.Information("Done initMap");
                        await JSRuntime.InvokeVoidAsync("initMap", LAT, LNG, Property.ShortName);
                    }
                    
                    await JSRuntime.InvokeVoidAsync("topContributorPieChart", JsonConvert.SerializeObject(OverAllIncomeByProperty));
                    await JSRuntime.InvokeVoidAsync("generateMonthlyExpenseChart", JsonConvert.SerializeObject(ExpenseByCategories), true);
                }
            }
        }

        public void CalculateAvgNetIncomeByProperty()
        {
            // 1. Loop through all properties
            //2. Calculate sum of Net income
            //3. Divide by # of months since first rent start date
            //4. Save it
            //5. Sum all of it
            //6. Divide by # of prop currently rented
            DateTime now = DateTime.Now;
            foreach (Property item in Properties)
            {
                
                MonthlyFinancials = MonthlyFinancialsMaster.Where(w => w.PropertyId == item.ID).GroupBy(g => g.PropertyId).Select(s => new MonthlyFinancial { PropertyId = s.Key, Net = s.Sum(x => x.Net) });
                int NumOfMonths = ((now.Year - item.DateOfPurchase.Year) * 12) + now.Month - item.DateOfPurchase.Month;
                if (item.ID == 1)
                    AvgNetIncome47F = (int)MonthlyFinancials.First().Net / NumOfMonths;
                else if (item.ID == 2)
                    AvgNetIncome198W = (int)MonthlyFinancials.First().Net / NumOfMonths;
            }
            OverAllNetIncomeMonthly = (AvgNetIncome47F + AvgNetIncome198W) / Properties.Count();
           
        }

        public async Task OnTabClick(int tabNum)
        {
            try
            {
                Log.Information("OnTab_Click Clicked.");
                SelectedTab = tabNum;
                SetInfoCards();
                if (tabNum == 0) //OverAll Tab
                {

                    //**************************************************************************
                    //***************THIS IS VERY IMPORTANT OTHERWISE CLICKING ON DIFFERENT TABS DOES NOT CHANGE GRAPH***********************************************************
                    //***************ESSENTIALLY YOU WAIT FOR PAGE TO BE LOADED THEN DRAW GRAPH ***********************************************************
                    await Task.Delay(2);
                    await JSRuntime.InvokeVoidAsync("topContributorPieChart", JsonConvert.SerializeObject(OverAllIncomeByProperty));
                    await JSRuntime.InvokeVoidAsync("generateMonthlyExpenseChart", JsonConvert.SerializeObject(ExpenseByCategories), true);
                    //await JSRuntime.InvokeVoidAsync("topContributorNetPieChart", JsonConvert.SerializeObject(OverAllNetIncomeByProperty));
                }
                else
                {
                    //SelectedTab = tabNum;
                    int year = TabNumToYearMapping[tabNum];
                    MonthlyFinancials = GetMonthlyFinancial(Property.ID, year);
                    if (Property.ID != -1)
                    {
                        //SetInfoCards();
                        RoomOccupancies = GetRoomOccupancies(Property.ID, year);
                        Expenses = GetExpenses(Property.ID, year);
                    }
                   
                    //await Task.Delay(2);
                    //SetInfoCards();
                    //StateHasChanged();
                    //**************************************************************************
                    //***************THIS IS VERY IMPORTANT OTHERWISE CLICKING ON DIFFERENT TABS DOES NOT CHANGE GRAPH***********************************************************
                    //***************ESSENTIALLY YOU WAIT FOR PAGE TO BE LOADED THEN DRAW GRAPH ***********************************************************
                    await Task.Delay(2);
                    //**************************************************************************
                    //**************************************************************************
                    await JSRuntime.InvokeVoidAsync("generateMonthlyExpenseChart_" + year.ToString(), JsonConvert.SerializeObject(MonthlyFinancials));
                }


            }
            catch (Exception e)
            {
                Log.Error("Exception in OnTabClick in HomeComponentBase.\n" + e.Message);
            }

        }

        public async Task OnPropertyTabClick(int tabNum)
        {
            if (tabNum == 0)
            {
                double lat = 0;
                double lng = 0;
                if (Property.ID == 1)
                {
                    lat = 42.10914364410624;
                    lng = -75.94791438872407;
                }
                if (Property.ID == 2)
                {
                    lat = 42.10951341171027;
                    lng = -75.94589527550427;
                }
                await Task.Delay(2);
                await JSRuntime.InvokeVoidAsync("initMap", lat, lng, Property.ShortName);

            }
        }

       
        public int GetYearFromSelectedTab()
        {
            return TabNumToYearMapping[SelectedTab];
        }


        public void SetInfoCards()
        {
            InfoCardList.Clear();


            foreach (var item in InfoItemList)
            {
                InfoCard infoCard = new InfoCard();
                decimal totalAllProperties = 0;
                infoCard.Type = item;
                if (item.Equals("Income"))
                    totalAllProperties = GetMonthlyFinancial(Property.ID, GetYearFromSelectedTab()).Sum(x => x.Income);
                else if (item.Equals("Expense"))
                    totalAllProperties = GetMonthlyFinancial(Property.ID, GetYearFromSelectedTab()).Sum(x => x.Expenditure);
                else if (item.Equals("Net"))
                    totalAllProperties = GetMonthlyFinancial(Property.ID, GetYearFromSelectedTab()).Sum(x => x.Net);
                if (Properties != null)
                {
                    foreach (var prpty in Properties)
                    {
                        string houseName = "";
                        decimal totalPerHouse = 0;
                        IEnumerable<MonthlyFinancial> mnthlyFin;
                        mnthlyFin = GetMonthlyFinancial(prpty.ID, GetYearFromSelectedTab());
                        houseName = prpty.ShortName;
                        if (item.Equals("Income"))
                            totalPerHouse = mnthlyFin.Sum(x => x.Income);
                        else if (item.Equals("Expense"))
                            totalPerHouse = mnthlyFin.Sum(x => x.Expenditure);
                        else if (item.Equals("Net"))
                            totalPerHouse = mnthlyFin.Sum(x => x.Net);
                        infoCard.PropertyList.Add(houseName, totalPerHouse);
                    }
                }
                infoCard.Amount = totalAllProperties;
                InfoCardList.Add(infoCard);
            }
        }

        public List<MonthlyFinancial> GetMonthlyFinancial(int propId, int year)
        {
            //If Selectedtab ==  0 meaning Overall shit, then disregard 'Year' paramter and query based on Property
            if (SelectedTab == 0)
            {
                if (propId == -1)
                {
                    return MonthlyFinancialsMaster.GroupBy(g => g.PropertyId).Select(s => new MonthlyFinancial { Expenditure = s.Sum(sum => sum.Expenditure), Income = s.Sum(sum => sum.Income), Net = s.Sum(sum => sum.Net) }).ToList();
                }
                else
                {
                    return MonthlyFinancialsMaster.Where(w => w.PropertyId == propId).GroupBy(g => g.PropertyId).Select(s => new MonthlyFinancial { Expenditure = s.Sum(sum => sum.Expenditure), Income = s.Sum(sum => sum.Income), Net = s.Sum(sum => sum.Net) }).ToList();
                }
            }
            else
            {
                if (propId == -1)
                    
                return MonthlyFinancialsMaster.Where(w => w.Year == year).GroupBy(g => new { g.Year, g.Month })
                        .Select(s => new MonthlyFinancial { Year = s.Key.Year, Month = s.Key.Month, Expenditure = s.Sum(sum => sum.Expenditure), Income = s.Sum(sum => sum.Income), Net = s.Sum(sum => sum.Net) }).ToList();
                
                return MonthlyFinancialsMaster.Where(w => w.PropertyId == propId && w.Year == year).GroupBy(g => new { g.Year, g.Month, g.MonthNumber, g.PropertyId })
                                .Select(s => new MonthlyFinancial { Year = s.Key.Year, Month = s.Key.Month, MonthNumber = s.Key.MonthNumber, PropertyId = s.Key.PropertyId, Expenditure = s.Sum(sum => sum.Expenditure), Income = s.Sum(sum => sum.Income), Net = s.Sum(sum => sum.Net) }).ToList();
            }


        }

        public List<RoomOccupancy> GetRoomOccupancies(int propId, int year)
        {
            return RoomOccupanciesMaster.Where(x => x.PropertyId == propId && x.Year == year).ToList();
        }

        public List<ExpenseByYearMonth> GetExpenses(int propId, int year)
        {
            return ExpensesMaster.Where(x => x.PropertyId == propId && x.Year == year).ToList();
        }

        public async void OnClickAddItem()
        {
            if (!string.IsNullOrWhiteSpace(NewToDoItem))
            {
                ToDoItem toDoItem = new ToDoItem();
                toDoItem.ProprtyId = Property.ID;
                toDoItem.Description = NewToDoItem;
                toDoItem.IsDone = false;
                int numOfRows = DBService.InsertIntoToDoItems(toDoItem);
                NewToDoItem = string.Empty;
                ToDoItemsDone = (await DBService.GetToDoItems(Property.ID, 1)).ToList();
            }
            StateHasChanged();
        }

        public string CellDisplayValue(decimal val)
        {
            //https://www.csharp-examples.net/string-format-double/
            if (val == 0)
                return "-";
            else
                return "$" + String.Format("{0:0}", val);
        }
    }
}
