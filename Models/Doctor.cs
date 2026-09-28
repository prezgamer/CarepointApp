namespace CarePointApp.Models;

public class Doctor : Person
{

    public int doctorSpecialityId { get; set; }
    // Type of Speciality of the doctor
    public DoctorSpeciality? speciality {get;set;}
}
