using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Interfaces.Services;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories.PostRepositoy
{
    public abstract class GeneralPostRepository : IGeneralPostRepository
    {
        protected readonly FitnessContext _context;
        private readonly IBlobService _blobService;

        public GeneralPostRepository(FitnessContext context,IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public abstract Task<IntResult> Add(AddPostDTO postDto, string userId);
        protected async Task<IntResult> AddPicturesToPost(AddPostDTO post, Post newPost)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SaveChangesAsync();
                if (!post.Images.IsNullOrEmpty())
                {
                    foreach (var image in post.Images)
                    {
                        var ImageResult = AddImageHelper.CheckImage(image);
                        if (ImageResult.Id != 1)
                        {
                            throw new Exception(ImageResult.Massage);
                        } 
                        var url=await _blobService.UploadImageAsync(image);
                        newPost.PictureUrls.Add(new PostPictureUrl { Url = url });
                    }
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                foreach(var url in newPost.PictureUrls.Select(x=>x.Url))
                {
                    await _blobService.DeleteImageAsync(url);
                }
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = newPost.Id };
        }

    }

}
