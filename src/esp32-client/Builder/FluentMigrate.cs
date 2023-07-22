using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using esp32_client.Domain;
using FluentMigrator;

namespace esp32_client.Builder;

[Migration(20180430121800)]
public class AddLogTable : Migration
{
    public override void Up()
    {
        if (!Schema.Table(nameof(UserAccount)).Exists())
        {
            Create
            .Table(nameof(UserAccount))
                .WithColumn(nameof(UserAccount.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(UserAccount.LoginName)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.Password)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.UserName)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.SalfKey)).AsString().NotNullable()
                .WithColumn(nameof(UserAccount.RoleId)).AsInt32().Nullable();
        }
    }

    public override void Down()
    { }
}

