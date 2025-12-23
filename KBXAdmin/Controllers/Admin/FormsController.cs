using KBXAdmin.Application.DTOs;
using KBXAdmin.Application.Interfaces.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class FormsController : ControllerBase
{
    private readonly IFormService _service;

    public FormsController(IFormService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        int page = 1,
        int pageSize = 10,
        string search = "")
        => Ok(await _service.GetFormsAsync(page, pageSize, search));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFormRequest request)
    {
        var userId =
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _service.CreateAsync(request.Title, userId);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Save(Guid id, [FromBody] SaveFormRequest request)
    {
        if (request.SchemaJson.ValueKind == JsonValueKind.Undefined ||
            request.SchemaJson.ValueKind == JsonValueKind.Null)
        {
            return BadRequest("schemaJson is required");
        }

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // ✅ SAFE conversion
        var schemaJsonString = request.SchemaJson.GetRawText();

        await _service.SaveDraftAsync(id, schemaJsonString, userId);
        return Ok();
    }


    [HttpPost("{id}/autosave")]
    public async Task<IActionResult> AutoSave(Guid id, [FromBody] AutoSaveRequest request)
    {
        if (request.SchemaJson.ValueKind == JsonValueKind.Undefined ||
            request.SchemaJson.ValueKind == JsonValueKind.Null)
        {
            return BadRequest("schemaJson is required");
        }

        var userId =
            User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // ✅ THIS IS THE KEY LINE
        var schemaJsonString = request.SchemaJson.GetRawText();

        await _service.AutoSaveAsync(id, schemaJsonString, userId);
        return Ok();
    }

    [HttpPost("{id}/session/start")]
    public async Task<IActionResult> Start(Guid id, [FromBody] dynamic body)
    {
        await _service.StartSessionAsync(id, (string)body.userId);
        return Ok();
    }

    [HttpPost("{id}/session/end")]
    public async Task<IActionResult> End(Guid id, [FromBody] dynamic body)
    {
        await _service.EndSessionAsync(id, (string)body.userId);
        return Ok();
    }

    [HttpGet("{id}/versions")]
    public async Task<IActionResult> Versions(Guid id)
        => Ok(await _service.GetVersionsAsync(id));

    [HttpPost("{id}/versions/{versionId}/restore")]
    public async Task<IActionResult> Restore(Guid id, Guid versionId, [FromBody] dynamic body)
    {
        await _service.RestoreVersionAsync(id, versionId, (string)body.userId);
        return Ok();
    }

    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _service.PublishAsync(id, userId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return Ok();
    }
    [HttpGet("{id}/audit")]
    public async Task<IActionResult> Audit(Guid id)
    {
        var logs = await _service.GetAuditLogsAsync(id);

        return Ok(logs);
    }
}
