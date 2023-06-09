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

    public IdentityUser? UploaderUser { get; set; }

    [Required]
    public DateTime UploadedDate { get; set; }
}