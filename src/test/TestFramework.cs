using AutoMapper;
using esp32_client.Builder;
using esp32_client.Domain;
using esp32_client.Models;
using esp32_client.Models.Singleton;
using esp32_client.Services;
using FluentMigrator.Runner;
using LinqToDB;

namespace test;

[TestFixture]
public class TestFramework
{
# nullable disable
    private IMapper _mapper;

    public TestFramework()
    {
        _mapper = BaseTest.GetService<IMapper>();
    }

    [Test]
    public async Task ShouldMapModels()
    {
        var createModel = new UserAccountCreateModel
        {
            LoginName = "1",
            Password = "2",
            UserName = "3",
            RoleId = 4
        };
        UserAccount modelMap = _mapper.Map<UserAccount>(createModel);
        System.Console.WriteLine("==== modelMap: " + Newtonsoft.Json.JsonConvert.SerializeObject(modelMap));
        await Task.CompletedTask;
    }

}