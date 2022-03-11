using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RentalPropertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace RentalPropertyManagement.Services
{
    /// <summary>
    /// This class is responsible for retriving data from database. DB is located at connString (see below)
    /// </summary>
    public class DBService
    {
        //private string connString = @"Server=localhost\SQLEXPRESS;Database=RentalPropertyManagementPROD;Trusted_Connection=True;";
        private IWebHostEnvironment _env;
        private IConfiguration _config;
        private string _connString = "";
        private string qryToOpen;
        private StreamReader _file;
        private string sql;
        private string _qryLocation;
        private string _blobContainer;

        public DBService(IConfiguration configuration, IWebHostEnvironment env)
        {
            _config = configuration;
            _env = env;
            _connString = configuration.GetConnectionString("RentalPropertyManagementContextConnection");
            _qryLocation = _config.GetConnectionString("RentalPropertyManagementStorageAccount");
            _blobContainer = "queries";
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_connString);
            }
        }

        /// <summary>
        /// Depending on where you are running the app (env or prod) get query from different location.
        /// </summary>
        /// <param name="qryToOpen"></param>
        /// <returns></returns>
        public string GetQueryContent(string qryToOpen)
        {
            string contents = "";
            if (_env.IsDevelopment())
            {
                _file = new StreamReader(_config.GetValue<string>("QueryLocationLocal") + qryToOpen);
                contents = _file.ReadToEnd();
                _file.Close();
                return contents;
            }
            else
            {
                //Ref: https://briancaos.wordpress.com/2019/03/28/read-blob-file-from-microsoft-azure-storage-with-net-core/
                contents = GetBlob(_blobContainer, qryToOpen);
                return contents;
            }
        }

        public string GetBlob(string containerName, string fileName)
        {
            string contents = "";
            try
            {
                string connectionString = _qryLocation; ;

                // Setup the connection to the storage account
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

                // Connect to the blob storage
                CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
                // Connect to the blob container
                CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
                // Connect to the blob file
                CloudBlockBlob blob = container.GetBlockBlobReference($"{fileName}");
                // Get the blob file as text
                contents = blob.DownloadTextAsync().Result;

            }
            catch (Exception e)
            {
                Log.Error("Error getting blob data. Blob: " + containerName + " Filename: " + fileName);
            }
            return contents;
        }

        public IEnumerable<int> GetDistinctYear(int propId)
        {
            if (propId == -1)
            {
                qryToOpen = "GetDistinctYear.sql";
                sql = GetQueryContent(qryToOpen);
                try
                {
                    using (IDbConnection dbConnection = Connection)
                    {
                        dbConnection.Open();
                        //Log.Information("Environment " + _env.EnvironmentName);
                        //Log.Information("Query: " + sql);
                        return dbConnection.Query<int>(sql);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                    return Enumerable.Empty<int>();
                }
            }
            else
            {
                qryToOpen = "GetDistinctYearById.sql";
                sql = GetQueryContent(qryToOpen);
                DynamicParameters param = new DynamicParameters();
                param.Add("@PropertyId", propId);
                try
                {
                    using (IDbConnection dbConnection = Connection)
                    {
                        dbConnection.Open();
                        return dbConnection.Query<int>(sql, param);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                    return Enumerable.Empty<int>();
                }
            }

        }

        public async Task<IEnumerable<int>> GetDistinctYearAsync()
        {
            qryToOpen = "GetDistinctYear.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<int>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<int>();
            }
        }

        /// <summary>
        /// Get all expenses in a year categorized by Category they fall under (e.g. Material, labor etc)
        /// </summary>
        /// <returns>A Task of IEnumerable of type ExpenseByCategory</returns>
        public async Task<IEnumerable<ExpenseByCategory>> GetExpenseByCategory()
        {

            qryToOpen = "GetExpenseByCategoryAll.sql";
            string val = GetQueryContent(qryToOpen);
            sql = GetQueryContent(qryToOpen);

            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<ExpenseByCategory>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<ExpenseByCategory>(); ;
            }

        }

        public async Task<IEnumerable<ExpenseByYearMonth>> GetExpense()
        {
            qryToOpen = "GetExpenseAll.sql";
            sql = GetQueryContent(qryToOpen);

            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Query: " + sql);

                    return await dbConnection.QueryAsync<ExpenseByYearMonth>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<ExpenseByYearMonth>(); ;
            }
        }

        /// <summary>
        /// Get all the income per year per room.
        /// </summary>
        /// <returns>A Task of Ienumerable of type IncomeByRoom</returns>
        public async Task<IEnumerable<IncomeByRoom>> GetIncomeByRoom()
        {
            qryToOpen = "GetIncomeByRoomAll.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<IncomeByRoom>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<IncomeByRoom>(); ;
            }


        }

        public async Task<IEnumerable<MonthlyFinancial>> GetMonthlyFinancial(int propId, string year)
        {
            if (propId == -1)
                return await GetMonthlyFinancialAll();
            else
                return await GetMonthlyFinancialPropId(propId, year);
        }

        /// <summary>
        /// Gets the monthly finanical detail which includes the overall expense, income and the net for each year.
        /// </summary>
        /// <param name="year">The year user is looking to get the financial detail for</param>
        /// <returns>A Task of IEnumerable of type MonthlyFinancial</returns>
        private async Task<IEnumerable<MonthlyFinancial>> GetMonthlyFinancialPropId(int propId, string year)
        {
            qryToOpen = "GetMonthlyFinancialPropId.sql";
            sql = GetQueryContent(qryToOpen);
            var parameter = new { Year = year, PropertyId = propId };
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("MySetting: " + _config.GetValue<string>("MySetting"));
                    //Log.Information("Parameter:\n" + "@Year:" + parameter.Year, parameter.PropertyId);
                    //Log.Information("Query: " + sql);
                    //if(year.Equals("2020"))
                    //    await Task.Delay(2000);
                    //if(year.Equals("2021"))
                    //    await Task.Delay(3000);
                    return await dbConnection.QueryAsync<MonthlyFinancial>(sql, parameter);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<MonthlyFinancial>();
            }
        }

        public async Task<IEnumerable<MonthlyFinancial>> GetMonthlyFinancialAll()
        {
            qryToOpen = "GetMonthlyFinancialAll.sql";
            sql = GetQueryContent(qryToOpen);

            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Query: " + sql);
                    //if(year.Equals("2020"))
                    //    await Task.Delay(2000);
                    //if(year.Equals("2021"))
                    //    await Task.Delay(3000);
                    return await dbConnection.QueryAsync<MonthlyFinancial>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<MonthlyFinancial>();
            }
        }

        public async Task<IEnumerable<MonthlyFinancial>> GetYearlyFinancial(int propId, string year)
        {
            qryToOpen = "GetYearlyFinancialByPropId.sql";
            sql = GetQueryContent(qryToOpen);
            var parameter = new { Year = year, PropertyId = propId };
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Environment " + _env.EnvironmentName);
                    //Log.Information("Parameter:\n" + "@Year:" + parameter.Year);
                    //Log.Information("Query: " + sql);
                    //if(year.Equals("2020"))
                    //    await Task.Delay(2000);
                    //if(year.Equals("2021"))
                    //    await Task.Delay(3000);
                    return await dbConnection.QueryAsync<MonthlyFinancial>(sql, parameter);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<MonthlyFinancial>();
            }
        }

        public async Task<IEnumerable<ExpenseItem>> GetExpenseItems(int propId)
        {
            if (propId == -1)
            {
                try
                {
                    qryToOpen = "GetExpenseItemsAll.sql";
                    sql = GetQueryContent(qryToOpen);
                    using (IDbConnection dbConnection = Connection)
                    {
                        dbConnection.Open();
                        //IEnumerable<ExpenseItem> items = await dbConnection.QueryAsync<ExpenseItem>(sql);
                        //foreach (var item in items)
                        //{
                        //    item.Year = item.DateOfExpense.Year;
                        //    item.Month = item.DateOfExpense.ToString("MMMM");
                        //    item.Day = item.DateOfExpense.Day;
                        //}
                        return await dbConnection.QueryAsync<ExpenseItem>(sql);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                    //https://stackoverflow.com/questions/1969993/is-it-better-to-return-null-or-empty-collection
                    return Enumerable.Empty<ExpenseItem>();
                }
            }
            else
            {
                try
                {
                    qryToOpen = "GetExpenseItemsPropId.sql";

                    sql = GetQueryContent(qryToOpen);
                    var parameter = new { PropertyId = propId };
                    using (IDbConnection dbConnection = Connection)
                    {
                        dbConnection.Open();
                        //IEnumerable<ExpenseItem> items = await dbConnection.QueryAsync<ExpenseItem>(sql, parameter);
                        //foreach (var item in items)
                        //{
                        //    item.Year = item.DateOfExpense.Year;
                        //    item.Month = item.DateOfExpense.ToString("MMMM");
                        //    item.Day = item.DateOfExpense.Day;
                        //}

                        return await dbConnection.QueryAsync<ExpenseItem>(sql, parameter);
                        //return await dbConnection.Query(sql).Select(row => new ExpenseItem()).ToList();
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                    //https://stackoverflow.com/questions/1969993/is-it-better-to-return-null-or-empty-collection
                    return Enumerable.Empty<ExpenseItem>();
                }
            }

        }

        public int InsertIntoGeneralLedger(int propId, ExpenseItem expenseItem)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@PropertyId", propId);
            param.Add("@Vendor", expenseItem.Vendor);
            param.Add("@Year", expenseItem.Year);
            param.Add("@Month", expenseItem.Month);
            param.Add("@DateOfExpense", expenseItem.DateOfExpense);
            param.Add("@Category", expenseItem.Category);
            param.Add("@Amount", expenseItem.Amount);
            param.Add("@Description", expenseItem.Description);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spInsertIntoGeneralLedger", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while inserting data into database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int UpdateGeneralLedger(ExpenseItem expenseItem)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@ID", expenseItem.ID);
            param.Add("@Vendor", expenseItem.Vendor);
            param.Add("@DateOfExpense", expenseItem.DateOfExpense);
            param.Add("@Category", expenseItem.Category);
            param.Add("@Amount", expenseItem.Amount);
            param.Add("@Description", expenseItem.Description);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spUpdateGeneralLedger", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while updating data in database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int DeleteGeneralLedger(int ID)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@ID", ID);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spDeleteGeneralLedger", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while deleting data from database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int UpdateRentInformation(IncomeByRoom incomeByRoom)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@ID", incomeByRoom.ID);
            param.Add("@Year", incomeByRoom.Year);
            param.Add("@Month", incomeByRoom.Month);
            param.Add("@Room", incomeByRoom.Room);
            param.Add("@Amount", incomeByRoom.Amount);
            param.Add("@Description", incomeByRoom.Description);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spUpdateRentInformation", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while updating data in database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int DeleteRentInformation(int ID)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@ID", ID);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spDeleteRentInformation", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while deleting data from database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int InsertIntoRentInformationAsync(int propId, IncomeByRoom incomeByRoom)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@PropertyId", propId);
            param.Add("@Year", incomeByRoom.Year);
            param.Add("@Month", incomeByRoom.Month);
            param.Add("@Room", incomeByRoom.Room);
            param.Add("@Amount", incomeByRoom.Amount);
            param.Add("@Description", incomeByRoom.Description);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        bool exists = CheckRentInfoExists(propId, incomeByRoom);
                        if (!exists)
                        {
                            numOfRowsAffected = dbConnection.Execute("spInsertIntoRentInformation", param, trans, commandType: CommandType.StoredProcedure);
                            trans.Commit();
                            return numOfRowsAffected;
                        }
                        else
                            return -1;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while inserting data into database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                        return numOfRowsAffected;
                    }
                }
            }

        }

        bool CheckRentInfoExists(int propId, IncomeByRoom incomeByRoom)
        {
            bool exists = false;
            try
            {
                qryToOpen = "GetSingleRentInformation.sql";
                sql = GetQueryContent(qryToOpen);
                var parameter = new { Year = incomeByRoom.Year, Month = incomeByRoom.Month, Room = incomeByRoom.Room, PropertyId = propId };
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Parameter:\n" + "@PropertyId:");
                    //Log.Information("Query: " + sql);
                    exists = dbConnection.ExecuteScalar<bool>(sql, parameter);
                    return exists;
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return exists;
            }

        }

        public async Task<IEnumerable<RoomOccupancy>> GetRoomOccupany()
        {
            qryToOpen = "GetRoomOccupanyAll.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<RoomOccupancy>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<RoomOccupancy>(); ;
            }
        }

        public Property GetPropertyDetail(int propId)
        {
            sql = "select * from RentalProperties where id=" + propId;
            //sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Query: " + sql);
                    return dbConnection.QuerySingle<Property>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return null;
            }
        }

        public IEnumerable<Property> GetAllProperties()
        {
            qryToOpen = "GetAllProperties.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Query: " + sql);
                    return dbConnection.Query<Property>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<Property>();
            }
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            qryToOpen = "GetAllProperties.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<Property>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception Occurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<Property>(); ; ;
            }
        }

        /// <summary>
        /// Gets the ledger items.
        /// </summary>
        /// <param name="propId">Property ID to retrive by
        /// <returns>A Task of IEnumerable of type GeneralLedgerItem</returns>
        public async Task<IEnumerable<ExpenseItem>> GetGeneralLedgerItem()
        {
            qryToOpen = "GetGeneralLedgerItem.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //Log.Information("Parameter:\n" + "@Property Id:" + parameter.PropertyId);
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<ExpenseItem>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<ExpenseItem>();
            }
        }

        public async Task<IEnumerable<OverAllNetIncome>> GetOverAllNetIncome()
        {
            qryToOpen = "GetOverAllNetIncome.sql";
            sql = GetQueryContent(qryToOpen);

            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    //await Task.Delay(3000);
                    //Log.Information("Query: " + sql);
                    return await dbConnection.QueryAsync<OverAllNetIncome>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<OverAllNetIncome>();
            }
        }

        public async Task<IEnumerable<PropertyTax>> GetPropertyTaxInfo(int propId)
        {
            qryToOpen = "GetPropertyTaxInfoByPropId.sql";
            sql = GetQueryContent(qryToOpen);
            DynamicParameters param = new DynamicParameters();
            param.Add("@PropertyId", propId);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    return await dbConnection.QueryAsync<PropertyTax>(sql, param);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<PropertyTax>();
            }
        }

        public async Task<IEnumerable<ToDoItem>> GetToDoItems(int propId, int isDone)
        {
            qryToOpen = "GetToDoItems.sql";
            sql = GetQueryContent(qryToOpen);
            DynamicParameters param = new DynamicParameters();
            param.Add("@PropertyId", propId);
            param.Add("@IsDone", isDone);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    return await dbConnection.QueryAsync<ToDoItem>(sql, param);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<ToDoItem>();
            }
        }

        public int InsertIntoToDoItems(ToDoItem toDoItem)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@PropertyId", toDoItem.ProprtyId);
            param.Add("@Description", toDoItem.Description);
            param.Add("@IsDone", toDoItem.IsDone);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spInsertIntoToDoItems", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while inserting data into database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public int InsertIntoFeedback(string comment)
        {
            int numOfRowsAffected = 0;
            DynamicParameters param = new DynamicParameters();
            param.Add("@Comment", comment);
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                using (var trans = dbConnection.BeginTransaction())
                {
                    try
                    {
                        numOfRowsAffected = dbConnection.Execute("spInsertIntoFeedBack", param, trans, commandType: CommandType.StoredProcedure);
                        trans.Commit();
                        return numOfRowsAffected;
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        Log.Error("Exception occurred while inserting data into database. Rolled back any updates.\n" + "Exception detail: " + e.Message);
                    }
                }
            }
            return numOfRowsAffected;
        }

        public async Task<IEnumerable<MonthlyFinancialGrid>> GetMonthlyFinancialGrid()
        {
            qryToOpen = "GetMonthlyFinancialGrid.sql";
            sql = GetQueryContent(qryToOpen);
            try
            {
                using (IDbConnection dbConnection = Connection)
                {
                    dbConnection.Open();
                    return await dbConnection.QueryAsync<MonthlyFinancialGrid>(sql);
                }
            }
            catch (Exception e)
            {
                Log.Error("Exception cccurred while retriving data from database\n" + "Exception detail: " + e.Message);
                return Enumerable.Empty<MonthlyFinancialGrid>();
            }
        }
    }
}
