using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace TkrulVideoUpload.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UploadVideoController : ControllerBase
{
    private readonly ILogger<UploadVideoController> _logger;
    private readonly IBlobService _blobService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public UploadVideoController(
        ILogger<UploadVideoController> logger,
        IBlobService blobService,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context)
    {
        _logger = logger;
        _blobService = blobService;
        _userManager = userManager;
        _context = context;
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

        var videoRecord = new Models.Entities.Video
        {
            Url = blob,
            UploaderUserId = _userManager.GetUserId(User),
            UploadedDate = DateTime.UtcNow
        };

        using (var scope = _context.Database.BeginTransaction())
        {
            await _context.Videos.AddAsync(videoRecord);
            await _context.SaveChangesAsync();
            scope.Commit();
        }

        return Ok(blob);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        var username = User.Identity.Name;
        return Ok($"Hello {username} from UploadVideoController");
    }

}
