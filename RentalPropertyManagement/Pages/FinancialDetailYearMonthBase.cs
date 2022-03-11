using Blazored.SessionStorage;
using BlazorStrap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using RentalPropertyManagement.Components;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class FinancialDetailYearMonthBase : ComponentBase
    {
        [Parameter]
        public string Year { get; set; }
        [Parameter]
        public string Month { get; set; }
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        
        public IEnumerable<ExpenseByCategory> ExpenseByCategories { get; set; }
        public IEnumerable<IncomeByRoom> IncomeByRooms { get; set; }
        public IEnumerable<ExpenseItem> ExpenseItems { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal IncomeTotal { get; set; }
        public string jsonExpenseByCategory { get; set; }
        public string jsonIncomeByRoom { get; set; }
        public bool DataAvailable { get; set; } = false;
        public bool ExpenseDataAvailable { get; set; } = false;
        public bool IncomeDataAvailable { get; set; } = false;
        public bool RetrivingData { get; set; } = true;
        public BSModal VerticallyCentered;
        public RadzenGrid<ExpenseByCategory> grid;
        public RadzenGrid<ExpenseItem> expenseItemsGrid;
        public RadzenGrid<IncomeByRoom> gridIncome;
        public RadzenGrid<IncomeByRoom> incomeItemsGrid;
        public int DeletedExpenseItemID { get; set; }
        public int DeletedIncomeItemID { get; set; }
        public int UpdatedExpenseItem { get; set; }
        public int UpdatedIncomeItem { get; set; }
        public string UpdateErrorMessage { get; set; } = "";
        public ConfirmBase ConfirmRef { get; set; }
        public bool deleteExpenseItem { get; set; } = false;
        public bool deleteIncomeItem { get; set; } = false;
        public ExpenseItem ExpenseItem { get; set; }
        public IncomeByRoom IncomeItem { get; set; }
        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        public ConfirmItems ConfirmItems { get; set; }
        public ExpenseByCategory ExpenseByCategory { get; set; }

        [Inject]
        public PageHistoryService PageHistoryService { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        public bool PropertySet { get; set; } = false;
        public int NumOfRoomsOccupied { get; set; }
        
        public IEnumerable<ExpenseByCategory> ExpenseByCategoriesAll { get; set; }
        public IEnumerable<IncomeByRoom> IncomeByRoomsAll { get; set; }
        
        public List<IncomeByRoom> NetIncomeAllList { get; set; }
        public string jsonNetAll { get; set; }
        public Dictionary<string, string> MonthNumberMap = new Dictionary<string, string>()
        {
            {"01", "January" },
            {"02", "February" },
            {"03", "March" },
            {"04", "April" },
            {"05", "May" },
            {"06", "June" },
            {"07", "July" },
            {"08", "August" },
            {"09", "September" },
            {"10", "October" },
            {"11", "November" },
            {"12", "December" }
        };
        public List<string> YearList = new List<string>()
        {
            "2022",
            "2021",
            "2020"
        };
        public List<string> MonthList = new List<string>()
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        //Created following two varialbles so that the  Year Month displayed on the header (left side atm) dont change when user is selecting new dates from dropdown
        public string SelectYear { get; set; }
        public string SelectMonth { get; set; }


        protected override void OnInitialized()
        {
            Year = Year ?? "2022";
            Month = Month ?? "January";
            SelectMonth = Month;
            SelectYear = Year;
            PageHistoryService.AddPageToHistory(NavigationManager.Uri);
            ConfirmItems = new ConfirmItems();
            ConfirmItems.Title = "Are you sure you want to delete?";
            ConfirmItems.Message = "Confirm Delete";
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                Property = await SessionStorageService.GetItemAsync<Property>("Property");
                //Set LastPage variable of the session to the current page. So when u change the property from the ChangePropertyComponent the LastPage variable will to used to navigate.
                await SessionStorageService.SetItemAsync<string>("LastPage", NavigationManager.Uri);
                //Set FirstVisit variable of the session to N. So when u change the propety from the ChangeProperComponent the FirstVisit will be N and the UI will display currently selected Property
                await SessionStorageService.SetItemAsync<string>("FirstVisit", "N");
                if (Property == null)
                {
                    Property = PropertyService.Property; //use defualt (all properties) in the event the session storage has nothing
                }
                PropertySet = true;
                StateHasChanged(); //Soon as the property is set change the Markup to display the Property Name on top. Then go ahead with Data fetch from DB.
                /*Useful LINQ reference:
                 https://www.csharp-examples.net/linq-sum/
                https://www.youtube.com/watch?v=cH7ZV98zkpU
                */
                RetrivingData = true;
                ExpenseItems = Enumerable.Empty<ExpenseItem>();
                IncomeByRoomsAll = (await DBService.GetIncomeByRoom()).ToList();
                ExpenseByCategoriesAll = (await DBService.GetExpenseByCategory()).ToList();
                if (Month.Equals("All months"))
                {
                    if (Property.ID != -1)
                    {
                        //Multiple Grouppby: https://dotnettutorials.net/lesson/groupby-multiple-keys-in-linq/
                        IncomeByRooms = IncomeByRoomsAll.Where(w => w.Year == Int32.Parse(this.Year) && w.PropertyId == Property.ID).GroupBy(g => new { g.Year, g.Room })
                            .Select(s => new IncomeByRoom { Room = s.Key.Room, Amount = s.Sum(sum => sum.Amount) }).ToList();
                    }
                    else
                    {
                        //Multiple groupbysss: https://dotnettutorials.net/lesson/groupby-multiple-keys-in-linq/
                        IncomeByRooms = IncomeByRoomsAll.Where(w => w.Year == Int32.Parse(this.Year)).GroupBy(g => new { g.Year, g.ShortName })
                            .Select(s => new IncomeByRoom { ShortName = s.Key.ShortName, Amount = s.Sum(sum => sum.Amount) }).ToList();
                    }

                    if (Property.ID != -1)
                    {
                        ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.Year == Int32.Parse(this.Year) && w.PropertyId == Property.ID).GroupBy(g => new { g.Year, g.Category })
                            .Select(s => new ExpenseByCategory { Year = s.Key.Year, Category = s.Key.Category, Amount = s.Sum(sum => sum.Amount) }).ToList();
                    }
                    else
                    {
                        ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.Year == Int32.Parse(this.Year)).GroupBy(g => new { g.Year, g.Category })
                            .Select(s => new ExpenseByCategory { Year = s.Key.Year, Category = s.Key.Category, Amount = s.Sum(sum => sum.Amount) }).ToList();
                    }



                    if (ExpenseByCategories.Count() > 0)
                    {
                        ExpenseDataAvailable = true;
                        DataAvailable = true;
                        //Get all the expenses for each item in ExpenseByCategories. 
                        //Basically get all the expense items from GenralLedger for a given year,month,category
                        //So if 2020 August Material = 1000 and Material bought were as follows:
                        // 1. Oven 500
                        // 2. Dishwasher 250
                        // 3. Fridge 250
                        // Then ExpenseByCategories[i] = 2020 August Material and
                        // ExpenseByCategories[i].expenseItems[0] = Oven 500
                        // ExpenseByCategories[i].expenseItems[1] = Dishwahser 500
                        // ExpenseByCategories[i].expenseItems[2] = Fridge 500
                        foreach (ExpenseByCategory item2 in ExpenseByCategories)
                        {
                            item2.expenseItems = (await DBService.GetExpenseItems(Property.ID)).Where(w => w.Category == item2.Category && w.Year == item2.Year).
                                Select(s => new ExpenseItem { Year = s.Year, Month = s.Month, Vendor = s.Vendor, Category = s.Category, DateOfExpense = s.DateOfExpense, Amount = s.Amount, Description = s.Description, ID = s.ID })
                                .ToList();
                        }
                    }
                }
                else //Not 'All Months'
                {
                    if (Property.ID != -1)
                    {
                        IncomeByRooms = IncomeByRoomsAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month && w.PropertyId == Property.ID)
                                            .Select(s => new IncomeByRoom { ID = s.ID, Year = s.Year, Month = s.Month, Room = s.Room, Amount = s.Amount, Description = s.Description, ShortName = s.ShortName })
                                            .ToList();
                    }
                    else
                    {
                        IncomeByRooms = IncomeByRoomsAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month).GroupBy(g => new {g.ShortName, g.Year, g.Month})
                                        .Select(s => new IncomeByRoom { Year = s.Key.Year, Month = s.Key.Month, Amount = s.Sum(sum => sum.Amount), ShortName = s.Key.ShortName })
                                        .ToList();
                    }

                    if (Property.ID != -1)
                    {
                        ExpenseByCategories = (from expenseByCategory in ExpenseByCategoriesAll
                                               where expenseByCategory.Year == Int32.Parse(this.Year) && expenseByCategory.Month == this.Month && expenseByCategory.PropertyId == Property.ID
                                               group expenseByCategory by new { expenseByCategory.Year, expenseByCategory.Month, expenseByCategory.Category, expenseByCategory.ShortName } into eGroup
                                               select new ExpenseByCategory
                                               {
                                                   Year = eGroup.Key.Year,
                                                   Month = eGroup.Key.Month,
                                                   Category = eGroup.Key.Category,
                                                   Amount = eGroup.Sum(x => x.Amount),
                                                   ShortName = eGroup.Key.ShortName
                                               }).ToList();
                        ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month && w.PropertyId == Property.ID).GroupBy(g => new { g.Year, g.Month, g.Category, g.ShortName })
                                                .Select(s => new ExpenseByCategory { Year = s.Key.Year, Month = s.Key.Month, Category = s.Key.Category, Amount = s.Sum(sum => sum.Amount), ShortName = s.Key.ShortName })
                                                .ToList();
                    }
                    else
                    {
                        ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month).GroupBy(g => new { g.Year, g.Month, g.ShortName })
                                                .Select(s => new ExpenseByCategory { Year = s.Key.Year, Month = s.Key.Month, Amount = s.Sum(sum => sum.Amount), ShortName = s.Key.ShortName })
                                                .ToList();
                    }




                    if (ExpenseByCategories.Count() > 0)
                    {
                        ExpenseDataAvailable = true;
                        DataAvailable = true;
                        foreach (ExpenseByCategory item2 in ExpenseByCategories)
                        {
                            item2.expenseItems = (await DBService.GetExpenseItems(Property.ID)).Where(w => w.Category == item2.Category && w.Year == item2.Year && w.Month == item2.Month)
                                                    .Select(s => new ExpenseItem { Year = s.Year, Month = s.Month, Vendor = s.Vendor, Category = s.Category, DateOfExpense = s.DateOfExpense, Amount = s.Amount, Description = s.Description, ID = s.ID })
                                                    .ToList();
                        }
                    }
                }
                ExpenseTotal = ExpenseByCategories.Sum(x => x.Amount);
                IncomeTotal = IncomeByRooms.Sum(x => x.Amount);
                NumOfRoomsOccupied = IncomeByRooms.Count<IncomeByRoom>();
                if (IncomeByRooms.Count() > 0)
                {
                    IncomeDataAvailable = true;
                    DataAvailable = true;
                    //Initialize the list contained for each object of IncomeByRoom in the list IncomeByRooms
                    //Notice the where clause. Year, Month, Room combined forms a unique key so you are guranteed 1 record.
                    //At the end of the what you have is a list of all the records per Year and Month (IncomeByRooms), then 
                    //for each record with in that you have one record per year, per month per room.
                    //All this bullshit was done so that RadZen inner grid could be used.
                    //  IncomeByRooms[3]:
                    //  [0]    2020 August Room1A 500 'Rent Aug2020'
                    //              IncomeByRooms[0].incomeByRooms:     2020 August Room1A 500 'Rent Aug2020'
                    //  [1]    2020 August Room2A 500 'Rent Aug2020'
                    //              IncomeByRooms[1].incomeByRooms:     2020 August Room2A 500 'Rent Aug2020'
                    //  [2]    2020 August Room1B 500 'Rent Aug2020'
                    //              IncomeByRooms[2].incomeByRooms:     2020 August Room1B 500 'Rent Aug2020'
                    //  [3]    2020 August Room1C 500 'Rent Aug2020'
                    //              IncomeByRooms[3].incomeByRooms:     2020 August Room1C 500 'Rent Aug2020'
                    foreach (IncomeByRoom item in IncomeByRooms)
                    {
                       item.incomeByRooms = IncomeByRooms.Where(w => w.Year == item.Year && w.Month == item.Month && w.Room == item.Room).GroupBy(g => new { g.Year, g.Month, g.Room })
                                                .Select(s => new IncomeByRoom { ID = item.ID, Year = item.Year, Month = item.Month, Room = s.Key.Room, Amount = s.Sum(x => x.Amount), Description = item.Description })
                                                .ToList();
                    }
                }

                int countE = ExpenseByCategories.Count();
                int countI = IncomeByRooms.Count();
                int maxCount = 0;
                if (countE > countI)
                    maxCount = countE;
                else
                    maxCount = countI;
                NetIncomeAllList = new List<IncomeByRoom>();
                decimal net;
                if (maxCount > 0)
                {
                    for (int i = 0; i < maxCount; i++)
                    {
                        string name = "";
                        try
                        {
                            name = IncomeByRooms.ElementAt(i).ShortName;
                        }
                        catch
                        {
                            name = ExpenseByCategories.ElementAt(i).ShortName;
                        }

                        decimal income = 0;
                        try
                        {
                            income = IncomeByRooms.ElementAt(i).Amount;
                        }
                        catch
                        {
                            income = 0;
                        }
                        decimal expense = 0;
                        try
                        {
                            expense = ExpenseByCategories.ElementAt(i).Amount;
                        }
                        catch
                        {
                            expense = 0;
                        }
                        net = income - expense;
                        NetIncomeAllList.Add(new IncomeByRoom
                        {
                            ShortName = name,
                            Amount = net
                        });

                    }
                }


                RetrivingData = false;
                StateHasChanged();

                jsonExpenseByCategory = JsonConvert.SerializeObject(ExpenseByCategories);
                jsonIncomeByRoom = JsonConvert.SerializeObject(IncomeByRooms);
                jsonNetAll = JsonConvert.SerializeObject(NetIncomeAllList);
                if (Property.ID != -1)
                {
                    if (ExpenseDataAvailable)
                        await JSRuntime.InvokeVoidAsync("generateMonthlyExpenseChart", jsonExpenseByCategory, false);
                    if (IncomeDataAvailable)
                        await JSRuntime.InvokeVoidAsync("generateMonthlyIncomeChart", jsonIncomeByRoom);
                }
                else
                {
                    if (IncomeDataAvailable)
                        await JSRuntime.InvokeVoidAsync("generateIncomePieChartTop", jsonIncomeByRoom);
                    if (ExpenseDataAvailable)
                        await JSRuntime.InvokeVoidAsync("generateExpensePieChartTop", jsonExpenseByCategory, true);
                    if (IncomeDataAvailable || ExpenseDataAvailable)
                        await JSRuntime.InvokeVoidAsync("generateNetPieChartTop", jsonNetAll);

                }

            }
        }
        public void HandleClick(string year, string month)
        {
            int m = 0;
            NavigationManager.NavigateTo("MonthlyFinancialDetail/" + year + "/" + month, true);
        }
        public void ToggleModal()
        {
            VerticallyCentered.Toggle();
        }

        #region EXPENSE GRID
        //***************** EXPENSE GRID *********************************************************************
        public void RowRender(RowRenderEventArgs<ExpenseByCategory> args)
        {
            args.Expandable = true; //set expandable item for all rows          
        }
        public void RowRenderIncomeGrid(RowRenderEventArgs<IncomeByRoom> args)
        {
            args.Expandable = true; //set expandable item for all rows          
        }
        public void OnUpdateRow(ExpenseItem item)
        {
            int updateStatus = DBService.UpdateGeneralLedger(item);
            if (updateStatus == 0)
                UpdatedExpenseItem = 0;
            else
                UpdatedExpenseItem = item.ID;
        }
        public void OnCreateRow(ExpenseItem item)
        {

        }
        public void EditRow(ExpenseItem item)
        {
            expenseItemsGrid.EditRow(item);
        }
        public async void CancelEdit(ExpenseItem item)
        {
            expenseItemsGrid.CancelEditRow(item);
            //await expenseItemsGrid.Reload();
            foreach (ExpenseByCategory item2 in ExpenseByCategories)
            {
                item2.expenseItems = (from expenseItem in (await DBService.GetExpenseItems(Property.ID)).ToList()
                                      where expenseItem.Category == item2.Category && expenseItem.Year == item2.Year && expenseItem.Month == item2.Month
                                      select new ExpenseItem
                                      {
                                          Year = expenseItem.Year,
                                          Month = expenseItem.Month,
                                          Vendor = expenseItem.Vendor,
                                          Category = expenseItem.Category,
                                          DateOfExpense = expenseItem.DateOfExpense,
                                          Amount = expenseItem.Amount,
                                          Description = expenseItem.Description,
                                          ID = expenseItem.ID
                                      }
                    ).ToList();
            }
            StateHasChanged();
        }
        public async void SaveRow(ExpenseByCategory itemParent, ExpenseItem itemChild)
        {
            await expenseItemsGrid.UpdateRow(itemChild);
            if (UpdatedExpenseItem != 0)
            {
                IEnumerable<ExpenseItem> expenses = itemParent.expenseItems;
                decimal sum = 0;
                foreach (ExpenseItem item in expenses)
                    sum += item.Amount;
                itemParent.Amount = sum;

            }
        }
        public void DeleteRow(ExpenseItem item)
        {
            ConfirmRef.Show();
            ExpenseItem = item;
            deleteExpenseItem = true;
            //after this the show dialog is opened, once user clicks either yes or no ConfirmDelete is called
        }
        //**************************************** EXPENSE GRID ********************************************************************************************
        #endregion


        #region INCOME GRID
        //**************************************** INCOME GRID ********************************************************************************************
        public void OnCreateRowIncomeInnerGrid(IncomeByRoom item)
        {

        }
        public void OnUpdateRowIncomeInnerGrid(IncomeByRoom item)
        {
            int updateStatus = DBService.UpdateRentInformation(item);
            if (updateStatus == 0)
                UpdatedIncomeItem = 0;
            else
                UpdatedIncomeItem = item.ID;

        }
        public void EditRowIncomeInnerGrid(IncomeByRoom item)
        {
            incomeItemsGrid.EditRow(item);
        }

        public async void CancelEditIncomeInnerGrid(IncomeByRoom item)
        {
            incomeItemsGrid.CancelEditRow(item);
            foreach (IncomeByRoom item2 in IncomeByRooms)
            {
                item2.incomeByRooms = (from incomeByRoom in IncomeByRooms
                                       where incomeByRoom.Year == item2.Year && incomeByRoom.Month == item2.Month && incomeByRoom.Room == item2.Room
                                       group incomeByRoom by new { incomeByRoom.Year, incomeByRoom.Month, incomeByRoom.Room } into eGroup
                                       select new IncomeByRoom
                                       {
                                           ID = item2.ID,
                                           Year = item2.Year,
                                           Month = item2.Month,
                                           Room = eGroup.Key.Room,
                                           Amount = eGroup.Sum(x => x.Amount),
                                           Description = item2.Description
                                       }).ToList();
            }
            StateHasChanged();
        }
        public async void SaveRowIncomeInnerGrid(IncomeByRoom itemParent, IncomeByRoom itemChild)
        {
            //UpdateRow automatically calls OnUpdateRowIncomeInnerGrid
            await incomeItemsGrid.UpdateRow(itemChild);
            if (UpdatedIncomeItem != 0)
                itemParent.Amount = itemChild.Amount;
        }
        public void DeleteRowIncomeInnerGrid(IncomeByRoom item)
        {
            ConfirmRef.Show();
            deleteIncomeItem = true;
            IncomeItem = item;
        }
        //**************************************** INCOME GRID ********************************************************************************************
        #endregion

        public void ConfirmDelete(bool val)
        {
            if (val)
            {
                int deleteStatus = 0;

                if (deleteExpenseItem)
                {
                    deleteStatus = DBService.DeleteGeneralLedger(ExpenseItem.ID);
                    if (deleteStatus == 0)
                        DeletedExpenseItemID = 0;
                    else
                    {
                        DeletedExpenseItemID = ExpenseItem.ID;
                        ReloadExpenses();
                    }
                }
                else if (deleteIncomeItem)
                {
                    deleteStatus = DBService.DeleteRentInformation(IncomeItem.ID);
                    if (deleteStatus == 0)
                        DeletedIncomeItemID = 0;
                    else
                    {
                        DeletedIncomeItemID = IncomeItem.ID;
                        ReloadIncome();
                    }
                }
            }
            deleteExpenseItem = false;
            deleteIncomeItem = false;
        }

        async void ReloadExpenses()
        {
            ExpenseByCategories = ExpenseByCategoriesAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month && w.PropertyId == Property.ID).GroupBy(g => new { g.Year, g.Month, g.Category })
                                     .Select(s => new ExpenseByCategory { Year = s.Key.Year, Month = s.Key.Month, Category = s.Key.Category, Amount = s.Sum(sum => sum.Amount) })
                                     .ToList();
            if (ExpenseByCategories.Any())
            {
                ExpenseDataAvailable = true; //May not need this line
                DataAvailable = true; //May not need this line
                ExpenseTotal = ExpenseByCategories.Sum(x => x.Amount);
                jsonExpenseByCategory = JsonConvert.SerializeObject(ExpenseByCategories);
                await JSRuntime.InvokeVoidAsync("generateMonthlyExpenseChart", jsonExpenseByCategory);
                foreach (ExpenseByCategory item2 in ExpenseByCategories)
                {
                    item2.expenseItems = (await DBService.GetExpenseItems(Property.ID)).Where(w => w.Category == item2.Category && w.Year == item2.Year && w.Month == item2.Month)
                                                    .Select(s => new ExpenseItem { Year = s.Year, Month = s.Month, Vendor = s.Vendor, Category = s.Category, DateOfExpense = s.DateOfExpense, Amount = s.Amount, Description = s.Description, ID = s.ID })
                                                    .ToList();
                }
                StateHasChanged();
            }
        }

        async void ReloadIncome()
        {
            IncomeByRooms = IncomeByRoomsAll.Where(w => w.Year == Int32.Parse(this.Year) && w.Month == this.Month && w.PropertyId == Property.ID)
                                .Select(s => new IncomeByRoom { ID = s.ID, Year = s.Year, Month = s.Month, Room = s.Room, Amount = s.Amount, Description = s.Description, ShortName = s.ShortName })
                                .ToList();
            if (IncomeByRooms.Any())
            {
                IncomeDataAvailable = true; //May not need this line
                DataAvailable = true; //May not need this line
                IncomeTotal = IncomeByRooms.Sum(x => x.Amount);
                jsonIncomeByRoom = JsonConvert.SerializeObject(IncomeByRooms);
                await JSRuntime.InvokeVoidAsync("generateMonthlyIncomeChart", jsonIncomeByRoom);
                foreach (IncomeByRoom item in IncomeByRooms)
                {
                    item.incomeByRooms = IncomeByRooms.Where(w => w.Year == item.Year && w.Month == item.Month && w.Room == item.Room).GroupBy(g => new { g.Year, g.Month, g.Room })
                                                .Select(s => new IncomeByRoom { ID = item.ID, Year = item.Year, Month = item.Month, Room = s.Key.Room, Amount = s.Sum(x => x.Amount), Description = item.Description })
                                                .ToList();

                }

                StateHasChanged();
            }
        }

        public void OnDateUpdate(DateTime? value, string name, string format)
        {
            string monthNumber =  value?.ToString(format).Split('/')[0];
            string month = MonthNumberMap[monthNumber];
            string year = value?.ToString(format).Split('/')[2];
            NavigationManager.NavigateTo("MonthlyFinancialDetail/" + year + "/" + month, true);
        }
    }
}
