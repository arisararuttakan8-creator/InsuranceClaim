namespace InsuranceClaim.DTOs;

public class ClaimUpdateDto
{
    public string? ClaimTitle { get; set; }
    public string? ClaimDesc { get; set; }
    public DateTime? IncidentDate { get; set; }
    public IFormFile? MedicalCertificate { get; set; }
    public string? BookBank { get; set; }
}