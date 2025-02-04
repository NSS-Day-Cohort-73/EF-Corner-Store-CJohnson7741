using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornerStore.Migrations
{
    /// <inheritdoc />
    public partial class FixProductModel2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 4, 14, 30, 47, 551, DateTimeKind.Local).AddTicks(6294));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 4, 14, 30, 47, 551, DateTimeKind.Local).AddTicks(6353));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 4, 10, 21, 51, 94, DateTimeKind.Local).AddTicks(8828));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 4, 10, 21, 51, 94, DateTimeKind.Local).AddTicks(8885));
        }
    }
}
