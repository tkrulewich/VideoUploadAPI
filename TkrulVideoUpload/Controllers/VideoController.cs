namespace TkrulVideoUpload.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    private readonly ILogger<VideoController> _logger;
    private readonly IBlobService _blobService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public VideoController(
        ILogger<VideoController> logger,
        IBlobService blobService,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _logger = logger;
        _blobService = blobService;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file received from the upload");
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var contentType = file.ContentType;
        var blob = await _blobService.UploadFileBlobAsync(file.OpenReadStream(), fileName, contentType);

        var videoRecord = new Models.Entities.Video
        {
            Url = blob,
            UploaderUserId = _userManager.GetUserId(User),
            UploadedDate = DateTime.UtcNow
        };

        await _context.Videos.AddAsync(videoRecord);
        await _context.SaveChangesAsync();
        
        
        var uniqueId = videoRecord.Id.ToString();

        return Ok(uniqueId);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var video = await _context.Videos.FindAsync(id);

        if (video == null)
        {
            return NotFound();
        }

        var stream = await _blobService.GetBlobDataAsync(video.Url);

        return Ok(stream);
    }

    [HttpGet("greeting")]
    public IActionResult Get()
    {
        var username = User?.Identity?.Name ?? "Anonymous";
        return Ok($"Hello {username} from UploadVideoController");
    }



}
