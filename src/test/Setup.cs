using LinqToDB;
using esp32_client.Builder;
using System.Reflection;
using FluentMigrator.Runner;
using LinqToDB.AspNet;
using Microsoft.Extensions.DependencyInjection;
using esp32_client.Services;
using LinqToDB.Data;

namespace test;

[SetUpFixture]
public class BaseTest
{
#nullable disable
    private static IServiceProvider _serviceProvider;
    // private static readonly string connectionString = "Data Source=:memory:";
    private static readonly string connectionString = "Data Source=database.sqlite;Mode=Memory;New=True;";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var services = new ServiceCollection();

        services.AddEntityFrameworkInMemoryDatabase();

        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                // Add SQLite support to FluentMigrator
                .AddSQLite()
                // Set the connection string
                .WithGlobalConnectionString(connectionString)
                // Define the assembly containing the migrations
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            // .BuildServiceProvider(true)
            ;

        services.AddLinqToDBContext<LinqToDb>((provider, options)
        => options
            .UseSQLite(connectionString)
        , ServiceLifetime.Scoped);


        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddScoped<IFactoryService, FactoryService>();
        services.AddScoped<ILineService, LineService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProcessService, ProcessService>();
        services.AddScoped<IMachineService, MachineService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IRoleOfUserService, RoleOfUserService>();
        services.AddScoped<IUserRightService, UserRightService>();
        services.AddScoped<IStationService, StationService>();
        services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
        services.AddScoped<IDataReportService, DataReportService>();
        services.AddScoped<ILogService, LogService>();

        services.AddSingleton<Settings>();

        _serviceProvider = services.BuildServiceProvider();

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

