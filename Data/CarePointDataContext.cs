using CarePointApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarePointApp.Data;


public class CarePointDataContext(DbContextOptions<CarePointDataContext> options) : DbContext(options)
{
    public DbSet<Patient> patients => Set<Patient>();
    public DbSet<ClinicalStatus> clinicalStatuses => Set<ClinicalStatus>();
    public DbSet<Doctor> doctors => Set<Doctor>();
    public DbSet<DoctorSpeciality> doctorSpecialities => Set<DoctorSpeciality>();
}
