using System;

namespace CarePointApp.Models;

public class Person
{
    public int id { get; set; }   
    public required string name { get; set; }  
    public required string phoneNumber { get; set; }  
}
