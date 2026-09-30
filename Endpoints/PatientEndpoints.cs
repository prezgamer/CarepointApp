using CarePointApp.Data;
using CarePointApp.Models;
using CarePointApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Endpoints;

public class PatientSearchRow
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string gender { get; set; } = "";
    public string nric { get; set; } = "";
    public string phoneNumber { get; set; } = "";
    public int clinicalStatusId { get; set; }
    public string? clinicalStatusName { get; set; }
    public int? doctorId { get; set; }
    public string? doctorName { get; set; }
    public DateTime? bookInDate { get; set; }
    public DateTime? bookOutDate { get; set; }
}

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this WebApplication app)
    {
        // Endpoint to get all patients with their clinical status and assigned doctor
        app.MapGet("/patients", async (CarePointDataContext db) => 
            await db.patients.
            Include(p => p.clinicalStatus).
            Include(p => p.assignedDoctor).
            ToListAsync());

        // Endpoint to get a specific patient by ID with their clinical status and assigned doctor
        app.MapGet("/patients/{id:int}", async (int id,CarePointDataContext db) =>
        {
            var patient = await db.patients
                .Include(p => p.clinicalStatus)
                .Include(p => p.assignedDoctor)
                .FirstOrDefaultAsync(p => p.id == id);

            return patient is null ? Results.NotFound($"No patients found matching of id of {id}.") : Results.Ok($"Patient is found: {patient}");
        });

        // Endpoint to search for patients by name with their clinical status and assigned doctor
        app.MapGet("/patients/search", async (string? name,CarePointDataContext db) => 
        {
            var patient = db.patients
            .Include(p => p.clinicalStatus)
            .Include(p => p.assignedDoctor)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                patient = patient.Where(p => p.name.Contains(name));
            }

            var results = await patient.ToListAsync();

            if (results.Count == 0)
            {
                return Results.NotFound($"No patients found matching '{name}'.");
            }

            return Results.Ok(await patient.ToListAsync());
        });

        // Endpoint to search for patients by name using a stored procedure 
        // (usp_SearchPatientsByName) with their clinical status and assigned doctor
        app.MapGet("/patients/search-proc", async (string? name, CarePointDataContext db) =>
        {
            var searchTerm = name ?? "";

            var results = await db.Database
                .SqlQueryRaw<PatientSearchRow>("EXEC usp_SearchPatientsByName @Name = {0}", searchTerm)
                .ToListAsync();

            if (results.Count == 0)
                return Results.NotFound($"No patients found matching '{searchTerm}'.");

            return Results.Ok(results);
        });

        // Endpoint to get patients sorted by their doctor using a stored procedure (usp_GetPatientsSortedByDoctor)
        app.MapGet("/patients/sorted-by-doctor", async (string? sortOrder, CarePointDataContext db) =>
        {
            var order = string.IsNullOrWhiteSpace(sortOrder) ? "ASC" : sortOrder.ToUpper();

            if (order != "ASC" && order != "DESC")
                return Results.BadRequest("Invalid sort order. Use 'ASC' or 'DESC'.");

            var results = await db.Database
                .SqlQueryRaw<PatientSearchRow>("EXEC usp_GetPatientsSortedByDoctor @SortOrder = {0}", order)
                .ToListAsync();

            return Results.Ok(results);
        });

        // Endpoint to get patients to be sorted by their name using a stored procedure 
        // (usp_GetPatientsSortedByName) with their clinical status and assigned doctor
        app.MapGet("/patients/sorted-by-name", async (string? sortOrder, CarePointDataContext db) =>
        {
            var order = string.IsNullOrWhiteSpace(sortOrder) ? "ASC" : sortOrder.ToUpper();

            if (order != "ASC" && order != "DESC")
                return Results.BadRequest("Invalid sort order. Use 'ASC' or 'DESC'.");

            var results = await db.Database
                .SqlQueryRaw<PatientSearchRow>("EXEC usp_GetPatientsSortedByName @SortOrder = {0}", order)
                .ToListAsync();

            return Results.Ok(results);
        });

        // Endpoint to get patients sorted by their book-in date using a stored procedure 
        // (usp_GetPatientsSortedByBookIn) with their clinical status and assigned doctor
        app.MapGet("/patients/sorted-by-bookin", async (string? sortOrder, CarePointDataContext db) =>
        {
            var order = string.IsNullOrWhiteSpace(sortOrder) ? "ASC" : sortOrder.ToUpper();

            if (order != "ASC" && order != "DESC")
                return Results.BadRequest("Invalid sort order. Use 'ASC' or 'DESC'.");

            var results = await db.Database
                .SqlQueryRaw<PatientSearchRow>("EXEC usp_GetPatientsSortedByBookIn @SortOrder = {0}", order)
                .ToListAsync();

            return Results.Ok(results);
        });

        // Endpoint to create a new patient
        app.MapPost("/patients", async (CreatePatientDto dto, CarePointDataContext db) =>
        {
            var statusExists = await db.clinicalStatuses.AnyAsync(s => s.clinicalStatusId == dto.clinicalStatusId);
            
            if (string.IsNullOrWhiteSpace(dto.patientName) ||
                string.IsNullOrWhiteSpace(dto.patientGender) ||
                string.IsNullOrWhiteSpace(dto.patientPhoneNumber) ||
                string.IsNullOrWhiteSpace(dto.patientNRIC))
            {
                return Results.BadRequest("Patient name, gender, phone number, NRIC are all required.");
            }

            if (!statusExists)
            {
                return Results.BadRequest($"No Clinical Status with id {dto.clinicalStatusId}");
            }

            var patient = new Patient
            {
                name = dto.patientName,
                gender = dto.patientGender,
                nric = dto.patientNRIC,
                phoneNumber = dto.patientPhoneNumber,
                clinicalStatusId = dto.clinicalStatusId,
                doctorId = dto.doctorId
            };

            db.patients.Add(patient);
            await db.SaveChangesAsync();

            return Results.Created($"/patients/{patient.id}", patient);
        });

        // Endpoint to update an existing patient
        app.MapPut("/patients/{id:int}", async (int id, UpdatePatientDto dto, CarePointDataContext db) =>
        {
            var patient = await db.patients.FindAsync(id);
            if (patient is null)
            {
                return Results.NotFound("Patient is not found, therefore cannot update");
            }

            patient.name = dto.patientName;
            patient.nric = dto.patientNRIC;
            patient.gender = dto.patientGender;
            patient.doctorId = dto.doctorId;
            patient.clinicalStatusId = dto.clinicalStatusId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Endpoint to check in a patient
        app.MapPost("/patients/{id:int}/checkin", async (int id, CheckInDto dto, CarePointDataContext db) =>
        {
            var patient = await db.patients.FindAsync(id);

            if (patient is null)
            {
                return Results.NotFound("Patient is not found, therefore cannot checkin");
            }

            patient.bookInDate = dto.bookInDate;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Endpoint to check out a patient
        app.MapPost("/patients/{id:int}/checkout", async (int id, CheckOutDto dto, CarePointDataContext db) =>
        {
            var patient = await db.patients.FindAsync(id);

            if (patient is null)
            {
                return Results.NotFound("Patient is not found, therefore cannot checkin");
            }

            if (patient.bookInDate is null)
            {
                return Results.BadRequest("Patient have not check in yet");
            }

            patient.bookOutDate = dto.bookOutDate;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Endpoint to delete a patient
        app.MapDelete("/patients/{id:int}", async (int id, CarePointDataContext db) =>
        {
            var patient = await db.patients.FindAsync(id);
            if (patient is null)
            {
                return Results.NotFound("Patient is not found");
            }

            db.patients.Remove(patient);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}