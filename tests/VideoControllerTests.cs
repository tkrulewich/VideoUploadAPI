using NUnit.Framework;
using Moq;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TkrulVideoUpload.Controllers;
using TkrulVideoUpload.Models.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Security.Claims;

public class VideoControllerTests
{
    private Mock<ILogger<VideoController>> _mockLogger = null!;
    private Mock<IBlobService> _mockBlobService = null!;
    private Mock<UserManager<IdentityUser>> _mockUserManager = null!;
    private ApplicationDbContext _dbContext = null!;
    private VideoController _controller = null!;


    private List<Video> videos = new List<Video>
    {
        new Video
        {
            Id = Guid.NewGuid(),
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            UploaderUserId = "1",
            UploadedDate = DateTime.UtcNow
        }
    };

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<VideoController>>();
        _mockBlobService = new Mock<IBlobService>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new ApplicationDbContext(options);

        _dbContext.Videos.AddRange(videos);
        _dbContext.SaveChanges();
        
        _controller = new VideoController(
            _mockLogger.Object,
            _mockBlobService.Object,
            _mockUserManager.Object,
            _dbContext
        );
        

        _mockBlobService.Setup(service => service.GetBlobDataAsync(It.IsAny<string>()))
            .ReturnsAsync(new MemoryStream());

         _mockBlobService.Setup(service => service.UploadFileBlobAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

        
        _mockUserManager.Setup(manager => manager.GetUserId(It.IsAny<ClaimsPrincipal>()))
            .Returns("1");

    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
    }

    [Test]
    public async Task Get_ExistingVideo_ReturnsOkResult()
    {
        // Act
        // We know that the first video in the list exists so we can use that id
        var result = await _controller.Get(
            videos.First().Id
        );

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task Get_NonExistingVideo_ReturnsNotFoundResult()
    {
        // Act
        // Using a random guid that we know doesn't exist
        var result = await _controller.Get(
            Guid.NewGuid()
        );

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task Upload_ValidFile_ReturnsOkResult()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var fileName = "test.txt";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(_ => _.ContentType).Returns("text/plain");

        // Act
        var result = await _controller.Upload(fileMock.Object);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task Upload_File_Get_File_ReturnsOkResult()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var fileName = "test.txt";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
        fileMock.Setup(_ => _.FileName).Returns(fileName);
        fileMock.Setup(_ => _.Length).Returns(ms.Length);
        fileMock.Setup(_ => _.ContentType).Returns("text/plain");

        // Act
        var result = await _controller.Upload(fileMock.Object);
        Assert.IsInstanceOf<OkObjectResult>(result);

        var video_id = (result as OkObjectResult)?.Value?.ToString();

        if (video_id == null)
        {
            Assert.Fail("Video id is null");
        } else 
        {
            var video = await _controller.Get(
                Guid.Parse(video_id)
            );
            
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(video);

        }
    }
}
