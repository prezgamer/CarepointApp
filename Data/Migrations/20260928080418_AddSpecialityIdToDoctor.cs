using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialityIdToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_doctorSpecialities_specialitydoctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropIndex(
                name: "IX_doctors_specialitydoctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropColumn(
                name: "specialitydoctorSpecialityId",
                table: "doctors");

            migrationBuilder.AddColumn<int>(
                name: "doctorSpecialityId",
                table: "doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_doctors_doctorSpecialityId",
                table: "doctors",
                column: "doctorSpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors",
                column: "doctorSpecialityId",
                principalTable: "doctorSpecialities",
                principalColumn: "doctorSpecialityId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropIndex(
                name: "IX_doctors_doctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropColumn(
                name: "doctorSpecialityId",
                table: "doctors");

            migrationBuilder.AddColumn<int>(
                name: "specialitydoctorSpecialityId",
                table: "doctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctors_specialitydoctorSpecialityId",
                table: "doctors",
                column: "specialitydoctorSpecialityId");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_doctorSpecialities_specialitydoctorSpecialityId",
                table: "doctors",
                column: "specialitydoctorSpecialityId",
                principalTable: "doctorSpecialities",
                principalColumn: "doctorSpecialityId");
        }
    }
}
