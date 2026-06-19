using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Services;

namespace TraineeManagement.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class SubmissionFileController : ControllerBase
{
    private readonly ISubmissionFileService _submissionFileService;

    public SubmissionFileController(
        ISubmissionFileService submissionFileService)
    {
        _submissionFileService = submissionFileService;
    }

    [RequestSizeLimit(20 * 1024 * 1024)]
    [HttpPost("submissions/{submissionId}/files")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
    int submissionId,
    IFormFile file,
    CancellationToken cancellationToken)
    {
        var result =
            await _submissionFileService.UploadAsync(
                submissionId,
                file,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetMetadata),
            new { id = result.Id },
            result);
    }

    [HttpGet("submission-files/{id}")]
    public async Task<IActionResult> GetMetadata(
    int id,
    CancellationToken cancellationToken)
    {
        var file =
            await _submissionFileService.GetByIdAsync(
                id,
                cancellationToken);

        if (file == null)
        {
            return NotFound(new
            {
                message = "File not found."
            });
        }

        return Ok(file);
    }

    [HttpGet("submission-files/{id}/download")]
    public async Task<IActionResult> Download(
        int id,
        CancellationToken cancellationToken)
    {
        var result =
            await _submissionFileService.DownloadAsync(
                id,
                cancellationToken);

        return File(
            result.Stream,
            result.File.ContentType,
            result.File.OriginalFileName);
    }

    [HttpDelete("submission-files/{id}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        await _submissionFileService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }
}