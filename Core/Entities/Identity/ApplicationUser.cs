using Core.Entities.FollowEntities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    [MaxLength(30)]
    public required string FirstName { get; set; }

    [MaxLength(30)]
    public required string LastName { get; set; }

    public string? Gender { get; set; }

    [MaxLength(100)]
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.Now;
    public List<RefreshToken>? refreshTokens { get; set; }

    public List<UserFollow>? Followers { get; set; } //people that follow this user
    public List<UserFollow>? Following { get; set; } //people the user follow thim
    public List<GymFollow>? FollowedGyms { get; set; }
    public List<ShopFollow>? FollowedShops { get; set; }
}
