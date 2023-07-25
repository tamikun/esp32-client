using esp32_client.Domain;
using LinqToDB;
using LinqToDB.Data;

namespace esp32_client.Builder;

public class LinqToDb : DataConnection
{
    public LinqToDb(DataOptions<LinqToDb> options)
       : base(options.Options)
    { }

    public ITable<Department> Department => this.GetTable<Department>();
    public ITable<Line> Line => this.GetTable<Line>();
    public ITable<Machine> Machine => this.GetTable<Machine>();
    public ITable<Pattern> Pattern => this.GetTable<Pattern>();
    public ITable<Process> Process => this.GetTable<Process>();
    public ITable<Product> Product => this.GetTable<Product>();
    public ITable<RoleOfUser> RoleOfUser => this.GetTable<RoleOfUser>();
    public ITable<Setting> Setting => this.GetTable<Setting>();
    public ITable<UserAccount> UserAccount => this.GetTable<UserAccount>();
    public ITable<UserRight> UserRight => this.GetTable<UserRight>();
    public ITable<UserRole> UserRole => this.GetTable<UserRole>();

    public async Task BulkInsert<T>(List<T> source) where T : class
    {
        var copyOptions = new BulkCopyOptions(TableOptions: TableOptions.CreateIfNotExists, BulkCopyType: BulkCopyType.MultipleRows, CheckConstraints: true);
        var temp = await this.BulkCopyAsync<T>(copyOptions, source);

    }
}