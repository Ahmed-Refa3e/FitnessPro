using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class CoachPostRepository : GeneralPostRepository
    {
        public CoachPostRepository(FitnessContext context) : base(context)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post)
        {
            var newPost = post as AddCoachPostDTO;
            if (post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            var coachPost = new CoachPost { Content = newPost.Content, CoachId = newPost.CoachId };
            _context.CoachPosts.Add(coachPost);
            return await AddPicturesToPost(post, coachPost);
        }
    }
}
