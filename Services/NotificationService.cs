using InsuranceClaim.Models;

namespace InsuranceClaim.Services;

public class NotificationService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public NotificationService(IConfiguration config, HttpClient http)
    {
        _config = config;
        _http = http;
    }

    public async Task SendAsync(Claim claim)
    {
        var webhookUrl = _config["N8N:WebhookUrl"];
        if (string.IsNullOrEmpty(webhookUrl)) return;

        var payload = new
        {
            claimId = claim.Id,
            customerNo = claim.CustomerNo,
            claimTitle = claim.ClaimTitle,
            claimStatus = claim.ClaimStatus,
            medicalStatus = claim.MedicalStatus,
            medicalComment = claim.MedicalComment,
            updatedDate = claim.UpdatedDate
        };

        await _http.PostAsJsonAsync(webhookUrl, payload);
    }
}