using System.Reflection;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;
using LinqToDB.AspNet;
using Microsoft.OpenApi.Models;
using esp32_client.Models.Singleton;
using Microsoft.EntityFrameworkCore;

namespace esp32_client.Builder
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureApplicationServices(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ToDo API",
                    Description = "An ASP.NET Core Web API for managing ToDo items",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Example Contact",
                        Url = new Uri("https://example.com/contact")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/license")
                    }
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[] {}
                    },
                });
            });

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureEndpointDefaults(listenOptions =>
                {
                    // ...
                });
            });

            builder.ConfigureServices();

            builder.ConfigureFluentMigrator();

            builder.ConfigureLinqToDB();

            builder.ConfigureEfCore();

            builder.Services.AddHostedService<ScheduledTask>();
        }

        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromDays(30)); // Check expired time by token
            builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddScoped<IFactoryService, FactoryService>();
            builder.Services.AddScoped<ILineService, LineService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProcessService, ProcessService>();
            builder.Services.AddScoped<IMachineService, MachineService>();
            builder.Services.AddScoped<IUserAccountService, UserAccountService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<IRoleOfUserService, RoleOfUserService>();
            builder.Services.AddScoped<IUserRightService, UserRightService>();
            builder.Services.AddScoped<IStationService, StationService>();
            builder.Services.AddScoped<IScheduleTaskService, ScheduleTaskService>();
            builder.Services.AddScoped<IDataReportService, DataReportService>();
            builder.Services.AddScoped<ILogService, LogService>();
            builder.Services.AddScoped<ISettingService, SettingService>();
            builder.Services.AddScoped<IUserSessionService, UserSessionService>();
            builder.Services.AddSingleton<Settings>();
        }

        public static void ConfigureFluentMigrator(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration["Settings:ConnectionString"].ToString();

            builder.Services.AddFluentMigratorCore()
                            .ConfigureRunner(rb => rb
                                // Add SQLite support to FluentMigrator
                                .AddMySql4()
                                // Set the connection string
                                .WithGlobalConnectionString(connectionString)
                                // Define the assembly containing the migrations
                                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
                            // Enable logging to console in the FluentMigrator way
                            .AddLogging(lb => lb.AddFluentMigratorConsole())
                            // Build the service provider
                            // .BuildServiceProvider(false)
                            ;
        }

        public static void ConfigureLinqToDB(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration["Settings:ConnectionString"].ToString();
            builder.Services.AddLinqToDBContext<LinqToDb>((provider, options)
                        => options
                            //will configure the AppDataConnection to use
                            .UseMySql(connectionString)
                        //default logging will log everything using the ILoggerFactory configured in the provider
                        // .UseDefaultLogging(provider)
                        , ServiceLifetime.Scoped);
        }

        public static void ConfigureEfCore(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration["Settings:ConnectionString"].ToString();

            builder.Services.AddDbContext<Context>(
                options => options.UseMySQL(connectionString)
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.None)))
            );
        }
    }
}