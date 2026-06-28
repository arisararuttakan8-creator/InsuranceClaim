namespace InsuranceClaim.DTOs;

public class MedicalUpdateDto
{
    public string MedicalStatus { get; set; } = string.Empty;
    // Approved / Rejected
    public string? MedicalComment { get; set; }
}