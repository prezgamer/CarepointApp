using System;
using System.Text.Json.Serialization;

namespace CarePointApp.Models;

public class ClinicalStatus
{
    // Primary key of Clinical Status ID
    public int clinicalStatusId {get; set;}
    
    // Clinical Status Name
    public required string clinicalStatusName {get; set;}
}
