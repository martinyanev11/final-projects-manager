using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_TopicId",
                table: "Students",
                column: "TopicId",
                unique: true,
                filter: "[TopicId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Topics_TopicId",
                table: "Students",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Topics_TopicId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_TopicId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Students");
        }
    }
}
