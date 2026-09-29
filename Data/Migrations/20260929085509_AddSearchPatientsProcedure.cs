using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchPatientsProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE usp_SearchPatientsByName
                    @Name NVARCHAR(100)
                AS
                BEGIN
                SET NOCOUNT ON;

                SELECT
                    p.id,
                    p.name,
                    p.gender,
                    p.nric,
                    p.phoneNumber,
                    p.clinicalStatusId,
                    c.clinicalStatusName,
                    p.doctorId,
                    d.name AS doctorName,
                    p.bookInDate,
                    p.bookOutDate
                FROM patients p
                LEFT JOIN clinicalStatuses c ON c.clinicalStatusId = p.clinicalStatusId
                LEFT JOIN doctors d ON d.id = p.doctorId
                WHERE p.name LIKE '%' + @Name + '%'
                ORDER BY p.name;
            END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_SearchPatientsByName");
        }
    }
}
