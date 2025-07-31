using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VnptSmsBrandName.Migrations.Master
{
    public partial class AddAuditColToMFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IdOrganization",
                table: "m_history",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "m_file",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreateBy",
                table: "m_file",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IsDeleted",
                table: "m_file",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "m_file",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "m_file",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdOrganization",
                table: "m_history");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "m_file");

            migrationBuilder.DropColumn(
                name: "CreateBy",
                table: "m_file");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "m_file");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "m_file");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "m_file");
        }
    }
}
