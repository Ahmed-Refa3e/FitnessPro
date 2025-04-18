using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class GymPostRepository : GeneralPostRepository
    {
        public GymPostRepository(FitnessContext context) : base(context)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post, string userId)
        {
            var newPost = post as AddGymPostDTO;
            if (post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            if (_context.Gyms.Find(newPost.GymId).CoachID != userId)
            {
                return new IntResult { Massage = "you are not the Owner of this gym to add post" };
            }
            var gymPost = new GymPost { Content = newPost.Content, GymId = newPost.GymId };
            _context.GymPosts.Add(gymPost);
            return await AddPicturesToPost(post, gymPost);
        }
    }
}
