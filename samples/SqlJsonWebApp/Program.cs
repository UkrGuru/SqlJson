using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using UkrGuru.SqlJson;

namespace SqlJsonWebApp
{
    public class Program
    {
        private static void SetupDb()
        {
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=True";

            DbHelper.ConnectionString = connectionString;
            Assembly.GetExecutingAssembly().ExecResource("InitDb.sql");

            DbHelper.ConnectionString = connectionString.Replace("master", "SqlJsonDemo");
            Assembly.GetExecutingAssembly().ExecResource("SeedDb.sql");
        }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            SetupDb();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
