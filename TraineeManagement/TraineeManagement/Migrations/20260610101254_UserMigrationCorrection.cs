using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Migrations
{
    /// <inheritdoc />
    public partial class UserMigrationCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 10, 10, 12, 53, 385, DateTimeKind.Utc).AddTicks(67), "$2a$11$6XOJDJyyiIrjv4Jd4MWwA.iaRnChhF2S6bY0zzQjwHs6K5p.hOu9e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 6, 10, 9, 16, 41, 54, DateTimeKind.Utc).AddTicks(2293), "$2a$11$yMvsCUzWSPuicbc6E3mFbem69GzpuQxK0o26vVacXNXzSvwE0Hp7W" });
        }
    }
}
