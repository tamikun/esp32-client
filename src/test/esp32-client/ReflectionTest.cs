using System.Reflection;
using esp32_client.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace test;

[TestFixture]
public class ReflectionTest
{
    public ReflectionTest()
    { }

    [SetUp]
    public void Setup()
    { }

    [Test]
    public async Task ShouldGetControllerMethods()
    {
        string assemblyName = "esp32-client";
        var assembly = Assembly.LoadFrom($"{assemblyName}.dll");

        var controllerTypes = assembly.GetTypes()
         .Where(type => typeof(ControllerBase).IsAssignableFrom(type) && !type.IsAbstract)
         .Where(s => s != typeof(BaseController) && s != typeof(TestApiController));

        var dict = new Dictionary<string, object>();

        var tasks = controllerTypes.Select(async controllerType =>
        {
            var data = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(s => s.ReturnType == typeof(Task<IActionResult>))
                .Select(s => s.Name)
                .ToList();

            dict.Add(controllerType.Name, data);
            await Task.CompletedTask;
        });

        await Task.WhenAll(tasks);

        System.Console.WriteLine("==== dict: " + Newtonsoft.Json.JsonConvert.SerializeObject(dict));
    }
}