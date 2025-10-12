using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTablesOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 只创建新表，不修改现有表

            migrationBuilder.CreateTable(
                name: "DELIVERY_INFOS",
                columns: table => new
                {
                    DELIVERYINFOID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ADDRESS = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    PHONENUMBER = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    NAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    GENDER = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    ISDEFAULT = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    CUSTOMERID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DELIVERY_INFOS", x => x.DELIVERYINFOID);
                    table.ForeignKey(
                        name: "FK_DELIVERY_INFOS_CUSTOMERS_CUSTOMERID",
                        column: x => x.CUSTOMERID,
                        principalTable: "CUSTOMERS",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DISH_CATEGORIES",
                columns: table => new
                {
                    CATEGORYID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    CATEGORYNAME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISH_CATEGORIES", x => x.CATEGORYID);
                });

            migrationBuilder.CreateTable(
                name: "MENU_DISH_CATEGORY",
                columns: table => new
                {
                    MENUID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CATEGORYID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MENU_DISH_CATEGORY", x => new { x.MENUID, x.CATEGORYID });
                    table.ForeignKey(
                        name: "FK_MENU_DISH_CATEGORY_DISH_CATEGORIES_CATEGORYID",
                        column: x => x.CATEGORYID,
                        principalTable: "DISH_CATEGORIES",
                        principalColumn: "CATEGORYID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MENU_DISH_CATEGORY_MENUS_MENUID",
                        column: x => x.MENUID,
                        principalTable: "MENUS",
                        principalColumn: "MENUID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DELIVERY_INFOS_CUSTOMERID",
                table: "DELIVERY_INFOS",
                column: "CUSTOMERID");

            migrationBuilder.CreateIndex(
                name: "IX_MENU_DISH_CATEGORY_CATEGORYID",
                table: "MENU_DISH_CATEGORY",
                column: "CATEGORYID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DELIVERY_INFOS");

            migrationBuilder.DropTable(
                name: "MENU_DISH_CATEGORY");

            migrationBuilder.DropTable(
                name: "DISH_CATEGORIES");
        }
    }
}
