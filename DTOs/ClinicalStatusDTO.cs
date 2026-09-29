namespace CarePointApp.DTOs;

public record CreateClinicalStatusDto
(
    string ClinicalStatusName // Clinical Status Name
    );

public record UpdateClinicalStatusDto(
    string ClinicalStatusName // Clinical Status Name
    );
