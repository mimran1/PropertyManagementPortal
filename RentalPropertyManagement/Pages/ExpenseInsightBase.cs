using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using RentalPropertyManagement.Models;
using RentalPropertyManagement.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentalPropertyManagement.Pages
{
    public class ExpenseInsightBase : ComponentBase
    {
        [Inject]
        public DBService DBService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }
        public IEnumerable<ExpenseByCategory> MonthlyExpenseByCategories_2020 { get; set; }
        public IEnumerable<ExpenseByCategory> MonthlyExpenseByCategories_2021 { get; set; }
        [Inject]
        public PropertyService PropertyService { get; set; }
        public Property Property { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    Property = PropertyService.Property;
                    MonthlyExpenseByCategories_2020 = (from expenseByCategory in (await DBService.GetExpenseByCategory()).ToList()
                                                       where expenseByCategory.Year == 2020
                                                       select new ExpenseByCategory
                                                       {
                                                           Year = expenseByCategory.Year,
                                                           Month = expenseByCategory.Month,
                                                           Category = expenseByCategory.Category,
                                                           Amount = expenseByCategory.Amount
                                                       }).ToList();
                    MonthlyExpenseByCategories_2021 = (from expenseByCategory in (await DBService.GetExpenseByCategory()).ToList()
                                                       where expenseByCategory.Year == 2021
                                                       select new ExpenseByCategory
                                                       {
                                                           Year = expenseByCategory.Year,
                                                           Month = expenseByCategory.Month,
                                                           Category = expenseByCategory.Category,
                                                           Amount = expenseByCategory.Amount
                                                       }).ToList();
                    StateHasChanged();
                    await JSRuntime.InvokeVoidAsync("expenseInsight_2020", JsonConvert.SerializeObject(MonthlyExpenseByCategories_2020));
                    await JSRuntime.InvokeVoidAsync("expenseInsight_2021", JsonConvert.SerializeObject(MonthlyExpenseByCategories_2021));
                }
                catch (Exception e)
                {

                }

            }

        }

        public async Task OnTab2020_Click()
        {
            Log.Information("OnTab2020_Click Clicked");
            if (MonthlyExpenseByCategories_2020 != null)
            {
                await JSRuntime.InvokeVoidAsync("expenseInsight_2020", JsonConvert.SerializeObject(MonthlyExpenseByCategories_2020));

            }
        }
        public async Task OnTab2021_Click()
        {
            Log.Information("OnTab2021_Click Clicked");
            if (MonthlyExpenseByCategories_2021 != null)
            {
                await JSRuntime.InvokeVoidAsync("expenseInsight_2021", JsonConvert.SerializeObject(MonthlyExpenseByCategories_2021));

            }
        }

    }
}
