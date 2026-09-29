using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class DoctorAndSpecialitySetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_doctors_doctorId",
                table: "patients");

            migrationBuilder.AlterColumn<int>(
                name: "doctorId",
                table: "patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "doctorSpecialityId",
                table: "doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors",
                column: "doctorSpecialityId",
                principalTable: "doctorSpecialities",
                principalColumn: "doctorSpecialityId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_doctors_doctorId",
                table: "patients",
                column: "doctorId",
                principalTable: "doctors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_patients_doctors_doctorId",
                table: "patients");

            migrationBuilder.AlterColumn<int>(
                name: "doctorId",
                table: "patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "doctorSpecialityId",
                table: "doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_doctorSpecialities_doctorSpecialityId",
                table: "doctors",
                column: "doctorSpecialityId",
                principalTable: "doctorSpecialities",
                principalColumn: "doctorSpecialityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_patients_doctors_doctorId",
                table: "patients",
                column: "doctorId",
                principalTable: "doctors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
