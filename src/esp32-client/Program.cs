using System.Reflection;
using esp32_client.Builder;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ConfigureEndpointDefaults(listenOptions =>
    {
        // ...
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPatternService, PatternService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IRoleOfUserService, RoleOfUserService>();
builder.Services.AddScoped<IUserRightService, UserRightService>();

builder.Services.AddSingleton<Settings>();
builder.Services.AddSingleton<ListServer>();

builder.Services.AddSession(option => option.IdleTimeout = TimeSpan.FromMinutes(30)); // 10:01
builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>(); // Add HttpContextAccessor

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
                .BuildServiceProvider(false);


builder.Services.AddLinqToDBContext<LinqToDb>((provider, options)
            => options
                //will configure the AppDataConnection to use
                .UseMySql(connectionString)
            //default logging will log everything using the ILoggerFactory configured in the provider
            // .UseDefaultLogging(provider)
            , ServiceLifetime.Scoped);

// Instantiate the runner
var serviceProvider = builder.Services.BuildServiceProvider();
var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger(options =>
    {
        options.SerializeAsV2 = true;
    });
    app.UseSwaggerUI();

}

app.UseSession(); // Enable session state

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
