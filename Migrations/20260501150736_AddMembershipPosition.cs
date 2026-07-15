using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsConnect1._0.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Memberships",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Memberships");
        }
    }
}
