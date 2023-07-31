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
        if (!Schema.Table(nameof(Factory)).Exists())
        {
            Create
            .Table(nameof(Factory))
                .WithColumn(nameof(Factory.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Factory.FactoryName)).AsString().NotNullable().Unique()
            ;
        }

        if (!Schema.Table(nameof(Line)).Exists())
        {
            Create
            .Table(nameof(Line))
                .WithColumn(nameof(Line.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Line.DepartmentId)).AsInt32()
                .WithColumn(nameof(Line.LineName)).AsString().NotNullable()
                .WithColumn(nameof(Line.Order)).AsInt32()
                .WithColumn(nameof(Line.ProductId)).AsInt32()
            ;
        }

        if (!Schema.Table(nameof(Product)).Exists())
        {
            Create
            .Table(nameof(Product))
                .WithColumn(nameof(Product.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Product.ProductName)).AsString();
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

        if (!Schema.Table(nameof(Process)).Exists())
        {
            Create
            .Table(nameof(Process))
                .WithColumn(nameof(Process.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Process.ProductId)).AsInt32()
                .WithColumn(nameof(Process.ProcessName)).AsString().NotNullable()
                .WithColumn(nameof(Process.PatternId)).AsInt32()
                .WithColumn(nameof(Process.Order)).AsInt32()
            ;
        }

        if (!Schema.Table(nameof(Machine)).Exists())
        {
            Create
            .Table(nameof(Machine))
                .WithColumn(nameof(Machine.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Machine.MachineName)).AsString().NotNullable()
                .WithColumn(nameof(Machine.IpAddress)).AsString().NotNullable()
                .WithColumn(nameof(Machine.DepartmentId)).AsInt32()
                .WithColumn(nameof(Machine.LineId)).AsInt32()
                .WithColumn(nameof(Machine.ProcessId)).AsInt32()
            ;
        }

        if (!Schema.Table(nameof(UserAccount)).Exists())
        {
            Create
            .Table(nameof(UserAccount))
                .WithColumn(nameof(UserAccount.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserAccount.LoginName)).AsString().NotNullable().Unique()
                .WithColumn(nameof(UserAccount.Password)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.UserName)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.SalfKey)).AsString().NotNullable()
            ;
        }

        if (!Schema.Table(nameof(UserRole)).Exists())
        {
            Create
            .Table(nameof(UserRole))
                .WithColumn(nameof(UserRole.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserRole.RoleName)).AsString().NotNullable().Unique()
            ;
        }

        if (!Schema.Table(nameof(RoleOfUser)).Exists())
        {
            Create
            .Table(nameof(RoleOfUser))
                .WithColumn(nameof(RoleOfUser.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(RoleOfUser.UserId)).AsInt32()
                .WithColumn(nameof(RoleOfUser.RoleId)).AsInt32()
            ;
        }

        if (!Schema.Table(nameof(UserRight)).Exists())
        {
            Create
            .Table(nameof(UserRight))
                .WithColumn(nameof(UserRight.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserRight.RoleId)).AsInt32()
                .WithColumn(nameof(UserRight.ControllerName)).AsString()
                .WithColumn(nameof(UserRight.ActionName)).AsString()
            ;
        }

        if (!Schema.Table(nameof(Setting)).Exists())
        {
            Create
            .Table(nameof(Setting))
                .WithColumn(nameof(Setting.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Setting.Name)).AsString().NotNullable()
                .WithColumn(nameof(Setting.Value)).AsString().NotNullable()
            ;
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
        var department = new List<Factory>{
            new Factory { FactoryName = "NhatTinh"},
        };
        _linq2Db.BulkInsert(department).Wait();

        var userAccount = new List<UserAccount>{
            new UserAccount { LoginName = "admin", Password = "MM08+DTe5SgJ/abCwZW2oRlH+g8mO1XyQxStxGwTetI=", SalfKey = "YSguanX0gfFpM9t6Cn711Q==", UserName = "QuanTM" },
        };
        _linq2Db.BulkInsert(userAccount).Wait();

        var userRole = new List<UserRole>{
            new UserRole{RoleName = "Administrator"},
            new UserRole{RoleName = "Operator"},
            new UserRole{RoleName = "View"},
        };
        _linq2Db.BulkInsert(userRole).Wait();

        var roleOfUser = new List<RoleOfUser>{
            new RoleOfUser{UserId = 1, RoleId = 1},
            new RoleOfUser{UserId = 1, RoleId = 2},
            new RoleOfUser{UserId = 1, RoleId = 3},
        };
        _linq2Db.BulkInsert(roleOfUser).Wait();

        var userRight = new List<UserRight>{
            new UserRight{RoleId = 1, ControllerName = "*", ActionName = "*"},
            new UserRight{RoleId = 2, ControllerName = "*", ActionName = "Index"},
            new UserRight{RoleId = 3, ControllerName = "*", ActionName = "Index"},
        };
        _linq2Db.BulkInsert(userRight).Wait();

        var settings = new List<Setting>{
            new Setting{Name = "GetApiTimeOut", Value = "1000"},
            new Setting{Name = "PostFileTimeOut", Value = "1000"},
            new Setting{Name = "PostApiTimeOut", Value = "1000"},
            new Setting{Name = "FileDataDirectory", Value = "/app/FileData/"},
            new Setting{Name = "NodeListEspFile", Value = "//table[@class='fixed']/tbody/tr"},
            new Setting{Name = "NodeServerState", Value = "//p[@id='result']"},
            new Setting{Name = "UploadFileFormat", Value = "http://{0}/upload/VDATA/{1}"},
            new Setting{Name = "ChangeMachineStateFormat", Value = "http://{0}/selectedMachine"},
            new Setting{Name = "ChangeServerStateFormat", Value = "http://{0}/selectedServer"},
            new Setting{Name = "ChangeStateDelay", Value = "500"},
            new Setting{Name = "DeleteFileFormat", Value = "http://{0}/delete/VDATA/{1}"},
            new Setting{Name = "PostFileFormat", Value = "http://{0}/upload/VDATA/{1}"},
            new Setting{Name = "GetListFileFormat", Value = "http://{0}/VDATA"},
            new Setting{Name = "LineFormat", Value = "Line {0}"},
            new Setting{Name = "StationFormat", Value = "Station {0}"},
            new Setting{Name = "PatternFormat", Value = "Pattern {0}"},
            new Setting{Name = "ProductFormat", Value = "Product {0}"},

        };
        _linq2Db.BulkInsert(settings).Wait();
    }
}

