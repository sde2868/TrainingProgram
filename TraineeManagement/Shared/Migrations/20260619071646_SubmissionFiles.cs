using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 19, 7, 16, 45, 408, DateTimeKind.Utc).AddTicks(5507), "$2a$11$rP6kpqxCXRiQG3Oh1A5i3u9aMLORbuf4lUR/G13BTqAGQ6AnM1hQG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 19, 7, 10, 23, 926, DateTimeKind.Utc).AddTicks(7457), "$2a$11$fOrTaV2a4f4KN/S79d/E...ZgvNeNJ76QE9RgX1E03AXn2hS10dKW" });
        }
    }
}
