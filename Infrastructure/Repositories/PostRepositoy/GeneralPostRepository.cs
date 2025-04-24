using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories.PostRepositoy
{
    public abstract class GeneralPostRepository : IGeneralPostRepository
    {
        protected readonly FitnessContext _context;
        public GeneralPostRepository(FitnessContext context)
        {
            _context = context;
        }

        public abstract Task<IntResult> Add(AddPostDTO postDto, string userId);
        protected async Task<IntResult> AddPicturesToPost(AddPostDTO post, Post newPost)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!post.Images.IsNullOrEmpty())
                {
                    foreach (var image in post.Images)
                    {
                        newPost.PictureUrls.Add(new PostPictureUrl { Url = image });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = newPost.Id };
        }

    }

}
