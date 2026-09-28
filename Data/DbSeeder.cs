using CarePointApp.Data;
using CarePointApp.Models;
using Microsoft.EntityFrameworkCore;
namespace CarePointApp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(CarePointDataContext db)
    {
        // ---------- 1. Clinical statuses ----------
        if (!await db.clinicalStatuses.AnyAsync())
        {
            db.clinicalStatuses.AddRange(
                new ClinicalStatus { clinicalStatusName = "Arrived" },
                new ClinicalStatus { clinicalStatusName = "Late" },
                new ClinicalStatus { clinicalStatusName = "Missed" });
            await db.SaveChangesAsync();
        }
 
        // ---------- 2. Doctor specialities ----------
        if (!await db.doctorSpecialities.AnyAsync())
        {
            db.doctorSpecialities.AddRange(
                new DoctorSpeciality { doctorSpecialityType = "General Practice" },
                new DoctorSpeciality { doctorSpecialityType = "Cardiology" },
                new DoctorSpeciality { doctorSpecialityType = "Paediatrics" },
                new DoctorSpeciality { doctorSpecialityType = "Orthopaedics" },
                new DoctorSpeciality { doctorSpecialityType = "Dermatology" });
            await db.SaveChangesAsync();
        }
 
        // ---------- 3. Doctors ----------
        if (!await db.doctors.AnyAsync())
        {
            // Read the ids back from the database instead of assuming they are 1, 2, 3...
            var specialityIds = await db.doctorSpecialities
                .ToDictionaryAsync(s => s.doctorSpecialityType, s => s.doctorSpecialityId);
 
            db.doctors.AddRange(
                new Doctor { name = "Dr. Lim Wei Jie", phoneNumber = "91110001", doctorSpecialityId = specialityIds["General Practice"] },
                new Doctor { name = "Dr. Priya Nair", phoneNumber = "91110002", doctorSpecialityId = specialityIds["Cardiology"] },
                new Doctor { name = "Dr. Ahmad Rahman", phoneNumber = "91110003", doctorSpecialityId = specialityIds["Paediatrics"] },
                new Doctor { name = "Dr. Chua Mei Ling", phoneNumber = "91110004", doctorSpecialityId = specialityIds["Orthopaedics"] },
                new Doctor { name = "Dr. Koh Jun Wei", phoneNumber = "91110005", doctorSpecialityId = specialityIds["Dermatology"] });
            await db.SaveChangesAsync();
        }
 
        // ---------- 4. Patients (100 fake ones) ----------
        if (!await db.patients.AnyAsync())
        {
            var statusIds = await db.clinicalStatuses
                .ToDictionaryAsync(s => s.clinicalStatusName, s => s.clinicalStatusId);
            var doctorIds = await db.doctors.Select(d => d.id).ToListAsync();
 
            string[] lastNames = { "Tan", "Lim", "Lee", "Ng", "Ong", "Wong", "Goh", "Chua", "Koh", "Teo" };
            string[] firstNames = { "Wei Ming", "Hui Min", "Jia Hui", "Xiu Ling", "Kai Xin",
                                    "Mei Ling", "Jun Wei", "Li Ting", "Zi Xuan", "Rui En" };
            const string nricLetters = "ABCDEFGHIZJ";
 
            var rng = new Random(42); // fixed seed = the same 100 patients every time
            var patients = new List<Patient>();
 
            for (int i = 1; i <= 100; i++)
            {
                // About 70% Arrived, 20% Late, 10% Missed
                var roll = rng.NextDouble();
                string status = roll < 0.7 ? "Arrived" : roll < 0.9 ? "Late" : "Missed";
 
                // Missed patients never came in, so both dates stay null
                DateTime? bookIn = null;
                DateTime? bookOut = null;
                if (status != "Missed")
                {
                    bookIn = DateTime.Today
                        .AddDays(-rng.Next(0, 21))
                        .AddHours(8 + rng.Next(0, 9))
                        .AddMinutes(rng.Next(0, 4) * 15);
 
                    // About 15% have checked in but not out yet
                    if (rng.NextDouble() > 0.15)
                        bookOut = bookIn.Value.AddMinutes(rng.Next(15, 61));
                }
 
                patients.Add(new Patient
                {
                    name = $"{lastNames[rng.Next(lastNames.Length)]} {firstNames[rng.Next(firstNames.Length)]}",
                    gender = rng.Next(2) == 0 ? "Male" : "Female",
                    // i * 137 makes every NRIC unique, so a unique index on nric won't clash
                    nric = $"S{1000000 + i * 137:D7}{nricLetters[i % nricLetters.Length]}",
                    // 8 digits, starting with 8 or 9
                    phoneNumber = $"{(rng.Next(2) == 0 ? 8 : 9)}{rng.Next(0, 10_000_000):D7}",
                    clinicalStatusId = statusIds[status],
                    doctorId = doctorIds[rng.Next(doctorIds.Count)],
                    bookInDate = bookIn,
                    bookOutDate = bookOut
                });
            }
 
            db.patients.AddRange(patients);
            await db.SaveChangesAsync();
        }
    }
}