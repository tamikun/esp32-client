using esp32_client.Builder;
using LinqToDB;
using Microsoft.EntityFrameworkCore;

namespace test;

[TestFixture]
public class EFCore
{
    private Context _context;
    public EFCore()
    {
        _context = BaseTest.GetService<Context>();
    }

    [SetUp]
    public void Setup()
    { }

# nullable disable

    [Test]
    public async Task ShouldUseEF()
    {
        var data = await _context.Factory.OrderBy(s => s.Id).LastOrDefaultAsync();
        System.Console.WriteLine("==== data: " + Newtonsoft.Json.JsonConvert.SerializeObject(data));
    }


}