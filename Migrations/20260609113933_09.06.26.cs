using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EBookStore.Migrations
{
    /// <inheritdoc />
    public partial class _090626 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionRoutes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Controller = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionRoutes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    ActionRouteID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RolePermissions_ActionRoutes_ActionRouteID",
                        column: x => x.ActionRouteID,
                        principalTable: "ActionRoutes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolePermissions_UserTypes_RoleID",
                        column: x => x.RoleID,
                        principalTable: "UserTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionRoutes_Controller_Action_Method",
                table: "ActionRoutes",
                columns: new[] { "Controller", "Action", "Method" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_ActionRouteID_RoleID",
                table: "RolePermissions",
                columns: new[] { "ActionRouteID", "RoleID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleID",
                table: "RolePermissions",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "ActionRoutes");
        }
    }
}
