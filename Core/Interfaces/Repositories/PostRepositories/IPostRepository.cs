using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IPostRepository
    {
        Task<List<ShowExternalFormOfShopPostDTO>> GetPostsOfShop(int shopId);
        Task<List<ShowExternalFormOfGymPostDTO>> GetPostsOfGym(int gymId);
        Task<List<ShowExternalFormOfCoachPostDTO>> GetPostsOfCoach(string coachId);
        Task<List<ShowGeneralFormOfPostDTO>> GetPostsForUserFromFollowers(string userId);
        Task<ShowPostDTO> GetPost(int id);
        Task<IntResult> DeleteComment(int commentId,string userId);
        Task<ShowLikeDTO> GetLike(int id);
        Task<ShowMainCommentDTO> GetComment(int id);
        //posts
        Task<IntResult> DeletePost(int id, string userId);
        Task<IntResult> AddLikeOnPost(AddLikeDTO likeDTO,string userId);
        Task<IntResult> DeleteLikeFromPost(string userId, int postId);
        Task<IntResult> AddCommentOnPost(AddCommentDTO commentDTO,string userId);
        Task<List<ShowLikeDTO>> GetLikeListOnPost(int id);
        //comments
        Task<IntResult> AddLikeOnComment(AddLikeDTO likeDTO,string userId);
        Task<IntResult> DeleteLikeFromComment(string userId, int commentId);
        Task<IntResult> AddCommentOnComment(AddCommentDTO commentDTO,string userId);
        Task<List<ShowLikeDTO>> GetLikeListOnComment(int id);

    }
}
