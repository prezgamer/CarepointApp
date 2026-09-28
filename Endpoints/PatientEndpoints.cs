using CarePointApp.Data;
using CarePointApp.Models;
using CarePointApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Endpoints;

public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this WebApplication app)
    {
        app.MapGet("/patients", async (CarePointDataContext db) => 
            await db.patients.
            Include(p => p.clinicalStatus).
            Include(p => p.assignedDoctor).
            ToListAsync());

        app.MapGet("/patients/{id:int}", async (int id,CarePointDataContext db) =>
        {
            var patient = await db.patients
                .Include(p => p.clinicalStatus)
                .Include(p => p.assignedDoctor)
                .FirstOrDefaultAsync(p => p.id == id);

            return patient is null ? Results.NotFound($"No patients found matching of id of {id}.") : Results.Ok($"Patient is found: {patient}");
        });

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
