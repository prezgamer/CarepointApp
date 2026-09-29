using System;

namespace CarePointApp.Models;

public class Person
{
    public int id { get; set; }   // Unique identifier for the person
    public required string name { get; set; }  // Name of the person
    public required string phoneNumber { get; set; }  // Phone number of the person
}
