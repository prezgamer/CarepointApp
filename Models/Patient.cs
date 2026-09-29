using System;

namespace CarePointApp.Models;

public class Patient : Person
{
    public required string nric { get; set; }   // NRIC of the Patient
    public required string gender { get; set; }   // Gender of the Patient
    public DateTime? bookInDate { get; set; }   // Book In Date of the Patient
    public DateTime? bookOutDate { get; set; }   // Book out Date of the Patient

    // FK of clinicalStatus
    public int clinicalStatusId {get; set;}

    // sets the clinical status of the patient
    public ClinicalStatus? clinicalStatus {get;set;}

    // FK of Doctor
    public int? doctorId { get; set; }      
    // sets the doctor of the patient
    public Doctor? assignedDoctor {get;set;}
}
