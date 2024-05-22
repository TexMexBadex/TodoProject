using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class AddReminderToModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tasks",
                newName: "Content");

            migrationBuilder.AddColumn<DateTime>(
                name: "Reminder",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reminder",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Tasks",
                newName: "Title");
        }
    }
}
