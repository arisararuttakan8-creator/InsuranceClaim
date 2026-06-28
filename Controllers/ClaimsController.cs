using Microsoft.AspNetCore.Mvc;
using InsuranceClaim.DTOs;
using InsuranceClaim.Services;


namespace InsuranceClaim.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClaimsController : ControllerBase
{
    private readonly ClaimService _claimService;

    public ClaimsController(ClaimService claimService)
    {
        _claimService = claimService;
    }

    // GET /api/claims
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // TODO: ดึง role และ userId จาก JWT ค่ะ
        // ตอนนี้ hardcode ก่อนค่ะ
        var claims = await _claimService.GetAllAsync("Admin", 1);
        return Ok(claims);
    }

    // GET /api/claims/:id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var claim = await _claimService.GetByIdAsync(id);
        if (claim == null) return NotFound();
        return Ok(claim);
    }

    // POST /api/claims
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ClaimCreateDto dto)
    {
        // TODO: ดึง userId จาก JWT ค่ะ
        var claim = await _claimService.CreateAsync(dto, 1);
        return CreatedAtAction(nameof(GetById),
            new { id = claim.Id }, claim);
    }

    // PUT /api/claims/:id
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id, [FromForm] ClaimUpdateDto dto)
    {
        var result = await _claimService.UpdateAsync(id, dto, 1);
        if (!result) return BadRequest("ไม่สามารถแก้ไขได้ค่ะ");
        return Ok();
    }

    // DELETE /api/claims/:id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _claimService.DeleteAsync(id, 1);
        if (!result) return BadRequest("ไม่สามารถลบได้ค่ะ");
        return Ok();
    }

    // PATCH /api/claims/:id/medical
    [HttpPatch("{id}/medical")]
    public async Task<IActionResult> MedicalUpdate(
        int id, [FromBody] MedicalUpdateDto dto)
    {
        var result = await _claimService.MedicalUpdateAsync(id, dto, 1);
        if (!result) return NotFound();
        return Ok();
    }
}