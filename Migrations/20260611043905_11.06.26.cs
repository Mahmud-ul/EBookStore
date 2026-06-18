using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBookStore.Migrations
{
    /// <inheritdoc />
    public partial class _110626 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActionRoutes_Controller_Action_Method",
                table: "ActionRoutes");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "ActionRoutes");

            migrationBuilder.CreateIndex(
                name: "IX_ActionRoutes_Controller_Action",
                table: "ActionRoutes",
                columns: new[] { "Controller", "Action" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActionRoutes_Controller_Action",
                table: "ActionRoutes");

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "ActionRoutes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ActionRoutes_Controller_Action_Method",
                table: "ActionRoutes",
                columns: new[] { "Controller", "Action", "Method" },
                unique: true);
        }
    }
}
