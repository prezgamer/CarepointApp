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

var connString = "Server=(localdb)\\MSSQLLocalDB;Database=CarePointApp;Trusted_Connection=True;TrustServerCertificate=True;";
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

app.MapGet("/", () => "Hello World!");
app.UseCors("AllowAll"); // Will not be AllowAll during Prod, only selected addresses

app.MapClinicalStatusEndpoints(); // Patient Clinical Statuses Endpoints API
app.MapDoctorSpecialityEndpoints(); // Doctor Speciality Endpoints API
app.MapDoctorEndpoints(); // Doctor Endpoints API
app.MapPatientEndpoints(); // Patient Endpoints API

app.Run();
