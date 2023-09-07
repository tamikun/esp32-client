using esp32_client.Domain;
using FluentMigrator;

namespace esp32_client.Builder;

[Migration(20180430122503)]
public class AddTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(Factory)).Exists())
        {
            Create
            .Table(nameof(Factory))
                .WithColumn(nameof(Factory.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Factory.FactoryNo)).AsString().NotNullable().Unique()
                .WithColumn(nameof(Factory.FactoryName)).AsString().NotNullable()
            ;
        }

        if (!Schema.Table(nameof(Line)).Exists())
        {
            Create
            .Table(nameof(Line))
                .WithColumn(nameof(Line.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Line.FactoryId)).AsInt32()
                .WithColumn(nameof(Line.LineNo)).AsString().NotNullable()
                .WithColumn(nameof(Line.LineName)).AsString()
                .WithColumn(nameof(Line.ProductId)).AsInt32()
            ;

            Create.UniqueConstraint("UC_Line").OnTable(nameof(Line)).Columns(nameof(Line.FactoryId), nameof(Line.LineNo));
        }

        if (!Schema.Table(nameof(Station)).Exists())
        {
            Create
            .Table(nameof(Station))
                .WithColumn(nameof(Station.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Station.LineId)).AsInt32()
                .WithColumn(nameof(Station.StationNo)).AsString().NotNullable()
                .WithColumn(nameof(Station.StationName)).AsString()
                .WithColumn(nameof(Station.ProcessId)).AsInt32()
            ;

        }

        if (!Schema.Table(nameof(Product)).Exists())
        {
            Create
            .Table(nameof(Product))
                .WithColumn(nameof(Product.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Product.FactoryId)).AsInt32()
                .WithColumn(nameof(Product.ProductNo)).AsString().NotNullable()
                .WithColumn(nameof(Product.ProductName)).AsString()
            ;

            Create.UniqueConstraint("UC_Product").OnTable(nameof(Product)).Columns(nameof(Product.FactoryId), nameof(Product.ProductNo));
        }

        if (!Schema.Table(nameof(Process)).Exists())
        {
            Create
            .Table(nameof(Process))
                .WithColumn(nameof(Process.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Process.ProductId)).AsInt32()
                .WithColumn(nameof(Process.ProcessName)).AsString().Nullable()
                .WithColumn(nameof(Process.ProcessNo)).AsString()
                .WithColumn(nameof(Process.PatternNo)).AsString()
                .WithColumn(nameof(Process.PatternDirectory)).AsString().Nullable()
                .WithColumn(nameof(Process.OperationData)).AsString().Nullable()
                .WithColumn(nameof(Process.COAttachment)).AsString().Nullable()
                .WithColumn(nameof(Process.Description)).AsString().Nullable()
            ;
        }

        if (!Schema.Table(nameof(Machine)).Exists())
        {
            Create
            .Table(nameof(Machine))
                .WithColumn(nameof(Machine.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Machine.MachineNo)).AsString().NotNullable()
                .WithColumn(nameof(Machine.MachineName)).AsString()
                .WithColumn(nameof(Machine.IpAddress)).AsString().NotNullable().Unique()
                .WithColumn(nameof(Machine.FactoryId)).AsInt32()
                .WithColumn(nameof(Machine.LineId)).AsInt32()
                .WithColumn(nameof(Machine.StationId)).AsInt32()
                .WithColumn(nameof(Machine.COPartNo)).AsString()
                .WithColumn(nameof(Machine.CncMachine)).AsBoolean().NotNullable()
                .WithColumn(nameof(Machine.UpdateFirmwareSucess)).AsBoolean().NotNullable()
            ;

            Create.UniqueConstraint("UC_Machine").OnTable(nameof(Machine)).Columns(nameof(Machine.FactoryId), nameof(Machine.MachineNo));
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
                .WithColumn(nameof(Setting.Description)).AsString(100).Nullable()
                .WithColumn(nameof(Setting.EnableEditing)).AsBoolean().NotNullable()
            ;
        }

        if (!Schema.Table(nameof(ScheduleTask)).Exists())
        {
            Create
            .Table(nameof(ScheduleTask))
                .WithColumn(nameof(ScheduleTask.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(ScheduleTask.Name)).AsString().NotNullable()
                .WithColumn(nameof(ScheduleTask.Seconds)).AsInt32()
                .WithColumn(nameof(ScheduleTask.Method)).AsString().NotNullable()
                .WithColumn(nameof(ScheduleTask.Enabled)).AsBoolean().NotNullable()
                .WithColumn(nameof(ScheduleTask.LastStartUtc)).AsDateTime2().Nullable()
                .WithColumn(nameof(ScheduleTask.LastSuccessUtc)).AsDateTime2().Nullable()
            ;
        }

        if (!Schema.Table(nameof(DataReport)).Exists())
        {
            Create
            .Table(nameof(DataReport))
                .WithColumn(nameof(DataReport.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(DataReport.StationId)).AsInt32()
                .WithColumn(nameof(DataReport.ProductNumber)).AsInt32()
                .WithColumn(nameof(DataReport.DateTimeUtc)).AsDateTime2().NotNullable()
            ;
        }

        if (!Schema.Table(nameof(Log)).Exists())
        {
            Create
            .Table(nameof(Log))
                .WithColumn(nameof(Log.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Log.Message)).AsString()
                .WithColumn(nameof(Log.FullMessage)).AsString(int.MaxValue)
                .WithColumn(nameof(Log.DateTimeUtc)).AsDateTime2().NotNullable()
            ;
        }

        if (!Schema.Table(nameof(UserSession)).Exists())
        {
            Create
            .Table(nameof(UserSession))
                .WithColumn(nameof(UserSession.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserSession.UserId)).AsInt32()
                .WithColumn(nameof(UserSession.Token)).AsString(500).Unique()
                .WithColumn(nameof(UserSession.ExpiredTime)).AsDateTime2()
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
            new Factory { FactoryName = "Juki", FactoryNo = "Factory 001"},
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
            new Setting{Name = "GetApiTimeOut", Value = "1000", EnableEditing = true},
            new Setting{Name = "PostFileTimeOut", Value = "1000", EnableEditing = true},
            new Setting{Name = "PostApiTimeOut", Value = "1000", EnableEditing = true},
            new Setting{Name = "FileDataDirectory", Value = "/app/FileData/", EnableEditing = true},
            new Setting{Name = "NodeListEspFile", Value = "//table[@class='fixed']/tbody/tr", EnableEditing = true},
            new Setting{Name = "UploadFileFormat", Value = "http://{0}/upload/VDATA/{1}", EnableEditing = true},
            new Setting{Name = "ChangeMachineStateFormat", Value = "http://{0}/selectedMachine", EnableEditing = true},
            new Setting{Name = "ChangeServerStateFormat", Value = "http://{0}/selectedServer", EnableEditing = true},
            new Setting{Name = "GetProductNumberFormat", Value = "http://{0}/data_prod", EnableEditing = true},
            new Setting{Name = "ChangeStateDelay", Value = "500", EnableEditing = true},
            new Setting{Name = "DeleteFileFormat", Value = "http://{0}/delete/VDATA/{1}", EnableEditing = true},
            new Setting{Name = "PostFileFormat", Value = "http://{0}/upload/VDATA/{1}", EnableEditing = true},
            new Setting{Name = "GetListFileFormat", Value = "http://{0}/VDATA", EnableEditing = true},
            new Setting{Name = "ResetMachineFormat", Value = "http://{0}/data_reset", EnableEditing = true},
            new Setting{Name = "StationFormat", Value = "Station {0}", EnableEditing = true},
            new Setting{Name = "MinCharStationFormat", Value = "3", EnableEditing = true},
            new Setting{Name = "ProductFormat", Value = "Product {0}", EnableEditing = true},
            new Setting{Name = "MinCharProductFormat", Value = "3", EnableEditing = true},
            new Setting{Name = "LineFormat", Value = "Line {0}", EnableEditing = true},
            new Setting{Name = "MinCharLineFormat", Value = "3", EnableEditing = true},
            new Setting{Name = "FactoryFormat", Value = "Factory {0}", EnableEditing = true},
            new Setting{Name = "MinCharFactoryFormat", Value = "3", EnableEditing = true},
            new Setting{Name = "MachineFormat", Value = "Machine {0}", EnableEditing = true},
            new Setting{Name = "MinCharMachineFormat", Value = "3", EnableEditing = true},
            new Setting{Name = "GetApiProductNumberTimeOut", Value = "5000", EnableEditing = true},
            new Setting{Name = "AcceptedFile", Value = ".VDT,.PMD", EnableEditing = true},
            new Setting{Name = "DeleteOnUploadingEmptyFile", Value = "false", EnableEditing = true},
            new Setting{Name = "ReloadMonitoringMilliseconds", Value = "5000", EnableEditing = true},
            new Setting{Name = "ReloadMonitoringBatchSize", Value = "20", EnableEditing = true},
            new Setting{Name = "EnableLog", Value = "false", EnableEditing = true},
            new Setting{Name = "MachineFirmwareFilePath", Value = "/app/FileData/operate_app.bin", EnableEditing = true},
            new Setting{Name = "DefaultNewMachineIp", Value = "192.168.1.99", EnableEditing = true},
        };

        _linq2Db.BulkInsert(settings).Wait();

        // var scheduleTask = new List<ScheduleTask>{
        //     new ScheduleTask{
        //         Name = "Save product data",
        //         Seconds = 120,
        //         Method = $"{nameof(ScheduleTaskService.SaveProductData)}",
        //         Enabled = true,
        //     },

        // };

        // _linq2Db.BulkInsert(scheduleTask).Wait();
    }
}

[Migration(20230811162705)]
public class AddTimeOutSetting : AutoReversingMigration
{
    private readonly LinqToDb _linq2Db;
    public AddTimeOutSetting(LinqToDb linq2Db)
    {
        _linq2Db = linq2Db;
    }
    public override void Up()
    {
        var settings = new List<Setting>{
            new Setting{Name = "JwtTokenSecret", Value = "Secret at least 128 bit for HmacSha256 SecurityAlgorithms", EnableEditing = false},
            new Setting{Name = "SessionExpiredTimeInSecond", Value = "36000", EnableEditing = true},
        };

        _linq2Db.BulkInsert(settings).Wait();
    }
}
