using LinqToDB;
using esp32_client.Builder;
using System.Reflection;
using FluentMigrator.Runner;
using LinqToDB.AspNet;
using Microsoft.Extensions.DependencyInjection;
using LinqToDB.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

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
        var builder = WebApplication.CreateBuilder(); // Tạo builder

        builder.Services.AddEntityFrameworkInMemoryDatabase(); // Thêm Entity Framework

        builder.Services.AddFluentMigratorCore()    // Configure FluentMigrator
            .ConfigureRunner(rb => rb
                .AddSQLite()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            ;

        builder.Services.AddLinqToDBContext<LinqToDb>((provider, options)   // Configure LinqToDB
            => options
                .UseSQLite(connectionString)
            , ServiceLifetime.Scoped);
        
        builder.Services.AddDbContext<Context>(options =>
            options.UseSqlite(connectionString)
        );

        builder.ConfigureServices();    // Thêm các khai báo service (Scoped, Singleton,...) ở project esp32-client

        _serviceProvider = builder.Services.BuildServiceProvider(); // Build service

        EngineContext.SetServiceProvider(_serviceProvider); // Khởi tạo giá trị cho EngineContext

        InitData(); // Gọi hàm chạy migration
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

