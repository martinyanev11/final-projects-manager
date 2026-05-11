using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorAndReviewerToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialisationId",
                table: "Supervisors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewerId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supervisors_SpecialisationId",
                table: "Supervisors",
                column: "SpecialisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ReviewerId",
                table: "Students",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SupervisorId",
                table: "Students",
                column: "SupervisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Supervisors_ReviewerId",
                table: "Students",
                column: "ReviewerId",
                principalTable: "Supervisors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Supervisors_SupervisorId",
                table: "Students",
                column: "SupervisorId",
                principalTable: "Supervisors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supervisors_Specialisations_SpecialisationId",
                table: "Supervisors",
                column: "SpecialisationId",
                principalTable: "Specialisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Supervisors_ReviewerId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Supervisors_SupervisorId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Supervisors_Specialisations_SpecialisationId",
                table: "Supervisors");

            migrationBuilder.DropIndex(
                name: "IX_Supervisors_SpecialisationId",
                table: "Supervisors");

            migrationBuilder.DropIndex(
                name: "IX_Students_ReviewerId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SupervisorId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SpecialisationId",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "ReviewerId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Students");
        }
    }
}
