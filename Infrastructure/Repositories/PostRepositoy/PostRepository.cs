using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.Identity;
using Core.Entities.PostEntities;
using Core.Enums;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Interfaces.Services;
using Core.Utilities;
using Humanizer;
using Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories.PostRepositoy
{
    public class PostRepository : IPostRepository
    {
        private readonly FitnessContext _context;
        private readonly IBlobService _blobService;
        public PostRepository(FitnessContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }
        public async Task<List<ShowExternalFormOfShopPostDTO>> GetPostsOfShop(int shopId, int pageNumber, string userId = "")
        {
            if (await _context.Shops.FindAsync(shopId) == null)
            {
                return null;
            }
            pageNumber = Math.Max(1, pageNumber);
            var posts = await _context.ShopPosts.Where(x => x.ShopId == shopId)
                .Select(p => new ShowExternalFormOfShopPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    IsYourPost = p.Shop.OwnerID == userId,
                    PhotoPassOfOwner = p.Shop.PictureUrl ?? "",
                    Name = p.Shop.Name,
                    ShopId = p.ShopId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                })
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * 10).Take(10)
                .ToListAsync();
            //var likes = _context.ShopPosts.Include(x => x.Likes).Where(x => x.ShopId == shopId).SelectMany(x => x.Likes);
            var postIds = posts.Select(p => p.Id).ToList();
            var ids = postIds.Any()
                    ? string.Join(",", postIds)
                    : "-1";
            var likes = await _context.postLikes
                .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
                .ToListAsync();
            foreach (var post in posts)
            {
                post.CreatedAt = DateHumanizeExtensions.Humanize((DateTime)post.CreatedAt);
                post.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
                var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
                if (like is not null && userId != "")
                {
                    post.IsLikedByYou = true;
                    post.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
                }
            }
            return posts;
        }
        private async Task<LikesDetailsDTO> LikesDetailsOnPost(List<PostLike> result)
        {
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
        public async Task<List<ShowExternalFormOfGymPostDTO>> GetPostsOfGym(int gymId, int pageNumber, string userId = "")
        {
            if (await _context.Gyms.FindAsync(gymId) == null)
            {
                return null;
            }
            pageNumber = Math.Max(1, pageNumber);
            var posts = await _context.GymPosts.Where(x => x.GymId == gymId)
                .Select(p => new ShowExternalFormOfGymPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPassOfOwner = p.Gym.PictureUrl ?? "",
                    IsYourPost = p.Gym.CoachID == userId,
                    Name = p.Gym.GymName,
                    GymId = p.GymId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                })
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * 10).Take(10)
                .ToListAsync();
            //var likes = _context.GymPosts.Include(x => x.Likes).Where(x => x.GymId == gymId).SelectMany(x => x.Likes);
            var postIds = posts.Select(p => p.Id).ToList();
            var ids = postIds.Any()
                    ? string.Join(",", postIds)
                    : "-1";
            var likes = await _context.postLikes
                .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
                .ToListAsync();
            foreach (var post in posts)
            {
                post.CreatedAt = DateHumanizeExtensions.Humanize((DateTime)post.CreatedAt);
                post.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
                var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
                if (like is not null && userId != "")
                {
                    post.IsLikedByYou = true;
                    post.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
                }
            }
            return posts;
        }
        public async Task<List<ShowExternalFormOfCoachPostDTO>> GetPostsOfCoach(string coachId, int pageNumber, string userId = "")
        {
            if (await _context.Users.FindAsync(coachId) == null)
            {
                return null;
            }
            pageNumber = Math.Max(1, pageNumber);
            var posts = await _context.CoachPosts.Where(x => x.CoachId == coachId)
                .Select(p => new ShowExternalFormOfCoachPostDTO
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    PhotoPassOfOwner = p.Coach.ProfilePictureUrl ?? "",
                    Name = $"{p.Coach.FirstName} {p.Coach.LastName}",
                    IsYourPost = coachId == userId,
                    CoachId = p.CoachId,
                    PictureUrls = p.PictureUrls.Select(x => x.Url).ToList()
                })
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * 10).Take(10)
                .ToListAsync();
            //var likes = _context.CoachPosts.Include(x => x.Likes).Where(x => x.CoachId == coachId).SelectMany(x => x.Likes);
            var postIds = posts.Select(p => p.Id).ToList();
            var ids = postIds.Any()
                    ? string.Join(",", postIds)
                    : "-1";
            var likes = await _context.postLikes
                .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
                .ToListAsync();
            foreach (var post in posts)
            {
                post.CreatedAt = DateHumanizeExtensions.Humanize((DateTime)post.CreatedAt);
                post.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
                var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
                if (like is not null && userId != "")
                {
                    post.IsLikedByYou = true;
                    post.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
                }
            }
            return posts;
        }
        public async Task<List<ShowGeneralFormOfPostDTO>> GetPostsForUserFromFollowers(int pageNumber, string userId)
        {
            pageNumber = Math.Max(1, pageNumber);
            if (string.IsNullOrEmpty(userId) || await _context.Users.FindAsync(userId) is null)
                return await GetRandoPosts(pageNumber, userId);
            var gymIds = await _context.gymFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.GymId)
                .ToListAsync();

            var shopIds = await _context.ShopFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.ShopId)
                .ToListAsync();

            var followedUserIds = await _context.userFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingId)
                .ToListAsync();
            var followedUserIdsString = followedUserIds.Any()
                ? $"'{string.Join("','", followedUserIds)}'"
                : "'-1'";
            var gymIdsString = gymIds.Any()
                ? string.Join(",", gymIds)
                : "-1";
            var shopIdsString = shopIds.Any()
                ? string.Join(",", shopIds)
                : "-1";
            var countSql = $@"
                                SELECT COUNT(*) AS Count FROM (
                                    SELECT Id FROM Posts WHERE CoachId IN ({followedUserIdsString})
                                    UNION
                                    SELECT Id FROM Posts WHERE GymId IN ({gymIdsString})
                                    UNION
                                    SELECT Id FROM Posts WHERE ShopId IN ({shopIdsString})
                                ) AS AllPosts
                                ";

            var countResult = await _context.countResults
                .FromSqlRaw(countSql)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            int count = countResult?.Count ?? 0;

            var pageSize = 10;
            (pageNumber, pageSize) = await PaginationHelper.NormalizePaginationWithCountAsync(count, pageNumber, pageSize);
            if (pageSize == -1)
            {
                return await GetRandoPosts(pageNumber, userId);
            }

            int offset = (pageNumber - 1) * pageSize;

            var sql = $@"
                            SELECT
                                p.Id,
                                p.Content,
                                p.CreatedAt,
                                p.CoachId,
                                p.GymId,
                                p.ShopId,
                                CASE p.PostType
                                  WHEN 'CoachPost' THEN c.ProfilePictureUrl
                                  WHEN 'GymPost'   THEN g.PictureUrl
                                  WHEN 'ShopPost'  THEN s.PictureUrl
                                  ELSE '' END AS PhotoPass,
                                CASE p.PostType
                                  WHEN 'CoachPost' THEN c.FirstName + ' ' + c.LastName
                                  WHEN 'GymPost'   THEN g.GymName
                                  WHEN 'ShopPost'  THEN s.Name
                                  ELSE '' END AS EntityName,
                                p.PostType AS SourceType,
                                CASE
                                  WHEN p.PostType = 'CoachPost' AND p.CoachId = @userId THEN CAST(1 AS bit)
                                  WHEN p.PostType = 'GymPost'   AND g.CoachID = @userId THEN CAST(1 AS bit)
                                  WHEN p.PostType = 'ShopPost'  AND s.OwnerID = @userId THEN CAST(1 AS bit)
                                  ELSE CAST(0 AS bit)
                                END AS IsYourPost
                            FROM Posts p
                            LEFT JOIN AspNetUsers c ON p.CoachId = c.Id
                            LEFT JOIN Gyms        g ON p.GymId    = g.GymId
                            LEFT JOIN Shops       s ON p.ShopId   = s.Id
                            WHERE
                                (p.PostType = 'CoachPost' AND p.CoachId IN ({followedUserIdsString}))
                             OR (p.PostType = 'GymPost'   AND p.GymId   IN ({gymIdsString}))
                             OR (p.PostType = 'ShopPost'  AND p.ShopId  IN ({shopIdsString}))
                            ORDER BY p.CreatedAt DESC
                            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                            ";

            var parameters = new[]
            {
        new SqlParameter("@userId",    userId),
        new SqlParameter("@offset",    offset),
        new SqlParameter("@pageSize",  pageSize)
    };

            var posts = await _context.RawPostDTOs
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
            var postIds = posts.Select(p => p.Id).ToList();
            var ids = postIds.Any()
                ? $"'{string.Join("','", postIds)}'"
                : "'-1'";
            var likes = await _context.postLikes
                .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
                .ToListAsync();
            var urls = await _context.PictureUrls.FromSqlRaw($"SELECT * FROM PostPictureUrl WHERE PostId IN ({ids})")
                .ToListAsync();
            var allPosts = new List<ShowGeneralFormOfPostDTO>();
            foreach (var post in posts)
            {
                var newPost = new ShowGeneralFormOfPostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = DateHumanizeExtensions.Humanize(post.CreatedAt),
                    IsYourPost = post.IsYourPost,
                    PhotoPass = post.PhotoPass ?? "",
                    Name = post.EntityName ?? "",
                    SourceType = post.SourceType == "CoachPost" ? PageType.COACH : post.SourceType == "GymPost" ? PageType.GYM : PageType.SHOP
                };
                if (newPost.SourceType == PageType.COACH)
                {
                    newPost.SourceId = post.CoachId ?? "";
                }
                else if (newPost.SourceType == PageType.GYM)
                {
                    newPost.SourceId = post.GymId;
                }
                else
                {
                    newPost.SourceId = post.ShopId;
                }
                newPost.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
                var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
                if (like is not null && userId != "")
                {
                    newPost.IsLikedByYou = true;
                    newPost.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
                }
                newPost.PictureUrls = urls.Where(x => x.PostId == newPost.Id).Select(x => x.Url).ToList();
                allPosts.Add(newPost);
            }
            return allPosts;
        }

        private async Task<List<ShowGeneralFormOfPostDTO>> GetRandoPosts(int pageNumber, string userId)
        {
            var pageSize = 10;
            int offset = (pageNumber - 1) * pageSize;

            var sql = $@"
                            SELECT
                                p.Id,
                                p.Content,
                                p.CreatedAt,
                                p.CoachId,
                                p.GymId,
                                p.ShopId,
                                CASE p.PostType
                                  WHEN 'CoachPost' THEN c.ProfilePictureUrl
                                  WHEN 'GymPost'   THEN g.PictureUrl
                                  WHEN 'ShopPost'  THEN s.PictureUrl
                                  ELSE '' END AS PhotoPass,
                                CASE p.PostType
                                  WHEN 'CoachPost' THEN c.FirstName + ' ' + c.LastName
                                  WHEN 'GymPost'   THEN g.GymName
                                  WHEN 'ShopPost'  THEN s.Name
                                  ELSE '' END AS EntityName,
                                p.PostType AS SourceType,
                                CASE
                                  WHEN p.PostType = 'CoachPost' AND p.CoachId = @userId THEN CAST(1 AS bit)
                                  WHEN p.PostType = 'GymPost'   AND g.CoachID = @userId THEN CAST(1 AS bit)
                                  WHEN p.PostType = 'ShopPost'  AND s.OwnerID = @userId THEN CAST(1 AS bit)
                                  ELSE CAST(0 AS bit)
                                END AS IsYourPost
                            FROM Posts p
                            LEFT JOIN AspNetUsers c ON p.CoachId = c.Id
                            LEFT JOIN Gyms        g ON p.GymId    = g.GymId
                            LEFT JOIN Shops       s ON p.ShopId   = s.Id
                            ORDER BY p.CreatedAt DESC
                            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;
                            ";

            var parameters = new[]
            {
    new SqlParameter("@userId", userId),
    new SqlParameter("@offset", offset),
    new SqlParameter("@pageSize", pageSize)
            };

            var posts = await _context.RawPostDTOs
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync();
            var postIds = posts.Select(p => p.Id).ToList();
            var ids = postIds.Any()
                ? $"'{string.Join("','", postIds)}'"
                : "'-1'";
            var likes = await _context.postLikes
                .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
                .ToListAsync();
            var urls = await _context.PictureUrls.FromSqlRaw($"SELECT * FROM PostPictureUrl WHERE PostId IN ({ids})")
                .ToListAsync();
            var allPosts = new List<ShowGeneralFormOfPostDTO>();
            foreach (var post in posts)
            {
                var newPost = new ShowGeneralFormOfPostDTO
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = DateHumanizeExtensions.Humanize(post.CreatedAt),
                    IsYourPost = post.IsYourPost,
                    PhotoPass = post.PhotoPass ?? "",
                    Name = post.EntityName ?? "",
                    SourceType = post.SourceType == "CoachPost" ? PageType.COACH : post.SourceType == "GymPost" ? PageType.GYM : PageType.SHOP
                };
                if (newPost.SourceType == PageType.COACH)
                {
                    newPost.SourceId = post.CoachId ?? "";
                }
                else if (newPost.SourceType == PageType.GYM)
                {
                    newPost.SourceId = post.GymId;
                }
                else
                {
                    newPost.SourceId = post.ShopId;
                }
                newPost.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
                var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
                if (like is not null && userId != "")
                {
                    newPost.IsLikedByYou = true;
                    newPost.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
                }
                newPost.PictureUrls = urls.Where(x => x.PostId == newPost.Id).Select(x => x.Url).ToList();
                allPosts.Add(newPost);
            }
            return allPosts;
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
                    await DeleteOneComment(comment);
                }
                _context.PictureUrls.RemoveRange(post.PictureUrls);
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                foreach (var url in post.PictureUrls.Select(x => x.Url))
                {
                    await _blobService.DeleteImageAsync(url);
                }
                return new IntResult { Id = 1 };
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
        }
        public async Task<IntResult> AddCommentOnPost(AddCommentOnPostDTO commentDTO, string userId)
        {
            var post = await FindPost(commentDTO.PostId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has tis Id." };
            }
            var comment = new PostComment { Content = commentDTO.Content, PostId = commentDTO.PostId, UserId = userId };
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
        public async Task<IntResult> AddLikeOnPost(AddLikeOnPostDTO likeDTO, string userId)
        {
            var post = await FindPost(likeDTO.PostId);
            if (post is null)
            {
                return new IntResult { Massage = "No post has this Id." };
            }
            var like = await SearchWithUserIdAndPostId(userId, likeDTO.PostId);
            if (like is not null)
            {
                like.Type = CheckStringAndReturnLikeType(likeDTO.Type);
            }
            else
            {
                like = new PostLike
                {
                    Type = CheckStringAndReturnLikeType(likeDTO.Type),
                    PostId = likeDTO.PostId,
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
                IsCoach = x.User is Coach,
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
                UserId = x.UserId,
                IsCoach=x.User is Coach,
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
        public async Task<IntResult> AddLikeOnComment(AddLikeOnCommentDTO likeDTO, string userId)
        {
            var comment = await _context.comments.FindAsync(likeDTO.CommentId);
            if (comment is null)
            {
                return new IntResult { Massage = "No comment has this Id." };
            }
            var like = await SearchWithUserIdAndCommentId(userId, likeDTO.CommentId);
            if (like is not null)
            {
                like.Type = CheckStringAndReturnLikeType(likeDTO.Type);
            }
            else
            {
                like = new CommentLike
                {
                    Type = CheckStringAndReturnLikeType(likeDTO.Type),
                    CommentId = likeDTO.CommentId,
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
        public async Task<IntResult> AddCommentOnComment(AddCommentOnCommentDTO commentDTO, string userId)
        {
            var oldComment = await _context.comments.FindAsync(commentDTO.CommentId);
            if (oldComment is null)
            {
                return new IntResult { Massage = "No comment has tis Id." };
            }
            var comment = new CommentComment { Content = commentDTO.Content, CommentId = commentDTO.CommentId, UserId = userId };
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
                IsCoach = x.User is Coach,
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
/*
public async Task<List<ShowGeneralFormOfPostDTO>> GetPostsForUserFromFollowersASLY(int pageNumber, string userId = "")
{
    if (string.IsNullOrEmpty(userId) || await _context.Users.FindAsync(userId) is null)
    {
        return await GetRandoPosts(pageNumber);
    }
    int part = (pageNumber - 1) / 3;
    var gymIds = await _context.gymFollows
        .Where(f => f.FollowerId == userId)
        .Select(f => f.GymId)
        .ToListAsync();

    var shopIds = await _context.ShopFollows
        .Where(f => f.FollowerId == userId)
        .Select(f => f.ShopId)
        .ToListAsync();

    var followedUserIds = await _context.userFollows
        .Where(f => f.FollowerId == userId)
        .Select(f => f.FollowingId)
        .ToListAsync();

    //await Task.WhenAll(gymIdsTask, shopIdsTask, userIdsTask);

    /*var gymIds = gymIdsTask.Result;
    var shopIds = shopIdsTask.Result;
    var followedUserIds = userIdsTask.Result;*/
/*var countOfCoachPost =await (
                from post in _context.CoachPosts
                join follow in _context.userFollows on post.CoachId equals follow.FollowingId
                where follow.FollowerId == userId
                select post.Id
                    ).CountAsync();
var countOfGymPost =await (
                    from post in _context.GymPosts
                    join follow in _context.gymFollows on post.GymId equals follow.GymId
                    where follow.FollowerId == userId
                    select post.Id
                    ).CountAsync();
var countOfShopPost =await (
                        from post in _context.ShopPosts
                        join follow in _context.ShopFollows on post.ShopId equals follow.ShopId
                        where follow.FollowerId == userId
                        select post.Id
        ).CountAsync();
//await Task.WhenAll(countOfCoachPost, countOfGymPost, countOfShopPost);
var count = countOfCoachPost + countOfGymPost + countOfShopPost;
var followedUserIdsString = followedUserIds.Any()
    ? $"'{string.Join("','", followedUserIds)}'"
    : "'-1'";
var gymIdsString = gymIds.Any()
    ? string.Join(",", gymIds)
    : "-1";
var shopIdsString = shopIds.Any()
    ? string.Join(",", shopIds)
    : "-1";
var countQuery = $@"
SELECT COUNT(*) FROM (
SELECT Id FROM Posts WHERE CoachId IN ({followedUserIdsString})
UNION
SELECT Id FROM Posts WHERE GymId IN ({gymIdsString})
UNION
SELECT Id FROM Posts WHERE ShopId IN ({shopIdsString})
) AS AllPosts
";
int count;

await using var command = _context.Database.GetDbConnection().CreateCommand();
command.CommandText = countQuery;

try
{
    await _context.Database.OpenConnectionAsync();
    var result = await command.ExecuteScalarAsync();
    count = Convert.ToInt32(result);
}
finally
{
    await _context.Database.CloseConnectionAsync();
}
var pageSize = 10;
(pageNumber, pageSize) = await PaginationHelper.NormalizePaginationWithCountAsync(count, pageNumber, pageSize);
if (pageSize == -1)
{
    return await GetRandoPosts(pageNumber);
}
var coachPosts = await (
                        from post in _context.CoachPosts
                        join follow in _context.userFollows on post.CoachId equals follow.FollowingId
                        where follow.FollowerId == userId
                        orderby post.CreatedAt descending
                        select new ShowGeneralFormOfPostDTO
                        {
                            Id = post.Id,
                            Content = post.Content,
                            IsYourPost = post.CoachId == userId,
                            CreatedAt = post.CreatedAt,
                            PhotoPass = post.Coach.ProfilePictureUrl ?? "",
                            Name = post.Coach.FirstName + " " + post.Coach.LastName,
                            SourceId = post.CoachId,
                            PictureUrls = post.PictureUrls.Select(x => x.Url).ToList(),
                            SourceType = PageType.COACH
                        }
                        ).Skip(10 * part).Take(10).ToListAsync();

var gymPosts = await (
                    from post in _context.GymPosts
                    join follow in _context.gymFollows on post.GymId equals follow.GymId
                    where follow.FollowerId == userId
                    orderby post.CreatedAt descending
                    select new ShowGeneralFormOfPostDTO
                    {
                        Id = post.Id,
                        Content = post.Content,
                        CreatedAt = post.CreatedAt,
                        IsYourPost = post.Gym.CoachID == userId,
                        PhotoPass = post.Gym.PictureUrl ?? "",
                        Name = post.Gym.GymName,
                        SourceId = post.GymId,
                        PictureUrls = post.PictureUrls.Select(x => x.Url).ToList(),
                        SourceType = PageType.GYM
                    }
                    ).Skip(10 * part).Take(10).ToListAsync();

var shopPosts = await (
                        from post in _context.ShopPosts
                        join follow in _context.ShopFollows on post.ShopId equals follow.ShopId
                        where follow.FollowerId == userId
                        orderby post.CreatedAt descending
                        select new ShowGeneralFormOfPostDTO
                        {
                            Id = post.Id,
                            Content = post.Content,
                            CreatedAt = post.CreatedAt,
                            IsYourPost = post.Shop.OwnerID == userId,
                            PhotoPass = post.Shop.PictureUrl ?? "",
                            Name = post.Shop.Name,
                            SourceId = post.ShopId,
                            PictureUrls = post.PictureUrls.Select(x => x.Url).ToList(),
                            SourceType = PageType.SHOP
                        }
                        ).Skip(10 * part).Take(10).ToListAsync();
//await Task.WhenAll(coachPosts, gymPosts, shopPosts);


var allPosts = coachPosts
    .Concat(gymPosts)
    .Concat(shopPosts)
    .OrderByDescending(p => p.CreatedAt).Skip(((pageSize - 1) % 3) * 10)
    .Take(10)
    .ToList();

/*var allPosts = posts.Select(x => new ShowGeneralFormOfPostDTO
{
    Id = x.Id,
    CreatedAt = x.CreatedAt,
    Content = x.Content,
    Name = x.EntityName,
    SourceId = (x.CoachId ? (x.GymId ?? x.ShopId))
}).ToList();
if (allPosts.Count == 0)
{
    return await GetRandoPosts(pageNumber);
}
var postIds = allPosts.Select(p => p.Id).ToList();
var ids = postIds.Any() ? $"'{string.Join("','", postIds)}'" : "'-1'";
var likes = await _context.postLikes
    .FromSqlRaw($"SELECT * FROM Likes WHERE PostId IN ({ids})")
    .ToListAsync();
foreach (var post in allPosts)
{
    post.CreatedAt = DateHumanizeExtensions.Humanize(post.CreatedAt);
    post.LikesDetails = await LikesDetailsOnPost(likes.Where(x => x.PostId == post.Id).ToList());
    var like = likes.FirstOrDefault(x => x.PostId == post.Id && x.UserId == userId);
    if (like is not null && userId != "")
    {
        post.IsLikedByYou = true;
        post.LikeType = (like.Type == LikeType.Love) ? "LOVE" : (like.Type == LikeType.Care) ? "CARE" : "NORMAL";
    }
}
return allPosts;
}*/