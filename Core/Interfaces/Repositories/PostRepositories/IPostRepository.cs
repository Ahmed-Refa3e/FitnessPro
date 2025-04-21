using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IPostRepository
    {
        ShowPostDTO GetPost(int id);
        IntResult DeleteComment(int commentId,string userId);
        ShowLikeDTO GetLike(int id);
        ShowMainCommentDTO GetComment(int id);
        //posts
        IntResult DeletePost(int id, string userId);
        IntResult AddLikeOnPost(AddLikeDTO likeDTO,string userId);
        IntResult DeleteLikeFromPost(string userId, int postId);
        IntResult AddCommentOnPost(AddCommentDTO commentDTO,string userId);
        List<ShowLikeDTO> GetLikeListOnPost(int id);
        //comments
        IntResult AddLikeOnComment(AddLikeDTO likeDTO,string userId);
        IntResult DeleteLikeFromComment(string userId, int commentId);
        IntResult AddCommentOnComment(AddCommentDTO commentDTO,string userId);
        List<ShowLikeDTO> GetLikeListOnComment(int id);

    }
}
