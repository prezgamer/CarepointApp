using CarePointApp.Data;
using CarePointApp.Models;
using CarePointApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Endpoints;

public static class DoctorEndpoints
{
    public static void MapDoctorEndpoints(this WebApplication app)
    {
        // Endpoint to get all doctors with their specialities
        app.MapGet("/doctors", async (CarePointDataContext db) => 
            await db.doctors.Include(d => d.speciality).ToListAsync());

        // Endpoint to get a specific doctor by ID with their speciality
        app.MapGet("/doctors/{id:int}", async (int id,CarePointDataContext db) =>
        {
            var doctor = await db.doctors.FirstOrDefaultAsync(p => 
            p.id == id);

            return doctor is null ? Results.NotFound($"No clinical status found matching of id of {id}.") : 
            Results.Ok($"Clinical Status is found: {doctor}");
        });

        // Endpoint to create a new doctor
        app.MapPost("/doctors", async (CreateDoctorDto dto, CarePointDataContext db) =>
        {
        
            if (string.IsNullOrWhiteSpace(dto.doctorName) ||
                string.IsNullOrWhiteSpace(dto.doctorPhoneNumber) ||
                dto.doctorSpecialityId is null or <= 0)
            {
                return Results.BadRequest("Doctor name, phone number and speciality are all required.");
            }

            
            var specialityExists = await db.doctorSpecialities
                .AnyAsync(s => s.doctorSpecialityId == dto.doctorSpecialityId);
            if (!specialityExists)
                return Results.BadRequest($"No speciality with id {dto.doctorSpecialityId}");


            var doctor = new Doctor
            {
                name = dto.doctorName,
                phoneNumber = dto.doctorPhoneNumber,
                doctorSpecialityId = dto.doctorSpecialityId.Value   // safe: null was rejected above
            };

            db.doctors.Add(doctor);
            await db.SaveChangesAsync();


            return Results.Created($"/doctors/{doctor.id}", new { doctor.id, doctor.name });
        });

        // Endpoint to update an existing doctor
        app.MapPut("/doctors/{id:int}", async (int id, UpdateDoctorDto dto, CarePointDataContext db) =>
        {
            if (string.IsNullOrWhiteSpace(dto.doctorName) ||
                string.IsNullOrWhiteSpace(dto.doctorPhoneNumber))
            {
                return Results.BadRequest("Doctor name and phone number are required.");
            }

            var doctor = await db.doctors.FindAsync(id);
            if (doctor is null)
                return Results.NotFound($"No doctor found with id {id}, therefore cannot update.");

            if (dto.doctorSpecialityId is not null)
            {
                var specialityExists = await db.doctorSpecialities
                    .AnyAsync(s => s.doctorSpecialityId == dto.doctorSpecialityId);
                if (!specialityExists)
                    return Results.BadRequest($"No speciality with id {dto.doctorSpecialityId}");

                doctor.doctorSpecialityId = dto.doctorSpecialityId.Value;
            }

            doctor.name = dto.doctorName;
            doctor.phoneNumber = dto.doctorPhoneNumber;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Endpoint to delete a doctor
        app.MapDelete("/doctors/{id:int}", async (int id, CarePointDataContext db) =>
        {
            var doctor = await db.doctors.FindAsync(id);
            if (doctor is null)
            {
                return Results.NotFound("No Doctor is found");
            }

            db.doctors.Remove(doctor);
            await db.SaveChangesAsync();  
            return Results.NoContent();
        });
    }
}
