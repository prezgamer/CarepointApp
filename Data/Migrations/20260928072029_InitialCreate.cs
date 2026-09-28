using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clinicalStatuses",
                columns: table => new
                {
                    clinicalStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clinicalStatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinicalStatuses", x => x.clinicalStatusId);
                });

            migrationBuilder.CreateTable(
                name: "doctorSpecialities",
                columns: table => new
                {
                    doctorSpecialityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doctorSpecialityType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctorSpecialities", x => x.doctorSpecialityId);
                });

            migrationBuilder.CreateTable(
                name: "doctors",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    specialitydoctorSpecialityId = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctors", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctors_doctorSpecialities_specialitydoctorSpecialityId",
                        column: x => x.specialitydoctorSpecialityId,
                        principalTable: "doctorSpecialities",
                        principalColumn: "doctorSpecialityId");
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nric = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bookInDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    bookOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    clinicalStatusId = table.Column<int>(type: "int", nullable: false),
                    doctorId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patients", x => x.id);
                    table.ForeignKey(
                        name: "FK_patients_clinicalStatuses_clinicalStatusId",
                        column: x => x.clinicalStatusId,
                        principalTable: "clinicalStatuses",
                        principalColumn: "clinicalStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_patients_doctors_doctorId",
                        column: x => x.doctorId,
                        principalTable: "doctors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctors_specialitydoctorSpecialityId",
                table: "doctors",
                column: "specialitydoctorSpecialityId");

            migrationBuilder.CreateIndex(
                name: "IX_patients_clinicalStatusId",
                table: "patients",
                column: "clinicalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_patients_doctorId",
                table: "patients",
                column: "doctorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "clinicalStatuses");

            migrationBuilder.DropTable(
                name: "doctors");

            migrationBuilder.DropTable(
                name: "doctorSpecialities");
        }
    }
}
