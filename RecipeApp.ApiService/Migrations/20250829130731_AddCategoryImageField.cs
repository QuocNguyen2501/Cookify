using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeApp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryImageField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Categories");
        }
    }
}
