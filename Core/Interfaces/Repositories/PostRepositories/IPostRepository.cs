using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repositories.PostRepositories
{
    public  interface IPostRepository
    {
        ShowPostDTO Get(int id);
        IntResult AddLikeOnPost(AddLikeDTO likeDTO);
        IntResult DeleteLikeFromPost(string userId,int postId);
        ShowLikeDTO GetLike(int id);
        IntResult AddComentOnPost(AddCommentDTO commentDTO);
        //IntResult DeleteCommentFromPost(string UserId, int postId);
        IntResult DeleteCommentFromPost(int commentId);
        ShowCommentDTO GetComment(int id);
        List<ShowLikeDTO> GetLikeListOnPost(int id);
    }
}
