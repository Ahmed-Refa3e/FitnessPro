using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Services;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class GymPostRepository : GeneralPostRepository
    {
        public GymPostRepository(FitnessContext context, IBlobService blobService) : base(context, blobService)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post, string userId)
        {
            var newPost = post as AddGymPostDTO;
            if (post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            var gym = await _context.Gyms.FindAsync(newPost.GymId);
            if (gym is null || gym.CoachID != userId)
            {
                return new IntResult { Massage = "Id is not valid." };
            }
            var gymPost = new GymPost { Content = newPost.Content, GymId = newPost.GymId };
            await _context.GymPosts.AddAsync(gymPost);
            return await AddPicturesToPost(post, gymPost);
        }
    }
}
