namespace TkrulVideoUpload.Models.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

public class Video
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Url { get; set; } = "";

    [ForeignKey("UploaderUser")]
    public string UploaderUserId { get; set; } = "";

    public IdentityUser UploaderUser { get; set; } = new IdentityUser();

    [Required]
    public DateTime UploadedDate { get; set; }

    public ICollection<VideoAccessLog> AccessLogs { get; set; } = new List<VideoAccessLog>();
}

public class VideoAccessLog
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Video")]
    public Guid VideoId { get; set; }

    public Video Video { get; set; } = null!;

    [ForeignKey("User")]
    public string UserId { get; set; } = "";

    public IdentityUser User { get; set; } = null!;

    [Required]
    public DateTime AccessDate { get; set; }

    [Required]
    public VideoAccessLogType AccessType { get; set; }
}


public enum VideoAccessLogType
{
    Read,
    Write
}