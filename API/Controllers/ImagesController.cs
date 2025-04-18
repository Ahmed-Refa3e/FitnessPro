using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;
public class ImagesController(IBlobService _blobService,
    SignInManager<ApplicationUser> signInManager, FitnessContext _context,
    IGymRepository gymRepo) : BaseApiController
{

    // Check the image (type, size, etc.)
    private IActionResult CheckImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Please upload a valid image file.");

        if (!file.ContentType.StartsWith("image/"))
            return BadRequest("Only image files are allowed.");

        const long maxSize = 5 * 1024 * 1024; // 5MB
        if (file.Length > maxSize)
            return BadRequest("Image size must not exceed 5MB.");

        return Ok();
    }

    // POST api/images/upload-user-image/{userId}
    [HttpPost("upload-user-image")]
    [Authorize(Roles = "Coach,Trainee")]
    public async Task<IActionResult> UploadUserImage([FromForm] IFormFile file)
    {
        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        // Check image validity
        var checkResult = CheckImage(file);
        if (checkResult is BadRequestObjectResult)
            return checkResult;

        // Check if the user already has an image
        if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            // Delete the old image from Azure Blob Storage
            await _blobService.DeleteImageAsync(user.ProfilePictureUrl);
        }
        // Upload the image to Azure Blob
        var imageUrl = await _blobService.UploadImageAsync(file);

        user.ProfilePictureUrl = imageUrl;
        _context.applicationUsers.Update(user);
        await _context.SaveChangesAsync();

        return Ok(new { Url = imageUrl });
    }

    // DELETE api/images/delete-user-image/{userId}
    [HttpDelete("delete-user-image")]
    [Authorize(Roles = "Coach,Trainee")]
    public async Task<IActionResult> DeleteUserImage()
    {
        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");

        if (string.IsNullOrEmpty(user.ProfilePictureUrl))
        {
            return NotFound("No image found to delete.");
        }

        // Remove image URL from the database
        await _blobService.DeleteImageAsync(user.ProfilePictureUrl);
        user.ProfilePictureUrl = null;
        _context.applicationUsers.Update(user);
        await _context.SaveChangesAsync();

        return Ok("Image deleted successfully.");
    }

    // POST api/images/upload-gym-image/{gymId}
    [HttpPost("upload-gym-image/{gymId}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> UploadGymImage(int gymId, [FromForm] IFormFile file)
    {
        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");
        // Check image validity
        var checkResult = CheckImage(file);
        if (checkResult is BadRequestObjectResult)
            return checkResult;

        var gym = await gymRepo.GetByIdAsync(gymId);

        if (gym == null)
            return NotFound("Gym not found.");

        if (gym.CoachID != user.Id)
            return Unauthorized("You are not authorized to update this gym.");

        // Check if the gym already has an image
        if (!string.IsNullOrEmpty(gym.PictureUrl))
        {
            // Delete the old image from Azure Blob Storage
            await _blobService.DeleteImageAsync(gym.PictureUrl);
        }
        // Upload the image to Azure Blob
        var imageUrl = await _blobService.UploadImageAsync(file);

        gym.PictureUrl = imageUrl;
        gymRepo.Update(gym);
        await _context.SaveChangesAsync();

        return Ok(new { Url = imageUrl });
    }

    // DELETE api/images/delete-gym-image/{gymId}
    [HttpDelete("delete-gym-image/{gymId}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> DeleteGymImage(int gymId)
    {
        var gym = await gymRepo.GetByIdAsync(gymId);
        if (gym == null)
            return NotFound("Gym not found.");

        // Delete image from Azure Blob Storage
        if (string.IsNullOrEmpty(gym.PictureUrl))
        {
            return NotFound("No image found to delete.");
        }
        // Check if the gym belongs to the user
        ApplicationUser? user = await signInManager.UserManager.GetUserAsync(User);
        if (user == null)
            return NotFound("User not found.");
        if (gym.CoachID != user.Id)
            return Unauthorized("You are not authorized to delete this gym image.");

        // Remove image URL from the database
        await _blobService.DeleteImageAsync(gym.PictureUrl);
        gym.PictureUrl = null;
        gymRepo.Update(gym);
        await _context.SaveChangesAsync();

        return Ok("Image deleted successfully.");
    }

    // POST api/images/upload-shop-image/{shopId}
    [HttpPost("upload-shop-image/{shopId}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> UploadShopImage(int shopId, [FromForm] IFormFile file)
    {
        // Check image validity
        var checkResult = CheckImage(file);
        if (checkResult is BadRequestObjectResult)
            return checkResult;

        var shop = await _context.Shops.FindAsync(shopId);
        if (shop == null)
            return NotFound("Shop not found.");
        // Check if the shop already has an image
        if (!string.IsNullOrEmpty(shop.PictureUrl))
        {
            // Delete the old image from Azure Blob Storage
            await _blobService.DeleteImageAsync(shop.PictureUrl);
        }
        // Upload the image to Azure Blob
        var imageUrl = await _blobService.UploadImageAsync(file);
  
        shop.PictureUrl = imageUrl;
        _context.Shops.Update(shop);
        await _context.SaveChangesAsync();

        return Ok(new { Url = imageUrl });
    }

    // DELETE api/images/delete-shop-image/{shopId}
    [HttpDelete("delete-shop-image/{shopId}")]
    [Authorize(Roles = "Coach")]
    public async Task<IActionResult> DeleteShopImage(int shopId)
    {
        // Find the shop by Id
        var shop = await _context.Shops.FindAsync(shopId);
        if (shop == null)
            return NotFound("Shop not found.");

        // Delete image from Azure Blob Storage
        if (string.IsNullOrEmpty(shop.PictureUrl))
        {
            return NotFound("No image found to delete.");
        }
        // Remove image URL from the database
        await _blobService.DeleteImageAsync(shop.PictureUrl);
        shop.PictureUrl = null;
        _context.Shops.Update(shop);
        await _context.SaveChangesAsync();

        return Ok("Image deleted successfully.");
    }
}
