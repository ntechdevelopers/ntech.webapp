using System;
using System.IO;
using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Extensions.Configuration;
using Serilog;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Ntech.Migration
{
    class Program
    {
        private static IConfiguration _configuration;

        private enum DBName
        {
            NtechDatabase = 0,
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var lastArg = 0;
            for (; lastArg < args.Length; lastArg++)
            {
                if (IsArg(args[lastArg], "ntech-database"))
                {
                    Log.Information("Run migration - Db");
                    Run(DBName.NtechDatabase, "ntech-database");
                    continue;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"{args[lastArg]} not found.");
                }
            }
        }

        private static void Run(DBName dBName, string schemaName)
        {
            var connString = _configuration.GetConnectionString(dBName.ToString());
            var scriptFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Scripts/{dBName}");
            //EnsureDatabase.For.SqlDatabase(connString);
            //EnsureDatabase.For.MySqlDatabase(connString);
            EnsureDatabase.For.PostgresqlDatabase(connString);

            Log.Information($"Run schemaName={schemaName}");

            var upgradeBuilder = DeployChanges.To
                //.MySqlDatabase(connString, schemaName)
                //.SqlDatabase(connString, schemaName)
                .PostgresqlDatabase(connString)
                .WithScriptsFromFileSystem(Path.Combine(scriptFolderPath, "Scripts"), new SqlScriptOptions { RunGroupOrder = 1, ScriptType = ScriptType.RunOnce })
                .WithScriptsFromFileSystem(Path.Combine(scriptFolderPath, "SeedData"), new SqlScriptOptions { RunGroupOrder = 2, ScriptType = ScriptType.RunOnce });

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                upgradeBuilder.WithScriptsFromFileSystem(Path.Combine(scriptFolderPath, "SampleData"), new SqlScriptOptions { RunGroupOrder = 3, ScriptType = ScriptType.RunOnce });
            }
            var upgrader = upgradeBuilder.LogToAutodetectedLog().Build();
            upgrader.PerformUpgrade();
        }

        private static bool IsArg(string candidate, string name)
        {
            return (name != null && candidate.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
