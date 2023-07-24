using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using esp32_client.Domain;
using FluentMigrator;
using LinqToDB;

namespace esp32_client.Builder;

[Migration(20180430122500)]
public class AddTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(UserAccount)).Exists())
        {
            Create
            .Table(nameof(UserAccount))
                .WithColumn(nameof(UserAccount.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserAccount.LoginName)).AsString().NotNullable().Unique()
                .WithColumn(nameof(UserAccount.Password)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.UserName)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.SalfKey)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.RoleId)).AsInt32().Nullable();
        }

        if (!Schema.Table(nameof(Pattern)).Exists())
        {
            Create
            .Table(nameof(Pattern))
                .WithColumn(nameof(Pattern.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Pattern.PatternNumber)).AsString().NotNullable().Unique()
                .WithColumn(nameof(Pattern.FileName)).AsString().NotNullable()
                .WithColumn(nameof(Pattern.FileData)).AsString(int.MaxValue)
                .WithColumn(nameof(Pattern.Description)).AsString();
        }

        if (!Schema.Table(nameof(Product)).Exists())
        {
            Create
            .Table(nameof(Product))
                .WithColumn(nameof(Product.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Product.ProductName)).AsString()
                .WithColumn(nameof(Product.ProcessName)).AsString()
                .WithColumn(nameof(Product.Order)).AsInt32()
                .WithColumn(nameof(Product.PatternNumber)).AsString();
        }

        if (!Schema.Table(nameof(Machine)).Exists())
        {
            Create
            .Table(nameof(Machine))
                .WithColumn(nameof(Machine.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Machine.MachineName)).AsString().NotNullable()
                .WithColumn(nameof(Machine.IpAddress)).AsString().NotNullable();
        }

        if (!Schema.Table(nameof(Setting)).Exists())
        {
            Create
            .Table(nameof(Setting))
                .WithColumn(nameof(Setting.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Setting.Name)).AsString().NotNullable()
                .WithColumn(nameof(Setting.Value)).AsString().NotNullable();
        }
    }


    public override void Down()
    { }
}

[Migration(20230724135800)]
public class AddInitData : AutoReversingMigration
{
    private readonly LinqToDb _linq2Db;
    public AddInitData(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }
    public override void Up()
    {
        _linq2Db.InsertAsync(new UserAccount { LoginName = "admin", Password = "MM08+DTe5SgJ/abCwZW2oRlH+g8mO1XyQxStxGwTetI=", SalfKey = "YSguanX0gfFpM9t6Cn711Q==", UserName = "QuanTM", RoleId = 1 }).Wait();

    }
}

[Migration(20230724194300)]
public class AddSettingData : AutoReversingMigration
{
    private readonly LinqToDb _linq2Db;
    public AddSettingData(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }
    public override void Up()
    {
        var settings = new List<Setting>{
            new Setting{Name = "GetDataTimeOut", Value = "1000"},
            new Setting{Name = "PostFileTimeOut", Value = "1000"},
            new Setting{Name = "FileDataDirectory", Value = "/app/FileData/"},
            new Setting{Name = "NodeListEspFile", Value = "//table[@class='fixed']/tbody/tr"},
            new Setting{Name = "NodeServerState", Value = "//p[@id='result']"},

        };
        _linq2Db.BulkInsert(settings).Wait();
    }
}

