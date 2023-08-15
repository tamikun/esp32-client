using esp32_client.Builder;
using FluentMigrator.Runner;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureApplicationServices();

var app = builder.Build();

var runner = app.Services.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

EngineContext.SetServiceProvider(app.Services);

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
