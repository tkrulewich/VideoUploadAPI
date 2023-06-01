using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace TkrulVideoUpload.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "VideoScope")]
public class UploadVideoController : ControllerBase
{
    private readonly ILogger<UploadVideoController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IBlobService _blobService;

    public UploadVideoController(ILogger<UploadVideoController> logger, IConfiguration configuration, IBlobService blobService)
    {
        _logger = logger;
        _configuration = configuration;
        _blobService = blobService;
    }

    [HttpPost]
    public async Task<IActionResult> UploadVideo(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file received from the upload");
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var contentType = file.ContentType;
        var blob = await _blobService.UploadFileBlobAsync(file.OpenReadStream(), fileName, contentType);

        return Ok(blob);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from UploadVideoController");
    }

}
