namespace InsuranceClaim.DTOs;

public class ClaimCreateDto
{
    public string CustomerNo { get; set; } = string.Empty;
    public string ClaimTitle { get; set; } = string.Empty;
    public string? ClaimDesc { get; set; }
    public DateTime IncidentDate { get; set; }
    public IFormFile? MedicalCertificate { get; set; }
    public string? BookBank { get; set; }
    public int ReceivedBy { get; set; }
}