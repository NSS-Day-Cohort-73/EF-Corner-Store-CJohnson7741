using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CornerStore.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 3, 14, 16, 51, 632, DateTimeKind.Local).AddTicks(1235));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 3, 14, 16, 51, 632, DateTimeKind.Local).AddTicks(1292));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 3, 11, 16, 38, 76, DateTimeKind.Local).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2,
                column: "PaidOnDate",
                value: new DateTime(2025, 2, 3, 11, 16, 38, 76, DateTimeKind.Local).AddTicks(2236));
        }
    }
}
