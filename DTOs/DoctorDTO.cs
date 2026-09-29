namespace CarePointApp.DTOs;
using System.ComponentModel.DataAnnotations;

public record CreateDoctorDto
(
    [Required] string doctorName, // Doctor Name is Required

    [Required]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Phone number must be 8 characters.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
    string doctorPhoneNumber, // Doctor Phone Number is Required

    [Required]
    int? doctorSpecialityId // Doctor Speciality ID is Required
);

public record UpdateDoctorDto
(
    [Required] string doctorName, // Doctor Name is Required

    [Required]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Phone number must be 8 characters.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
    string doctorPhoneNumber, // Doctor Phone Number is Required

    [Required]
    int? doctorSpecialityId // Doctor Speciality ID is Required
);
