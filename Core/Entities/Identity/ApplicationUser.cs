using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(30)]
    public required string FirstName { get; set; }

    [MaxLength(30)]
    public required string LastName { get; set; }

    public required string Gender { get; set; }

    [MaxLength(100)]
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.Now;
    public List<RefreshToken>? refreshTokens { get; set; }
}
