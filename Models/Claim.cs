namespace InsuranceClaim.Models;

public class Claim
{
    public int Id { get; set; }
    public string CustomerNo { get; set; } = string.Empty;
    public string ClaimTitle { get; set; } = string.Empty;
    public string? ClaimDesc { get; set; }
    public DateTime IncidentDate { get; set; }
    public string? MedicalCertificatePath { get; set; }
    public string ClaimStatus { get; set; } = "Pending";
    // Pending / Pending_Medical_Approved / Approved / Rejected / Deleted
    public string? MedicalStatus { get; set; }
    // Approved / Rejected
    public string? MedicalComment { get; set; }
    public string? BookBank { get; set; }
    public DateTime ReportedDate { get; set; } = DateTime.UtcNow;
    public int ReceivedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<ClaimAuditLog> AuditLogs { get; set; } = new List<ClaimAuditLog>();
}