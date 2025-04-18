using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Enums;
using Core.Interfaces.Repositories.PostRepositories;
using Humanizer;
using Infrastructure.Data;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.IdentityModel.Tokens;

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
        public IntResult DeletePost(int id, string userId)
        {
            var post = _context.Posts
                    .Include(x => x.PictureUrls)
                    .Include(x => x.Likes)
                    .Include(x => x.Comments).ThenInclude(x => x.Comments)
                    .FirstOrDefault(x => x.Id == id);
            if (post is null)
                return new IntResult { Massage = "No post has this Id" };
            if (!IsUserIsThePostOwner(post, userId))
                return new IntResult { Massage = "You are not the Owner of this page to delete" };
            var uploadedFilePaths = post.PictureUrls?.Select(x => x.Url).ToList() ?? new List<string>();
            var backupDirectory = Path.Combine(_storagePath, "Backup");
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                Directory.CreateDirectory(backupDirectory);
                var backupFiles = BackupFiles(uploadedFilePaths, backupDirectory);
                _context.likes.RemoveRange(post.Likes);
                foreach (var comment in post.Comments)
                {
                    DeleteOneComment(comment);
                }
                _context.Posts.Remove(post);
                _context.SaveChanges();
                DeleteFiles(backupFiles);
                transaction.Commit();
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
                RestoreBackupFiles(backupDirectory);
                return new IntResult { Massage = ex.Message };
            }
        }
        public IntResult AddCommentOnPost(AddCommentDTO commentDTO, string userId)
        {
            var post = FindPost(commentDTO.OwnerId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has tis Id." };
            }
            var comment = new PostComment { Content = commentDTO.Content, PostId = commentDTO.OwnerId, UserId = userId };
            post.Comments.Add(comment);
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
        public IntResult DeleteComment(int commentId,string userId)
        {
            var comment = _context.comments.Find(commentId);
            if (comment is null)
            {
                return new IntResult { Massage = "No comment has this Id" };
            }
            if (!IsUserAllowToDeleteComment(comment,userId))
            {
                return new IntResult { Massage = "You do not allow to delete this comment." };
            }
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    DeleteOneComment(comment);
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    return new IntResult { Massage = ex.Message };
                }
            }
            return new IntResult { Id = comment.Id };
        }
        public IntResult AddLikeOnPost(AddLikeDTO likeDTO,string userId)
        {
            var post = FindPost(likeDTO.OwnerId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has this Id." };
            }
            var like = SearchWithUserIdAndPostId(userId, likeDTO.OwnerId);
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
            var like = SearchWithUserIdAndPostId(userId, postId);
            if (like is null)
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
        public ShowLikeDTO GetLike(int id)
        {
            var like = _context.postLikes.Where(x => x.Id == id).Select(x => new ShowLikeDTO
            {
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL",
                UserName = x.User.FirstName + " " + x.User.LastName
            }).FirstOrDefault();
            return like;
        }
        public ShowMainCommentDTO GetComment(int id)
        {
            var comment = _context.postComments.Where(x => x.Id == id).Select(x => new ShowMainCommentDTO
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
            }).ToList().FirstOrDefault();
            if (comment is null)
            {
                return null;
            }
            comment.LikesDetails = LikesDetailsOnComment(id);
            comment.Date = DateHumanizeExtensions.Humanize((DateTime)comment.Date);
            foreach (var cmnt in comment.Comments)
            {
                cmnt.Date = DateHumanizeExtensions.Humanize((DateTime)cmnt.Date);
            }
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
        string CheckLikeTypeAndReturnString(LikeType type)
        {
            return (type == LikeType.Love) ? "LOVE" : (type == LikeType.Care) ? "CARE" : "NORMAL";
        }
        LikeType CheckStringAndReturnLikeType(string type)
        {
            return (type == "CARE") ? LikeType.Care : (type == "LOVE") ? LikeType.Love : LikeType.Normal;
        }
        //Get Post Detais
        public ShowPostDTO GetPost(int id)
        {
            var post = FindPost(id);
            if (post is null) return null;

            ShowPostDTO newPost = post switch
            {
                CoachPost => GetCoachPostDTO(id),
                GymPost => GetGymPostDTO(id),
                ShopPost => GetShopPostDTO(id),
                _ => null
            };
            if (newPost is null) return null;
            newPost.LikesDetails = LikesDetailsOnPost(id);
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
        public IntResult AddLikeOnComment(AddLikeDTO likeDTO,string userId)
        {
            var comment = _context.comments.Find(likeDTO.OwnerId);
            if (comment is null)
            {
                return new IntResult { Massage = "No comment has this Id." };
            }
            var like = SearchWithUserIdAndCommentId(userId, likeDTO.OwnerId);
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
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = like.Id };
        }
        public IntResult DeleteLikeFromComment(string userId, int commentId)
        {
            var like = SearchWithUserIdAndCommentId(userId, commentId);
            if (like is null)
            {
                return new IntResult { Massage = "you do not like this comment yet to delete like" };
            }
            _context.commentLikes.Remove(like);
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
        public IntResult AddCommentOnComment(AddCommentDTO commentDTO,string userId)
        {
            var oldComment = _context.comments.Find(commentDTO.OwnerId);
            if (oldComment is null)
            {
                return new IntResult { Massage = "No comment has tis Id." };
            }
            var comment = new CommentComment { Content = commentDTO.Content, CommentId = commentDTO.OwnerId, UserId = userId };
            oldComment.Comments.Add(comment);
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
        public List<ShowLikeDTO> GetLikeListOnComment(int id)
        {
            if (_context.comments.Find(id) is null)
            {
                return null;
            }
            List<ShowLikeDTO> showLikes = _context.commentLikes.Where(x => x.CommentId == id).Select(x => new ShowLikeDTO
            {
                UserName = x.User.FirstName + " " + x.User.LastName,
                PictureUrl = x.User.ProfilePictureUrl ?? "",
                Type = (x.Type == LikeType.Love) ? "LOVE" : (x.Type == LikeType.Care) ? "CARE" : "NORMAL"
            }).ToList();
            return showLikes;
        }
        //private method that work for more than method
        private Post FindPost(int id) => _context.Posts.Find(id);
        private void DeleteOneComment(Comment comment)
        {
            comment = _context.comments.Include(x => x.Comments).Include(x => x.Likes).Where(x => x.Id == comment.Id).FirstOrDefault();
            foreach (var like in comment.Likes)
            {
                _context.likes.Remove(like);
            }
            foreach (var comnt in comment.Comments)
            {
                DeleteOneComment(comnt);
            }
            _context.comments.Remove(comment);
        }
        PostLike SearchWithUserIdAndPostId(string userId, int postId)
        {
            return _context.postLikes.Where(x => x.UserId == userId && x.PostId == postId).FirstOrDefault();
        }
        CommentLike SearchWithUserIdAndCommentId(string userId, int commentId)
        {
            return _context.commentLikes.Where(x => x.UserId == userId && x.CommentId == commentId).FirstOrDefault();
        }
        private ShowCoachPostDTO GetCoachPostDTO(int id)
        {
            return _context.CoachPosts
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
                .Where(p => p.Id == id)
                .FirstOrDefault();
        }
        private ShowGymPostDTO GetGymPostDTO(int id)
        {
            return _context.GymPosts
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
                .Where(p => p.Id == id)
                .FirstOrDefault();
        }
        private ShowShopPostDTO GetShopPostDTO(int id)
        {
            return _context.ShopPosts
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
                }).Where(p => p.Id == id)
                .FirstOrDefault();
        }
        private LikesDetailsDTO LikesDetailsOnComment(int id)
        {
            var result = _context.commentLikes
                .Where(x => x.CommentId == id).ToList();
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
        private List<string> BackupFiles(List<string> paths, string backupDir)
        {
            var backupFiles = new List<string>();
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    var backupPath = Path.Combine(backupDir, Path.GetFileName(path));
                    File.Move(path, backupPath);
                    backupFiles.Add(backupPath);
                }
            }
            return backupFiles;
        }
        private void DeleteFiles(List<string> paths)
        {
            foreach (var path in paths)
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }
        private void RestoreBackupFiles(string backupDir)
        {
            foreach (var backupPath in Directory.GetFiles(backupDir))
            {
                var originalPath = Path.Combine(_storagePath, Path.GetFileName(backupPath));
                File.Move(backupPath, originalPath);
            }
        }
        private bool IsUserAllowToDeleteComment(Comment comment, string userId)
        {
            if (comment is CommentComment)
            {
                var commentComment = _context.commentComments.Include(x => x.Comment).FirstOrDefault(x => x.Id == comment.Id);
                if (commentComment.UserId != userId && commentComment.Comment.UserId != userId)
                    return false;
                return true;
            }
            if (comment is PostComment)
            {
                var postComment = _context.postComments.Include(x => x.Post).FirstOrDefault(x => x.Id == comment.Id);
                if (postComment.UserId != userId && !IsUserIsThePostOwner(postComment.Post, userId))
                    return false;
                return true;
            }
            return false;
        }
        private bool IsUserIsThePostOwner(Post post, string userId)
        {
            if (post is CoachPost)
            {
                var coachPost = _context.CoachPosts.FirstOrDefault(x => x.Id == post.Id);
                if (coachPost?.CoachId != userId)
                    return false;
                return true;
            }
            if (post is GymPost)
            {
                var gymPost = _context.GymPosts.Include(x => x.Gym).FirstOrDefault(x => x.Id == post.Id);
                if (gymPost?.Gym.CoachID != userId)
                    return false;
                return true;
            }
            if (post is ShopPost)
            {
                var shopPost = _context.ShopPosts.Include(x => x.Shop).FirstOrDefault(x => x.Id == post.Id);
                if (shopPost?.Shop.OwnerID != userId)
                    return false;
                return true;
            }
            return false;
        }
        private LikesDetailsDTO LikesDetailsOnPost(int id)
        {
            var result = _context.postLikes
                .Where(x => x.PostId == id).ToList();
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
