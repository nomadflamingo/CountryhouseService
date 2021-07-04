using Microsoft.EntityFrameworkCore.Migrations;

namespace CountryhouseService.Migrations
{
    public partial class add_statusid_and_authorid_columns_to_ads : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Ads",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Ads",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Ads_AuthorId",
                table: "Ads",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_StatusId",
                table: "Ads",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_AdStatuses_StatusId",
                table: "Ads",
                column: "StatusId",
                principalTable: "AdStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_AspNetUsers_AuthorId",
                table: "Ads",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_AdStatuses_StatusId",
                table: "Ads");

            migrationBuilder.DropForeignKey(
                name: "FK_Ads_AspNetUsers_AuthorId",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_AuthorId",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_StatusId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Ads");
        }
    }
}
