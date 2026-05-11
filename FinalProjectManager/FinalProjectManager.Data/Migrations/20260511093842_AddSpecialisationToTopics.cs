using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialisationToTopics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialisationId",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SpecialisationId",
                table: "Topics",
                column: "SpecialisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Specialisations_SpecialisationId",
                table: "Topics",
                column: "SpecialisationId",
                principalTable: "Specialisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Specialisations_SpecialisationId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_SpecialisationId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "SpecialisationId",
                table: "Topics");
        }
    }
}
