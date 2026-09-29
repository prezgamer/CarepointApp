using CarePointApp.Data;
using CarePointApp.Models;
using CarePointApp.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Endpoints;

public static class DoctorSpecialityEndpoints
{
    public static void MapDoctorSpecialityEndpoints(this WebApplication app)
    {
        // Endpoint to get all doctor specialities
        app.MapGet("/doctorspeciality", async (CarePointDataContext db) => 
            await db.doctorSpecialities.ToListAsync());
        
        // Endpoint to get a specific doctor speciality by ID
        app.MapGet("/doctorspeciality/{id:int}", async (int id,CarePointDataContext db) =>
        {
            var doctorSpeciality = await db.doctorSpecialities.FirstOrDefaultAsync(p => 
            p.doctorSpecialityId == id);

            return doctorSpeciality is null ? Results.NotFound($"No clinical status found matching of id of {id}.") : 
            Results.Ok($"Clinical Status is found: {doctorSpeciality}");
        });

        // Endpoint to create a new doctor speciality
        app.MapPost("/doctorspeciality", async (CreateDoctorSpecialityDto dto, CarePointDataContext db) =>
        {
            var doctorSpeciality = new DoctorSpeciality {doctorSpecialityType = dto.doctorSpecialityType};

            if (string.IsNullOrWhiteSpace(dto.doctorSpecialityType))
            {
                return Results.BadRequest("The speciality type is empty");
            }

            db.doctorSpecialities.Add(doctorSpeciality);

            await db.SaveChangesAsync();

            return Results.Created($"/clinicalstatuses/{doctorSpeciality.doctorSpecialityId}", doctorSpeciality);
        });

        // Endpoint to update an existing doctor speciality
        app.MapDelete("/doctorspeciality/{id:int}", async (int id, CarePointDataContext db) =>
        {
            var doctorSpeciality = await db.doctorSpecialities.FindAsync(id);
            if (doctorSpeciality is null)
            {
                return Results.NotFound();
            }

            db.doctorSpecialities.Remove(doctorSpeciality);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
