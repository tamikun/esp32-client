using System.Linq.Expressions;
using System.Reflection;
using esp32_client.Domain;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;

namespace esp32_client.Builder;

public class LinqToDb : DataConnection
{
    public LinqToDb(DataOptions<LinqToDb> options)
       : base(options.Options)
    { }

    public ITable<Factory> Factory => this.GetTable<Factory>();
    public ITable<Line> Line => this.GetTable<Line>();
    public ITable<Machine> Machine => this.GetTable<Machine>();
    public ITable<Station> Station => this.GetTable<Station>();
    public ITable<Process> Process => this.GetTable<Process>();
    public ITable<Product> Product => this.GetTable<Product>();
    public ITable<RoleOfUser> RoleOfUser => this.GetTable<RoleOfUser>();
    public ITable<Setting> Setting => this.GetTable<Setting>();
    public ITable<UserAccount> UserAccount => this.GetTable<UserAccount>();
    public ITable<UserRight> UserRight => this.GetTable<UserRight>();
    public ITable<UserRole> UserRole => this.GetTable<UserRole>();

    public async Task BulkInsert<T>(IEnumerable<T> source) where T : BaseEntity
    {
        var copyOptions = new BulkCopyOptions(TableOptions: TableOptions.CreateIfNotExists, BulkCopyType: BulkCopyType.MultipleRows, CheckConstraints: true);
        var temp = await this.BulkCopyAsync<T>(copyOptions, source);
    }

    public async Task<T?> Update<T>(T source) where T : BaseEntity
    {
#nullable disable
        if (source is null) return null;

        var propertyInfos = typeof(T).GetProperties();

        if (propertyInfos is null) return null;

        // Get Id
        PropertyInfo pId = propertyInfos.FirstOrDefault(s => s.Name == "Id");

        int id = Int32.Parse(pId?.GetValue(source)?.ToString() ?? "0");

        // update query
        IUpdatable<T> updateQuery = this.GetTable<T>().Where(p => p.Id == id).AsUpdatable();

        foreach (var prop in propertyInfos.Where(s => s.Name != "Id"))
        {
            // Create the member access expression for the property
            var parameter = Expression.Parameter(typeof(T), "p");
            var memberAccess = Expression.Property(parameter, prop);

            // Convert value type to nullable and then to object
            var conversion = Expression.Convert(memberAccess, typeof(object));

            // Create the lambda expression: p => (object)p.PropertyName
            var lambdaExpression = Expression.Lambda<Func<T, object>>(conversion, parameter);

            // Get the new value of the property from the source object
            object propValue = prop.GetValue(source);

            // Use the Set method with the dynamically created lambda expression
            updateQuery = updateQuery.Set(lambdaExpression, propValue);
        }

        await updateQuery.UpdateAsync();

        return source;
    }


}