using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeOpenXml;
using RentalPropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;
using BlazorStrap;

namespace RentalPropertyManagement.Components
{
    public class LedgerGridComponentBase : ComponentBase
    {
        [Parameter]
        public IEnumerable<ExpenseItem> ExpenseItems { get; set; }
        [Parameter]
        public IEnumerable<IncomeByRoom> IncomeByRooms { get; set; }
        public Property Property { get; set; }
        [Inject]
        public ISessionStorageService SessionStorageService { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                Property = await SessionStorageService.GetItemAsync<Property>("Property");
                StateHasChanged();
                await JSRuntime.InvokeVoidAsync("removeUnderline");
            }
        }
        public string GetCellValue(decimal val)
        {
            if (val != 0)
                return "$" + String.Format("{0:0.##}", val);

            return "-";
        }

        public async void Export(string type)
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //IEnumerable<IncomeByRoom> rooms = (from r in IncomeByRooms
            //                                   select new IncomeByRoom
            //                                   {
            //                                       Year = r.Year,
            //                                       Month = r.Month,
            //                                       Room = r.Room,
            //                                       Amount = r.Amount,
            //                                       Description = r.Description
            //                                   });
            //IEnumerable<ExpenseItem> expenses = (from e in ExpenseItems
            //                                     select new ExpenseItem
            //                                     {
            //                                         Vendor = e.Vendor,
            //                                         Year = e.Year,
            //                                         Month = e.Month,
            //                                         Category = e.Category,
            //                                         Amount = e.Amount,
            //                                         Description = e.Description,
            //                                         DateOfExpense = e.DateOfExpense
            //                                     });
            DataTable rooms = ToDataTable<IncomeByRoom>(IncomeByRooms.ToList());
            DataTable expenses = ToDataTable<ExpenseItem>(ExpenseItems.ToList());
            using (var package = new ExcelPackage())
            {
                var workSheetIncome = package.Workbook.Worksheets.Add("Income");
                workSheetIncome.Cells.LoadFromDataTable(rooms, true);

                var workSheetExpense = package.Workbook.Worksheets.Add("Expense");
                workSheetExpense.Cells.LoadFromDataTable(expenses, true);
                fileContents = package.GetAsByteArray();
            }
            await JSRuntime.InvokeVoidAsync("saveAsFile", Property.ShortName + "_Ledger_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx", Convert.ToBase64String(fileContents));
        }

        public async Task OnTabClick(int tabNum)
        {
            try
            {


            }
            catch (Exception e)
            {

            }

        }

        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// https://www.chinhdo.com/20090402/convert-list-to-datatable/
        /// </summary>
        public DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] propsMain = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo[] props;
            if (Property.ID == -1)
            {
                props = propsMain.Where(
                    p => p.Name != "incomeByRooms"
                    && p.Name != "ID"
                    && p.Name != "MonthNumber"
                    && p.Name != "PropertyId"
                            ).ToArray();
            }
            else
            {
                props = propsMain.Where(
                    p => p.Name != "incomeByRooms"
                    && p.Name != "ID"
                    && p.Name != "ShortName"
                    && p.Name != "MonthNumber"
                    && p.Name != "PropertyId"
                            ).ToArray();
            }
            
           
            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                if(prop.Name != "ID" && prop.Name != "incomeByRooms")
                    tb.Columns.Add(prop.Name, t);
                
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }

        public void Show(BSTabEvent e)
        {
            Console.WriteLine($"Show   -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
        }
        public void Shown(BSTabEvent e)
        {
            Console.WriteLine($"Shown  -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
        }
        public void Hide(BSTabEvent e)
        {
            Console.WriteLine($"Hide   ->  Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
        }
        public void Hidden(BSTabEvent e)
        {
            Console.WriteLine($"Hidden -> Activated: {e.Activated?.Id.ToString()} , Deactivated: {e.Deactivated?.Id.ToString()}");
        }
    }
}
