using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class GetPatientSortedByNameOfPatientProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE usp_GetPatientsSortedByName
                @SortOrder NVARCHAR(4) = 'ASC'   -- 'ASC' or 'DESC'
            AS
            BEGIN
                SET NOCOUNT ON;

                IF @SortOrder = 'DESC'
                BEGIN
                    SELECT p.id, p.name, p.gender, p.nric, p.phoneNumber,
                        p.clinicalStatusId, c.clinicalStatusName,
                        p.doctorId, d.name AS doctorName,
                        p.bookInDate, p.bookOutDate
                    FROM patients p
                    LEFT JOIN clinicalStatuses c ON c.clinicalStatusId = p.clinicalStatusId
                    LEFT JOIN doctors d ON d.id = p.doctorId
                    ORDER BY p.name DESC;
                END
                ELSE
                BEGIN
                    SELECT p.id, p.name, p.gender, p.nric, p.phoneNumber,
                        p.clinicalStatusId, c.clinicalStatusName,
                        p.doctorId, d.name AS doctorName,
                        p.bookInDate, p.bookOutDate
                    FROM patients p
                    LEFT JOIN clinicalStatuses c ON c.clinicalStatusId = p.clinicalStatusId
                    LEFT JOIN doctors d ON d.id = p.doctorId
                    ORDER BY p.name ASC;
                END
            END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_GetPatientsSortedByName");
        }
    }
}
