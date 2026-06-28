namespace InsuranceClaim.Models;

public class ClaimAuditLog
{
    public int Id { get; set; }
    public int ClaimId { get; set; }
    public int ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    public string? OldStatus { get; set; }
    public string? NewStatus { get; set; }
    public string? Remark { get; set; }

    // Navigation
    public Claim Claim { get; set; } = null!;
}