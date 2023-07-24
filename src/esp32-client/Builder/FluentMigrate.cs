using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using esp32_client.Domain;
using FluentMigrator;

namespace esp32_client.Builder;

[Migration(20180430122200)]
public class AddLogTable : Migration
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
        if (!Schema.Table(nameof(Patern)).Exists())
        {
            Create
            .Table(nameof(Patern))
                .WithColumn(nameof(Patern.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(Patern.PaternNumber)).AsString()
                .WithColumn(nameof(Patern.FileName)).AsString().NotNullable()
                .WithColumn(nameof(Patern.FileData)).AsString(int.MaxValue)
                .WithColumn(nameof(Patern.Description)).AsString();
        }
    }


    public override void Down()
    { }
}

