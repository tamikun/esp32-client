using esp32_client.Domain;
using LinqToDB;
using LinqToDB.Data;

namespace esp32_client.Builder;

public class LinqToDb : DataConnection
{
    public LinqToDb(DataOptions<LinqToDb> options)
       : base(options.Options)
    { }

    public ITable<UserAccount> UserAccount => this.GetTable<UserAccount>();
    public ITable<Pattern> Pattern => this.GetTable<Pattern>();
    public ITable<Product> Product => this.GetTable<Product>();

    public async Task BulkInsert<T>(List<T> source) where T : class
    {
        var copyOptions = new BulkCopyOptions(TableOptions: TableOptions.CreateIfNotExists, BulkCopyType: BulkCopyType.MultipleRows, CheckConstraints: true);
        var temp = await this.BulkCopyAsync<T>(copyOptions, source);

    }
}