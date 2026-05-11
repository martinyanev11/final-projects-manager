using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialisations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "Students",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SpecialisationId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Specialisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialisations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_SpecialisationId",
                table: "Students",
                column: "SpecialisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Specialisations_SpecialisationId",
                table: "Students",
                column: "SpecialisationId",
                principalTable: "Specialisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Specialisations_SpecialisationId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "Specialisations");

            migrationBuilder.DropIndex(
                name: "IX_Students_SpecialisationId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SpecialisationId",
                table: "Students");
        }
    }
}
