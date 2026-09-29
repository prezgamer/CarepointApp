using CarePointApp.Data;
using CarePointApp.Models;
using Microsoft.EntityFrameworkCore;
namespace CarePointApp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(CarePointDataContext db)
    {
        // ---------- 1. Clinical statuses ----------
        // Seed each name individually if missing, instead of only checking "is the table empty".
        // This survives a partial delete/cascade without crashing later lookups.
        string[] statusNames = { "Arrived", "Late", "Missed" };
        foreach (var name in statusNames)
        {
            if (!await db.clinicalStatuses.AnyAsync(s => s.clinicalStatusName == name))
                db.clinicalStatuses.Add(new ClinicalStatus { clinicalStatusName = name });
        }
        await db.SaveChangesAsync();

        // ---------- 2. Doctor specialities ----------
        string[] specialityNames = { "General Practice", "Cardiology", "Paediatrics", "Orthopaedics", "Dermatology" };
        foreach (var name in specialityNames)
        {
            if (!await db.doctorSpecialities.AnyAsync(s => s.doctorSpecialityType == name))
                db.doctorSpecialities.Add(new DoctorSpeciality { doctorSpecialityType = name });
        }
        await db.SaveChangesAsync();

        // ---------- 3. Doctors ----------
        // Re-read ids AFTER the specialities above are guaranteed to exist.
        var specialityIds = await db.doctorSpecialities
            .ToDictionaryAsync(s => s.doctorSpecialityType, s => s.doctorSpecialityId);

        var doctorSeed = new (string Name, string Phone, string Speciality)[]
        {
            ("Dr. Lim Wei Jie",  "91110001", "General Practice"),
            ("Dr. Priya Nair",   "91110002", "Cardiology"),
            ("Dr. Ahmad Rahman", "91110003", "Paediatrics"),
            ("Dr. Chua Mei Ling","91110004", "Orthopaedics"),
            ("Dr. Koh Jun Wei",  "91110005", "Dermatology"),
        };

        foreach (var d in doctorSeed)
        {
            if (!await db.doctors.AnyAsync(x => x.name == d.Name))
            {
                db.doctors.Add(new Doctor
                {
                    name = d.Name,
                    phoneNumber = d.Phone,
                    doctorSpecialityId = specialityIds[d.Speciality]
                });
            }
        }
        await db.SaveChangesAsync();

        // ---------- 4. Patients (100 fake ones) ----------
        // Still gated on "table is empty", since 100 randomly generated patients
        // aren't individually named/checkable the way statuses/specialities/doctors are.
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