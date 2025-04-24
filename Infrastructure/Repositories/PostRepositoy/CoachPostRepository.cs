using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.Identity;
using Core.Entities.PostEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class CoachPostRepository : GeneralPostRepository
    {
        public CoachPostRepository(FitnessContext context) : base(context)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post, string userId)
        {
            if (post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            if (await _context.Users.FindAsync(userId) is not Coach )
            {
                return new IntResult { Massage = "you are not a Coach to add post" };
            }
            var coachPost = new CoachPost { Content = post.Content, CoachId = userId };
            await _context.CoachPosts.AddAsync(coachPost);
            return await AddPicturesToPost(post, coachPost);
        }
    }
}
