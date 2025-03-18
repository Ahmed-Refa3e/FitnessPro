using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Enums;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class PostRepository : IPostRepository
    {
        private readonly FitnessContext _context;

        public PostRepository(FitnessContext context)
        {
            _context = context;
        }

        public IntResult AddComentOnPost(AddCommentDTO commentDTO)
        {
            var post = GetPost(commentDTO.PostId);
            if(post is null)
            {
                return new IntResult { Massage = "No post has tis Id." };
            }
            var comment = new PostComment { Content = commentDTO.Content, PostId = commentDTO.PostId, UserId = commentDTO.UserId };
            post.Comments.Add(comment);
            try
            {
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id=comment.Id};
        }
        public IntResult DeleteCommentFromPost(int commentId)
        {
            var comment = _context.postComments.Find(commentId);
            if(comment is null)
            {
                return new IntResult { Massage = "No comment has this Id" };
            }
            _context.postComments.Remove(comment);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = comment.Id };
        }
        public IntResult AddLikeOnPost(AddLikeDTO likeDTO)
        {
            var post = GetPost(likeDTO.PostId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has tis Id." };
            }
            var oldLike= SearchWithUserIdAndPostId(likeDTO.UserId,likeDTO.PostId);
            if(oldLike is not null)
            {
                return new IntResult { Massage = "You already like this post." };
            }
            var like = new PostLike
            {
                Type = CheckStringAndReturnLikeType(likeDTO.Type),
                PostId = likeDTO.PostId,
                UserId = likeDTO.UserId
            };
            post.Likes.Add(like);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = like.Id };
        }
        public IntResult DeleteLikeFromPost(string userId, int postId)
        {
            var like=SearchWithUserIdAndPostId(userId, postId);
            if(like is null)
            {
                return new IntResult { Massage = "you do not like this post yet to delete like" };
            }
            _context.postLikes.Remove(like);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }
        PostLike SearchWithUserIdAndPostId(string userId, int postId)
        {
            return _context.postLikes.Where(x => x.UserId == userId && postId == postId).FirstOrDefault();
        }
        public ShowLikeDTO GetLike(int id)
        {
            var like = _context.postLikes.Where(x=>x.Id==id).Select(x=>new ShowLikeDTO
            {
                PictureUrl = x.User.ProfilePictureUrl??"",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL",
                UserName = x.User.FirstName + " " + x.User.LastName
            }).FirstOrDefault();
            return like;
        }
        public ShowCommentDTO GetComment(int id)
        {
            var comment = _context.postComments.Where(x => x.Id == id).Select(x => new ShowCommentDTO
            {
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Content = x.Content,
                UserName = x.User.FirstName + " " + x.User.LastName
            }).FirstOrDefault();
            return comment;
        }
        public List<ShowLikeDTO> GetLikeListOnPost(int id)
        {
            if (_context.Posts.Find(id) is null)
            {
                return null;
            }
            List<ShowLikeDTO> showLikes = _context.postLikes.Where(x => x.PostId == id).Select(x => new ShowLikeDTO
            {
                UserName = x.User.FirstName + " " + x.User.LastName,
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL"
            }).ToList();
            return showLikes;
        }
        LikesDetailsDTO LikesDetailsOnPost(int id)
        {
            var result = _context.postLikes
                .Where(x => x.PostId == id) 
                .GroupBy(x => x.Type)
                .OrderByDescending(g => g.Count())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
            var reurnedResult = new LikesDetailsDTO ();
            foreach ( var item in result )
            {
                reurnedResult.Count += item.Value;
                reurnedResult.OrderedType.Add(CheckLikeTypeAndReturnString(item.Key));
            }
            return reurnedResult;
        }
        string CheckLikeTypeAndReturnString(LikeType type)
        {
            return (type == LikeType.Love) ? "LOVE" : (type == LikeType.Care) ? "CARE" : "NORMAL";
        }
        LikeType CheckStringAndReturnLikeType(string type)
        {
            return (type == "CARE") ? LikeType.Care : (type == "LOVE") ? LikeType.Love : LikeType.Normal;
        }
        public ShowPostDTO Get(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return null;
            }

            return post switch
            {
                CoachPost coachPost => _context.CoachPosts.Select(p => new ShowCoachPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Coach.ProfilePictureUrl??"",
                    Name = p.Coach.FirstName + " " + p.Coach.FirstName,
                    CoachId = p.CoachId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                GymPost gymPost => _context.GymPosts.Select(p => new ShowGymPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Gym.PictureUrl ?? "",
                    Name = p.Gym.GymName,
                    GymId = p.GymId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                ShopPost shopPost => _context.ShopPosts.Select(p => new ShowShopPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Shop.PictureUrl ?? "",
                    Name = p.Shop.Name,
                    ShopId = p.ShopId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                }).Where(x => x.Id == id)
                .FirstOrDefault(),
                _ => null
            };
        }
        private Post GetPost(int id) => _context.Posts.Find(id);
    }
}
