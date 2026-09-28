using CarePointApp.Data;
using CarePointApp.DTOs;
using CarePointApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Endpoints;

public static class ClinicalStatusEndpoints
{
    public static void MapClinicalStatusEndpoints(this WebApplication app)
    {
        app.MapGet("/clinicalStatuses", async (CarePointDataContext db) => 
            await db.clinicalStatuses.ToListAsync());


        app.MapGet("/clinicalStatuses/{id:int}", async (int id,CarePointDataContext db) =>
        {
            var clinicalStatus = await db.clinicalStatuses.FirstOrDefaultAsync(p => 
            p.clinicalStatusId == id);

            return clinicalStatus is null ? Results.NotFound($"No clinical status found matching of id of {id}.") : 
            Results.Ok($"Clinical Status is found: {clinicalStatus}");
        });

        app.MapPost("/clinicalStatuses", async (CreateClinicalStatusDto dto, CarePointDataContext db) =>
        {
            var status = new ClinicalStatus {clinicalStatusName = dto.ClinicalStatusName };

            if (string.IsNullOrWhiteSpace(dto.ClinicalStatusName))
            {
                return Results.BadRequest("Clinical Status is Required");
            }

            db.clinicalStatuses.Add(status);

            await db.SaveChangesAsync();

            return Results.Created($"/clinicalstatuses/{status.clinicalStatusId}", status);
        });

        app.MapDelete("/clinicalstatuses/{id:int}", async (int id, CarePointDataContext db) =>
        {
            var status = await db.clinicalStatuses.FindAsync(id);

            if (status is null)
            {
                return Results.NotFound();
            }

            db.clinicalStatuses.Remove(status);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
