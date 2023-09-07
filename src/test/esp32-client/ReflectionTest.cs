using System.Reflection;
using esp32_client.Controllers;
using esp32_client.Domain;
using esp32_client.Utils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    public async Task GetControllerMethods()
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
    }

    [Test]
    public async Task ShouldGetControllerMethods()
    {
        var data = await Utils.GetControllerMethods();
        var userRight = new List<UserRight>();

        foreach (var controller in data)
        {
            var strMethods = JsonConvert.SerializeObject(controller.Value);
            var listMethods = JsonConvert.DeserializeObject<List<string>>(strMethods);
            foreach (var method in listMethods)
            {
                userRight.Add(new UserRight { RoleId = 1, ControllerName = controller.Key, ActionName = method });
            }
        }
        Assert.That(userRight.Count, Is.GreaterThan(0));
    }
}