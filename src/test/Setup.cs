using LinqToDB;
using esp32_client.Builder;
using System.Reflection;
using FluentMigrator.Runner;
using LinqToDB.AspNet;
using Microsoft.Extensions.DependencyInjection;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace test;

[SetUpFixture]
public class BaseTest
{
#nullable disable
    private static IServiceProvider _serviceProvider;
    private static readonly string connectionString = "Data Source=database.sqlite;";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddEntityFrameworkInMemoryDatabase();

        builder.Services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            ;

        builder.Services.AddLinqToDBContext<LinqToDb>((provider, options)
            => options
                .UseSQLite(connectionString)
            , ServiceLifetime.Scoped);


        // builder.Services.AddSingleton<DbConnection, SqliteConnection>(serviceProvider =>
        // {
        //     var connection = new SqliteConnection(connectionString);
        //     connection.Open();
        //     return connection;
        // });

        // builder.Services.AddDbContext<Context>((serviceProvider, options) =>
        // {
        //     var connection = serviceProvider.GetRequiredService<DbConnection>();
        //     options.UseSqlite(connection);
        // });
        
        builder.Services.AddDbContext<Context>(options =>
            options.UseSqlite(connectionString)
        );

        builder.ConfigureServices();

        _serviceProvider = builder.Services.BuildServiceProvider();

        EngineContext.SetServiceProvider(_serviceProvider);

        InitData();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        var linq2Db = GetService<LinqToDb>();

        var tables = linq2Db.DataProvider.GetSchemaProvider().GetSchema(new DataConnection(linq2Db.DataProvider, connectionString, linq2Db.MappingSchema)).Tables;
        foreach (var table in tables)
        {
            // Drop the table by executing raw SQL
            linq2Db.Execute($"DROP TABLE IF EXISTS {table.TableName}");
        }

    }

    public void InitData()
    {
        var linq2Db = GetService<LinqToDb>();

        var runner = GetService<IMigrationRunner>();

        var addTable = new AddTable();
        runner.Up(addTable);

        var addInitData = new AddInitData(GetService<LinqToDb>());
        addInitData.Up();

        var addTimeOutSetting = new AddTimeOutSetting(linq2Db);
        addTimeOutSetting.Up();
    }

    public static T GetService<T>()
    {
        try
        {
            return _serviceProvider.GetRequiredService<T>();
        }
        catch (InvalidOperationException ex)
        {
            System.Console.WriteLine("[ERROR]: cannot get " + typeof(T).FullName + ex.ToString());
            throw;
        }
    }
}

