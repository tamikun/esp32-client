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
}