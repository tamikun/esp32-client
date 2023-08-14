using LinqToDB;
using esp32_client.Builder;
using System.Reflection;
using FluentMigrator.Runner;
using LinqToDB.AspNet;
using Microsoft.Extensions.DependencyInjection;
using esp32_client.Services;

namespace test;

[SetUpFixture]
public static class BaseTest //: IDisposable
{
#nullable disable
    private static IServiceProvider _serviceProvider;
    // private static readonly string connectionString = "Data Source=:memory:";
    private static readonly string connectionString = "Data Source=database.sqlite;Mode=Memory;New=True;";

    [OneTimeSetUp]
    public static void OneTimeSetUp()
    {
        var services = new ServiceCollection();

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
            // .BuildServiceProvider(false)
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
    public static void OneTimeTearDown()
    {
        var linq2Db = GetService<LinqToDb>();
        linq2Db.Close();
        linq2Db.Dispose();
    }

    // public void Dispose()
    // {
    //     // if (_serviceProvider is IDisposable disposable)
    //     // {
    //     //     disposable.Dispose();
    //     // }
    // }

    public static void InitData()
    {
        var linq2Db = GetService<LinqToDb>();
        var runner = GetService<IMigrationRunner>();

        var addTable = new AddTable();
        runner.Up(addTable);

        // var addInitData = new AddInitData(linq2Db);
        // runner.Up(addInitData);


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

