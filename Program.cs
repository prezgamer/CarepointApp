using CarePointApp;
using CarePointApp.Data;
using CarePointApp.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// var connString = "Server=(localdb)\\MSSQLLocalDB;Database=CarePointApp;Trusted_Connection=True;TrustServerCertificate=True;";
// builder.Services.AddSqlServer<CarePointDataContext>(connString);

var connString = "Server=.\\SQLEXPRESS;Database=CarePointApp;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddSqlServer<CarePointDataContext>(connString);

var app = builder.Build();

// Development only: apply migrations, then fill an empty database
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CarePointDataContext>();
    await db.Database.MigrateAsync();   // replaces running "dotnet ef database update" by hand
    await DbSeeder.SeedAsync(db);
}
// Development only: Setup a debug endpoint to run arbitrary SQL queries against the database
if (app.Environment.IsDevelopment())
{
    // For Checking the database connection string and other details, for debugging purposes
    app.MapGet("/debug/query", async (string sql, CarePointDataContext db) =>
    {
        var results = new List<Dictionary<string, object?>>();
        var connection = db.Database.GetDbConnection();

        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            results.Add(row);
        }

        return Results.Ok(results);
    });
}

app.UseDefaultFiles(); // Serve index.html as the default file
app.UseStaticFiles();  // Serve static files from wwwroot folder

app.UseCors("AllowAll"); // Will not be AllowAll during Prod, only selected addresses

app.MapClinicalStatusEndpoints(); // Patient Clinical Statuses Endpoints API
app.MapDoctorSpecialityEndpoints(); // Doctor Speciality Endpoints API
app.MapDoctorEndpoints(); // Doctor Endpoints API
app.MapPatientEndpoints(); // Patient Endpoints API

app.Run();
