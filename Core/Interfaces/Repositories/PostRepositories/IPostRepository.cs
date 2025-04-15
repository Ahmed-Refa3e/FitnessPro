using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public interface IPostRepository
    {
        ShowPostDTO GetPost(int id);
        IntResult DeleteComment(int commentId);
        ShowLikeDTO GetLike(int id);
        ShowMainCommentDTO GetComment(int id);
        //posts
        IntResult DeletePost(int id);
        IntResult AddLikeOnPost(AddLikeDTO likeDTO);
        IntResult DeleteLikeFromPost(string userId, int postId);
        IntResult AddComentOnPost(AddCommentDTO commentDTO);
        List<ShowLikeDTO> GetLikeListOnPost(int id);
        //comments
        IntResult AddLikeOnComment(AddLikeDTO likeDTO);
        IntResult DeleteLikeFromComment(string userId, int commentId);
        IntResult AddComentOnComment(AddCommentDTO commentDTO);
        List<ShowLikeDTO> GetLikeListOnComment(int id);

    }
}
