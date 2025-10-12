using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsProfilePublicColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ISPROFILEPUBLIC",
                table: "USERS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ISPROFILEPUBLIC",
                table: "USERS",
                type: "NVARCHAR2(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
