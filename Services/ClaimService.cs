using Microsoft.EntityFrameworkCore;
using InsuranceClaim.Data;
using InsuranceClaim.DTOs;   
using InsuranceClaim.Models;
using InsuranceClaim.Services;  

namespace InsuranceClaim.Services;

public class ClaimService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly NotificationService _notification;

    public ClaimService(
        AppDbContext db,
        IWebHostEnvironment env,
        NotificationService notification)
    {
        _db = db;
        _env = env;
        _notification = notification;
    }

    // GET ALL
    public async Task<List<Claim>> GetAllAsync(string role, int userId)
    {
        var query = _db.Claims
            .Where(c => c.ClaimStatus != "Deleted");

        if (role == "Officer")
            query = query.Where(c => c.ReceivedBy == userId);

        return await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    // GET BY ID
    public async Task<Claim?> GetByIdAsync(int id)
    {
        return await _db.Claims
            .Include(c => c.AuditLogs)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    // CREATE
    public async Task<Claim> CreateAsync(ClaimCreateDto dto, int userId)
    {
        var claim = new Claim
        {
            CustomerNo = dto.CustomerNo,
            ClaimTitle = dto.ClaimTitle,
            ClaimDesc = dto.ClaimDesc,
            IncidentDate = dto.IncidentDate,
            BookBank = dto.BookBank,
            ReceivedBy = userId,
            ReportedDate = DateTime.UtcNow,
            ClaimStatus = "Pending"
        };

        // Upload file
        if (dto.MedicalCertificate != null)
            claim.MedicalCertificatePath = await SaveFileAsync(dto.MedicalCertificate);

        _db.Claims.Add(claim);
        await _db.SaveChangesAsync();

        // Audit log
        await AddAuditLogAsync(claim.Id, userId, null, "Pending", "Created");

        return claim;
    }

    // UPDATE
    public async Task<bool> UpdateAsync(int id, ClaimUpdateDto dto, int userId)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return false;

        // Block ถ้า status ไม่ใช่ Pending
        if (claim.ClaimStatus != "Pending") return false;

        claim.ClaimTitle = dto.ClaimTitle ?? claim.ClaimTitle;
        claim.ClaimDesc = dto.ClaimDesc ?? claim.ClaimDesc;
        claim.IncidentDate = dto.IncidentDate ?? claim.IncidentDate;
        claim.BookBank = dto.BookBank ?? claim.BookBank;
        claim.UpdatedDate = DateTime.UtcNow;

        if (dto.MedicalCertificate != null)
            claim.MedicalCertificatePath = await SaveFileAsync(dto.MedicalCertificate);

        await _db.SaveChangesAsync();
        await AddAuditLogAsync(claim.Id, userId, "Pending", "Pending", "Updated");

        return true;
    }

    // DELETE (soft delete)
    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return false;

        if (claim.ClaimStatus == "Approved" ||
            claim.ClaimStatus == "Rejected") return false;

        var oldStatus = claim.ClaimStatus;
        claim.ClaimStatus = "Deleted";
        claim.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await AddAuditLogAsync(claim.Id, userId, oldStatus, "Deleted", "Deleted");

        return true;
    }

    // MEDICAL UPDATE
    public async Task<bool> MedicalUpdateAsync(int id, MedicalUpdateDto dto, int userId)
    {
        var claim = await _db.Claims.FindAsync(id);
        if (claim == null) return false;

        var oldStatus = claim.ClaimStatus;
        claim.MedicalStatus = dto.MedicalStatus;
        claim.MedicalComment = dto.MedicalComment;
        claim.ClaimStatus = dto.MedicalStatus == "Approved"
            ? "Pending_Medical_Approved"
            : "Rejected";
        claim.UpdatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await AddAuditLogAsync(claim.Id, userId, oldStatus, claim.ClaimStatus, "Medical updated");

        // แจ้งเตือนผ่าน n8n
        await _notification.SendAsync(claim);

        return true;
    }

    // SAVE FILE
    private async Task<string> SaveFileAsync(IFormFile file)
    {
        var folder = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(folder, fileName);

        using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{fileName}";
    }

    // AUDIT LOG
    private async Task AddAuditLogAsync(
        int claimId, int userId,
        string? oldStatus, string? newStatus,
        string remark)
    {
        _db.ClaimAuditLogs.Add(new ClaimAuditLog
        {
            ClaimId = claimId,
            ChangedBy = userId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Remark = remark
        });
        await _db.SaveChangesAsync();
    }
}