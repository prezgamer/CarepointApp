using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarePointApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class GetPatientSortedByDoctorsProcedure : Migration
    {
        // This migration creates a stored procedure named 'usp_GetPatientsSortedByDoctor' 
        // that retrieves patients sorted by their associated doctor's name. The procedure accepts an optional parameter 
        // '@SortOrder' to specify the sorting order (ascending or descending). 
        // If no parameter is provided, it defaults to ascending order. 
        // The procedure joins the 'patients' table with 'clinicalStatuses' and 'doctors' tables to 
        // include relevant information in the result set.
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE OR ALTER PROCEDURE usp_GetPatientsSortedByDoctor
                @SortOrder NVARCHAR(4) = 'ASC'
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
                    ORDER BY d.name DESC;
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
                    ORDER BY d.name ASC;
                END
            END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS usp_GetPatientsSortedByDoctor");
        }
    }
}
