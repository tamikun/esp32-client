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
    public ITable<ScheduleTask> ScheduleTask => this.GetTable<ScheduleTask>();
    public ITable<DataReport> DataReport => this.GetTable<DataReport>();
    public ITable<Log> Log => this.GetTable<Log>();
}

public static class LinqToDbExtension
{
    public static async Task BulkInsert<T>(this LinqToDb _linq2db, IEnumerable<T> source) where T : BaseEntity
    {
        var copyOptions = new BulkCopyOptions(TableOptions: TableOptions.CreateIfNotExists, BulkCopyType: BulkCopyType.MultipleRows, CheckConstraints: true);
        var temp = await _linq2db.BulkCopyAsync<T>(copyOptions, source);
    }

    public static async Task<T?> Update<T>(this LinqToDb _linq2db, T source) where T : BaseEntity
    {
#nullable disable
        if (source is null) return null;

        var propertyInfos = typeof(T).GetProperties();

        if (propertyInfos is null) return null;

        // Get Id
        PropertyInfo pId = propertyInfos.FirstOrDefault(s => s.Name == nameof(BaseEntity.Id));

        int id = Int32.Parse(pId?.GetValue(source)?.ToString() ?? "0");

        if (id == 0) return null;

        // update query
        IUpdatable<T> updateQuery = _linq2db.GetTable<T>().Where(p => p.Id == id).AsUpdatable();

        foreach (var prop in propertyInfos.Where(s => s.Name != nameof(BaseEntity.Id)))
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
    public static async Task<T> Insert<T>(this LinqToDb _linq2db, T source) where T : BaseEntity
    {
        if (source is null) throw new Exception("Cannot insert value null");

        source.Id = await _linq2db.InsertWithInt32IdentityAsync(source);

        return source;
    }

    public static async Task Delete<T>(this LinqToDb _linq2db, T source) where T : BaseEntity
    {
        if (source is null) return;

        await _linq2db.DeleteAsync(source);
    }

    public static async Task DeleteQuery<T>(this IQueryable<T> query) where T : BaseEntity
    {
        await query.DeleteAsync();
    }

    public static async Task UpdateQuery<T>(this IUpdatable<T> query) where T : BaseEntity
    {
        await query.UpdateAsync();
    }

    // public static async Task Truncate<T>(this LinqToDb _linq2db, bool restartIdentity = false) where T : BaseEntity
    // {
    //     if (restartIdentity)
    //     {
    //         // await _linq2db.ExecuteAsync($"TRUNCATE TABLE {typeof(T).Name}");
    //     }
    //     else
    //     {
    //         await _linq2db.GetTable<T>().DeleteAsync();
    //     }
    // }
}