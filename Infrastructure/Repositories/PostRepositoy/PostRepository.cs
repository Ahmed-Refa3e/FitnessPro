using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Enums;
using Core.Interfaces.Repositories.PostRepositories;
using Humanizer;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class PostRepository : IPostRepository
    {
        private readonly FitnessContext _context;
        protected readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImagesForPosts");

        public PostRepository(FitnessContext context)
        {
            _context = context;
        }
        public async Task<IntResult> DeletePost(int id, string userId)
        {
            var post = await _context.Posts
                    .Include(x => x.PictureUrls)
                    .Include(x => x.Likes)
                    .Include(x => x.Comments).ThenInclude(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (post is null)
                return new IntResult { Massage = "No post has this Id" };
            if (!await IsUserIsThePostOwner(post, userId))
                return new IntResult { Massage = "You are not the Owner of this page to delete" };
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.likes.RemoveRange(post.Likes);
                foreach (var comment in post.Comments)
                {
                    DeleteOneComment(comment);
                }
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }
        public async Task<IntResult> AddCommentOnPost(AddCommentDTO commentDTO, string userId)
        {
            var post = await FindPost(commentDTO.OwnerId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has tis Id." };
            }
            var comment = new PostComment { Content = commentDTO.Content, PostId = commentDTO.OwnerId, UserId = userId };
            post.Comments.Add(comment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = comment.Id };
        }
        public async Task<IntResult> DeleteComment(int commentId, string userId)
        {
            var comment = await _context.comments.FindAsync(commentId);
            if (comment is null)
            {
                return new IntResult { Massage = "No comment has this Id" };
            }
            if (!await IsUserAllowToDeleteComment(comment, userId))
            {
                return new IntResult { Massage = "You do not allow to delete this comment." };
            }
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await DeleteOneComment(comment);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }

            return new IntResult { Id = comment.Id };
        }
        public async Task<IntResult> AddLikeOnPost(AddLikeDTO likeDTO, string userId)
        {
            var post = await FindPost(likeDTO.OwnerId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has this Id." };
            }
            var like = await SearchWithUserIdAndPostId(userId, likeDTO.OwnerId);
            if (like is not null)
            {
                like.Type = CheckStringAndReturnLikeType(likeDTO.Type);
            }
            else
            {
                like = new PostLike
                {
                    Type = CheckStringAndReturnLikeType(likeDTO.Type),
                    PostId = likeDTO.OwnerId,
                    UserId = userId
                };
                post.Likes.Add(like);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = like.Id };
        }
        public async Task<IntResult> DeleteLikeFromPost(string userId, int postId)
        {
            var like = await SearchWithUserIdAndPostId(userId, postId);
            if (like is null)
            {
                return new IntResult { Massage = "you do not like this post yet to delete like" };
            }
            _context.postLikes.Remove(like);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }
        public async Task<ShowLikeDTO> GetLike(int id)
        {
            var like = await _context.postLikes.Where(x => x.Id == id).Select(x => new ShowLikeDTO
            {
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL",
                UserName = x.User.FirstName + " " + x.User.LastName
            }).FirstOrDefaultAsync();
            return like;
        }
        public async Task<ShowMainCommentDTO> GetComment(int id)
        {
            var comment = await _context.postComments.Where(x => x.Id == id).Select(x => new ShowMainCommentDTO
            {
                Id = x.Id,
                Date = x.Created,
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Content = x.Content,
                UserName = $"{x.User.FirstName} {x.User.LastName}",
                Comments = x.Comments.Select(c => new ShowCommentDTO
                {
                    Date = x.Created,
                    ID = c.Id,
                    PictureUrl = c.User.ProfilePictureUrl ?? "",
                    Content = c.Content,
                    UserName = $"{c.User.FirstName} {c.User.LastName}",
                    HaveComments = c.Comments.Any()
                }).ToList()
            }).FirstOrDefaultAsync();
            if (comment is null)
            {
                return null;
            }
            comment.LikesDetails = await LikesDetailsOnComment(id);
            comment.Date = DateHumanizeExtensions.Humanize((DateTime)comment.Date);
            foreach (var cmnt in comment.Comments)
            {
                cmnt.Date = DateHumanizeExtensions.Humanize((DateTime)cmnt.Date);
            }
            return comment;
        }
        public async Task<List<ShowLikeDTO>> GetLikeListOnPost(int id)
        {
            if (await _context.Posts.FindAsync(id) is null)
            {
                return null;
            }
            List<ShowLikeDTO> showLikes = await _context.postLikes.Where(x => x.PostId == id).Select(x => new ShowLikeDTO
            {
                UserName = x.User.FirstName + " " + x.User.LastName,
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL"
            }).ToListAsync();
            return showLikes;
        }
        string CheckLikeTypeAndReturnString(LikeType type)
        {
            return (type == LikeType.Love) ? "LOVE" : (type == LikeType.Care) ? "CARE" : "NORMAL";
        }
        LikeType CheckStringAndReturnLikeType(string type)
        {
            return (type == "CARE") ? LikeType.Care : (type == "LOVE") ? LikeType.Love : LikeType.Normal;
        }
        //Get Post Detais
        public async Task<ShowPostDTO> GetPost(int id)
        {
            var post = await FindPost(id);
            if (post is null) return null;

            ShowPostDTO newPost = post switch
            {
                CoachPost => await GetCoachPostDTO(id),
                GymPost => await GetGymPostDTO(id),
                ShopPost => await GetShopPostDTO(id),
                _ => null
            };
            if (newPost is null) return null;
            newPost.LikesDetails = await LikesDetailsOnPost(id);
            newPost.CreatedAt = DateHumanizeExtensions.Humanize((DateTime)newPost.CreatedAt);
            foreach (var comment in newPost.Comments)
            {
                comment.Date = DateHumanizeExtensions.Humanize((DateTime)comment.Date);
                foreach (var cmnt in comment.Comments)
                {
                    cmnt.Date = DateHumanizeExtensions.Humanize((DateTime)cmnt.Date);
                }
            }
            return newPost;
        }

        //comment
        public async Task<IntResult> AddLikeOnComment(AddLikeDTO likeDTO, string userId)
        {
            var comment = await _context.comments.FindAsync(likeDTO.OwnerId);
            if (comment is null)
            {
                return new IntResult { Massage = "No comment has this Id." };
            }
            var like = await SearchWithUserIdAndCommentId(userId, likeDTO.OwnerId);
            if (like is not null)
            {
                like.Type = CheckStringAndReturnLikeType(likeDTO.Type);
            }
            else
            {
                like = new CommentLike
                {
                    Type = CheckStringAndReturnLikeType(likeDTO.Type),
                    CommentId = likeDTO.OwnerId,
                    UserId = userId
                };
                comment.Likes.Add(like);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = like.Id };
        }
        public async Task<IntResult> DeleteLikeFromComment(string userId, int commentId)
        {
            var like = await SearchWithUserIdAndCommentId(userId, commentId);
            if (like is null)
            {
                return new IntResult { Massage = "you do not like this comment yet to delete like" };
            }
            _context.commentLikes.Remove(like);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }
        public async Task<IntResult> AddCommentOnComment(AddCommentDTO commentDTO, string userId)
        {
            var oldComment = await _context.comments.FindAsync(commentDTO.OwnerId);
            if (oldComment is null)
            {
                return new IntResult { Massage = "No comment has tis Id." };
            }
            var comment = new CommentComment { Content = commentDTO.Content, CommentId = commentDTO.OwnerId, UserId = userId };
            oldComment.Comments.Add(comment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = comment.Id };
        }
        public async Task<List<ShowLikeDTO>> GetLikeListOnComment(int id)
        {
            if (await _context.comments.FindAsync(id) is null)
            {
                return null;
            }
            List<ShowLikeDTO> showLikes = await _context.commentLikes.Where(x => x.CommentId == id).Select(x => new ShowLikeDTO
            {
                UserName = x.User.FirstName + " " + x.User.LastName,
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL"
            }).ToListAsync();
            return showLikes;
        }
        //private method that work for more than method
        private async Task<Post> FindPost(int id) => await _context.Posts.FindAsync(id);
        private async Task DeleteOneComment(Comment comment)
        {
            comment = await _context.comments
                .Include(x => x.Comments)
                .Include(x => x.Likes)
                .FirstOrDefaultAsync(x => x.Id == comment.Id);

            foreach (var like in comment.Likes)
                _context.likes.Remove(like);

            foreach (var comnt in comment.Comments)
                await DeleteOneComment(comnt);

            _context.comments.Remove(comment);
        }

        private async Task<PostLike> SearchWithUserIdAndPostId(string userId, int postId)
        {
            return await _context.postLikes
                .FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
        }

        async Task<CommentLike> SearchWithUserIdAndCommentId(string userId, int commentId)
        {
            return await _context.commentLikes.FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == commentId);
        }
        private async Task<ShowCoachPostDTO> GetCoachPostDTO(int id)
        {
            return await _context.CoachPosts
                .Select(p => new ShowCoachPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Coach.ProfilePictureUrl ?? "",
                    Name = $"{p.Coach.FirstName} {p.Coach.LastName}",
                    CoachId = p.CoachId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList(),
                    Comments = p.Comments.Select(x => new ShowMainCommentDTO
                    {
                        Date = x.Created,
                        Id = x.Id,
                        PictureUrl = x.User.ProfilePictureUrl ?? "",
                        Content = x.Content,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Comments = x.Comments.Select(x => new ShowCommentDTO
                        {
                            ID = x.Id,
                            PictureUrl = x.User.ProfilePictureUrl ?? "",
                            Date = x.Created,
                            Content = x.Content,
                            UserName = x.User.FirstName + " " + x.User.LastName,
                            HaveComments = x.Comments.Any()
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        private async Task<ShowGymPostDTO> GetGymPostDTO(int id)
        {
            return await _context.GymPosts
                .Select(p => new ShowGymPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Gym.PictureUrl ?? "",
                    Name = p.Gym.GymName,
                    GymId = p.GymId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList(),
                    Comments = p.Comments.Select(x => new ShowMainCommentDTO
                    {
                        Id = x.Id,
                        Date = x.Created,
                        PictureUrl = x.User.ProfilePictureUrl ?? "",
                        Content = x.Content,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Comments = x.Comments.Select(x => new ShowCommentDTO
                        {
                            ID = x.Id,
                            Date = x.Created,
                            PictureUrl = x.User.ProfilePictureUrl ?? "",
                            Content = x.Content,
                            UserName = x.User.FirstName + " " + x.User.LastName,
                            HaveComments = x.Comments.Any()
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        private async Task<ShowShopPostDTO> GetShopPostDTO(int id)
        {
            return await _context.ShopPosts
                .Select(p => new ShowShopPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPass = p.Shop.PictureUrl ?? "",
                    Name = p.Shop.Name,
                    ShopId = p.ShopId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList(),
                    Comments = p.Comments.Select(x => new ShowMainCommentDTO
                    {
                        Id = x.Id,
                        Date = x.Created,
                        PictureUrl = x.User.ProfilePictureUrl ?? "",
                        Content = x.Content,
                        UserName = x.User.FirstName + " " + x.User.LastName,
                        Comments = x.Comments.Select(x => new ShowCommentDTO
                        {
                            ID = x.Id,
                            Date = x.Created,
                            PictureUrl = x.User.ProfilePictureUrl ?? "",
                            Content = x.Content,
                            UserName = x.User.FirstName + " " + x.User.LastName,
                            HaveComments = x.Comments.Any()
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync(p => p.Id == id);
        }
        private async Task<LikesDetailsDTO> LikesDetailsOnComment(int id)
        {
            var result = await _context.commentLikes
                .Where(x => x.CommentId == id).ToListAsync();
            var dic = result
                .GroupBy(x => x.Type)
                .OrderByDescending(g => g.Count())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
            var returnedResult = new LikesDetailsDTO();
            foreach (var item in dic)
            {
                returnedResult.Count += item.Value;
                returnedResult.OrderedType.Add(CheckLikeTypeAndReturnString(item.Key));
            }
            return returnedResult;
        }
        private async Task<bool> IsUserAllowToDeleteComment(Comment comment, string userId)
        {
            if (comment is CommentComment)
            {
                var commentComment = await _context.commentComments
                    .Include(x => x.Comment)
                    .FirstOrDefaultAsync(x => x.Id == comment.Id);

                return commentComment.UserId == userId || commentComment.Comment.UserId == userId;
            }

            if (comment is PostComment)
            {
                var postComment = await _context.postComments
                    .Include(x => x.Post)
                    .FirstOrDefaultAsync(x => x.Id == comment.Id);

                return postComment.UserId == userId || await IsUserIsThePostOwner(postComment.Post, userId);
            }

            return false;
        }

        private async Task<bool> IsUserIsThePostOwner(Post post, string userId)
        {
            if (post is CoachPost)
            {
                var coachPost = await _context.CoachPosts.FirstOrDefaultAsync(x => x.Id == post.Id);
                if (coachPost?.CoachId != userId)
                    return false;
                return true;
            }
            if (post is GymPost)
            {
                var gymPost = await _context.GymPosts.Include(x => x.Gym).FirstOrDefaultAsync(x => x.Id == post.Id);
                if (gymPost?.Gym.CoachID != userId)
                    return false;
                return true;
            }
            if (post is ShopPost)
            {
                var shopPost = await _context.ShopPosts.Include(x => x.Shop).FirstOrDefaultAsync(x => x.Id == post.Id);
                if (shopPost?.Shop.OwnerID != userId)
                    return false;
                return true;
            }
            return false;
        }
        private async Task<LikesDetailsDTO> LikesDetailsOnPost(int id)
        {
            var result = await _context.postLikes
                .Where(x => x.PostId == id).ToListAsync();
            var dic = result
                .GroupBy(x => x.Type)
                .OrderByDescending(g => g.Count())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
            var returnedResult = new LikesDetailsDTO();
            foreach (var item in dic)
            {
                returnedResult.Count += item.Value;
                returnedResult.OrderedType.Add(CheckLikeTypeAndReturnString(item.Key));
            }
            return returnedResult;
        }
    }
}
