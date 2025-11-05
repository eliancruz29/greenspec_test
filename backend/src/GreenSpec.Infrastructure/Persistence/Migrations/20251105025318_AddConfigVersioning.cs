using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenSpec.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add CreatedAt column with default value set to UpdatedAt for existing rows
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Configs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            // Add IsActive column with default value of true for existing rows
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Configs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Update CreatedAt to match UpdatedAt for existing records (best approximation)
            migrationBuilder.Sql("UPDATE Configs SET CreatedAt = UpdatedAt WHERE CreatedAt = '0001-01-01'");

            // Create indexes for efficient querying
            migrationBuilder.CreateIndex(
                name: "IX_Configs_IsActive",
                table: "Configs",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Configs_IsActive_CreatedAt",
                table: "Configs",
                columns: new[] { "IsActive", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Configs_IsActive",
                table: "Configs");

            migrationBuilder.DropIndex(
                name: "IX_Configs_IsActive_CreatedAt",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Configs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Configs");
        }
    }
}
