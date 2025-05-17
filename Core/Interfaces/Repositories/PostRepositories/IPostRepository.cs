using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IPostRepository
    {
        Task<List<ShowExternalFormOfShopPostDTO>> GetPostsOfShop(int shopId,int pageNumber, string userId="");
        Task<List<ShowExternalFormOfGymPostDTO>> GetPostsOfGym(int gymId, int pageNumber, string userId="");
        Task<List<ShowExternalFormOfCoachPostDTO>> GetPostsOfCoach(string coachId, int pageNumber, string userId = "");
        Task<List<ShowGeneralFormOfPostDTO>> GetPostsForUserFromFollowers(int pageNumber, string userId = "");
        Task<ShowPostDTO> GetPost(int id);
        Task<IntResult> DeleteComment(int commentId,string userId);
        Task<ShowLikeDTO> GetLike(int id);
        Task<ShowMainCommentDTO> GetComment(int id);
        //posts
        Task<IntResult> DeletePost(int id, string userId);
        Task<IntResult> AddLikeOnPost(AddLikeOnPostDTO likeDTO,string userId);
        Task<IntResult> DeleteLikeFromPost(string userId, int postId);
        Task<IntResult> AddCommentOnPost(AddCommentOnPostDTO commentDTO,string userId);
        Task<List<ShowLikeDTO>> GetLikeListOnPost(int id);
        //comments
        Task<IntResult> AddLikeOnComment(AddLikeOnCommentDTO likeDTO,string userId);
        Task<IntResult> DeleteLikeFromComment(string userId, int commentId);
        Task<IntResult> AddCommentOnComment(AddCommentOnCommentDTO commentDTO,string userId);
        Task<List<ShowLikeDTO>> GetLikeListOnComment(int id);

    }
}
