using DbPerformanceTest.Domain;
using DbPerformanceTest.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Data.Entity;

namespace DbPerformanceTest
{
    class Program
    {
        private const int NumberOfLoops = 1;

        static void Main(string[] args)
        {
            var averageADONet = TestADONet();
            var averageEntityFramework = TestEntityFramework();
            var averageEntityFrameworkAsNoTracking = TestEntityFrameworkAsNoTracking();
            var averageEntityFrameworkViewModel = TestEntityFrameworkViewModel();

            Console.WriteLine($"ADO.NET > {averageADONet}ms - BASE");
            Console.WriteLine($"Entity Framework > {averageEntityFramework}ms - {CalculateAndFormatPercentage(averageADONet, averageEntityFramework)}");
            Console.WriteLine($"Entity Framework AsNoTracking > {averageEntityFrameworkAsNoTracking}ms - {CalculateAndFormatPercentage(averageADONet, averageEntityFrameworkAsNoTracking)}");
            Console.WriteLine($"Entity Framework ViewModel > {averageEntityFrameworkViewModel}ms - {CalculateAndFormatPercentage(averageADONet, averageEntityFrameworkViewModel)}");

            Console.ReadLine();
        }

        public static string CalculateAndFormatPercentage(int firstNumber, int secondNumber)
        {
            double result = secondNumber - firstNumber;
            result = result / firstNumber;
            result = result * 100;

            return $"{result.ToString("N2")}%";
        }

        public static int TestADONet()
        {
            var counter = 1;
            var executionTimesInMs = new List<long>();
            var stopwatch = new Stopwatch();

            while (counter <= NumberOfLoops)
            {
                stopwatch.Start();

                using (var connection = new SqlConnection(
                @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=AdventureWorks2016;Integrated Security=True"))
                {
                    connection.Open();

                    var sql = $"SELECT WorkOrderId, " +
                        $"w.ProductId, " +
                        $"OrderQty, " +
                        $"StartDate," +
                        $"Name, " +
                        $"MakeFlag, " +
                        $"ProductNumber, " +
                        $"SellStartDate " +
                        $"FROM Production.WorkOrder w " +
                        $"INNER JOIN Production.Product p ON w.ProductId = p.ProductId ";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            var records = new List<WorkOrderViewModel>();

                            while (reader.Read())
                            {
                                var workOrder = new WorkOrderViewModel
                                {
                                    WorkOrderId = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(1),
                                    OrderQty = reader.GetInt32(2),
                                    StartDate = reader.GetDateTime(3),
                                    Product = new ProductViewModel
                                    {
                                        Name = reader.GetString(4),
                                        MakeFlag = reader.GetBoolean(5),
                                        ProductId = reader.GetInt32(1),
                                        ProductNumber = reader.GetString(6),
                                        SellStartDate = reader.GetDateTime(7)
                                    }
                                };

                                records.Add(workOrder);
                            }
                        }
                    }

                    connection.Close();
                }

                stopwatch.Stop();
                executionTimesInMs.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                counter++;
            }

            return Convert.ToInt32(executionTimesInMs.Average());
        }

        public static int TestEntityFramework()
        {
            var counter = 1;
            var executionTimesInMs = new List<long>();
            var stopwatch = new Stopwatch();

            while (counter <= NumberOfLoops)
            {
                stopwatch.Start();

                using(var dbContext = new ApplicationDbContext())
                {
                    var records = dbContext
                        .WorkOrders
                        .Where(p => p.Product.WorkOrders.Any())
                        .Include(p => p.Product)
                        .ToList();
                }

                stopwatch.Stop();
                executionTimesInMs.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                counter++;
            }

            return Convert.ToInt32(executionTimesInMs.Average());
        }

        public static int TestEntityFrameworkAsNoTracking()
        {
            var counter = 1;
            var executionTimesInMs = new List<long>();
            var stopwatch = new Stopwatch();

            while (counter <= NumberOfLoops)
            {
                stopwatch.Start();

                using(var dbContext = new ApplicationDbContext())
                {
                    var records = dbContext
                        .WorkOrders
                        .Include(p => p.Product)
                        .AsNoTracking()
                        .ToList();
                }

                stopwatch.Stop();
                executionTimesInMs.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                counter++;
            }

            return Convert.ToInt32(executionTimesInMs.Average());
        }

        public static int TestEntityFrameworkViewModel()
        {
            var counter = 1;
            var executionTimesInMs = new List<long>();
            var stopwatch = new Stopwatch();

            while (counter <= NumberOfLoops)
            {
                stopwatch.Start();

                using (var dbContext = new ApplicationDbContext())
                {
                    var records = dbContext
                        .WorkOrders
                        .Select(p => new WorkOrderViewModel
                        {
                            OrderQty = p.OrderQty,
                            ProductId = p.ProductId,
                            StartDate = p.StartDate,
                            WorkOrderId = p.WorkOrderId,
                            Product = new ProductViewModel
                            {
                                MakeFlag = p.Product.MakeFlag,
                                Name = p.Product.Name,
                                ProductId = p.ProductId,
                                ProductNumber = p.Product.ProductNumber,
                                SellStartDate = p.Product.SellStartDate
                            }
                        })
                        .ToList();
                }

                stopwatch.Stop();
                executionTimesInMs.Add(stopwatch.ElapsedMilliseconds);
                stopwatch.Reset();
                counter++;
            }

            return Convert.ToInt32(executionTimesInMs.Average());
        }
    }
}
