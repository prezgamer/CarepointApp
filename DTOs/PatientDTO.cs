namespace CarePointApp.DTOs;
using System.ComponentModel.DataAnnotations;

// For Creating Patient DTO
public record CreatePatientDto
(
    [Required] string patientName, // Patient Name is Required
    [Required] string patientGender, // Patient Gender is Required

    [StringLength(9, MinimumLength = 9, ErrorMessage = "NRIC need to be 8 Characters")]
    [Required] string patientNRIC, // Patient NRIC is Required

    // patient phone number should not be less or more than 8 numbers, it can also only contain digits
    [Required]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Phone number must be 8 characters.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
    string patientPhoneNumber,

    int clinicalStatusId,
    int doctorId
);

// For Updating Patient DTO
public record UpdatePatientDto(
    [Required] string patientName, // Patient Name is Required
    [Required] string patientGender, // Patient Gender is Required

    [StringLength(9, MinimumLength = 9, ErrorMessage = "NRIC need to be 8 Characters")]
    [Required] string patientNRIC, // Patient NRIC is Required

    // patient phone number should not be less or more than 8 numbers, it can also only contain digits
    [Required]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Phone number must be 8 characters.")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only digits.")]
    string patientPhoneNumber,

    int clinicalStatusId,
    int doctorId
);

public record CheckInDto(DateTime bookInDate);
public record CheckOutDto(DateTime bookOutDate);
