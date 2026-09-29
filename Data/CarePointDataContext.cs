using CarePointApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Data;


public class CarePointDataContext(DbContextOptions<CarePointDataContext> options) : DbContext(options)
{
    // Define DbSet properties for each entity
    public DbSet<Patient> patients => Set<Patient>();
    public DbSet<ClinicalStatus> clinicalStatuses => Set<ClinicalStatus>();
    public DbSet<Doctor> doctors => Set<Doctor>();
    public DbSet<DoctorSpeciality> doctorSpecialities => Set<DoctorSpeciality>();

    // Configure the model relationships 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the relationship between Patient and Doctor
        modelBuilder.Entity<Patient>()
            .HasOne(p => p.assignedDoctor)
            .WithMany()
            .HasForeignKey(p => p.doctorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure the relationship between Doctor and DoctorSpeciality
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.speciality)
            .WithMany()
            .HasForeignKey(d => d.doctorSpecialityId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
