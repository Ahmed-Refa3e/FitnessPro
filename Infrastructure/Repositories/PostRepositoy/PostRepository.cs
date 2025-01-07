using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImagesForPosts");
        public PostRepository(FitnessContext context)
        {
            _context = context;
        }

        public async Task<IntResult> Add(AddPostDTO postDto)
        {
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }

            Post newPost = postDto switch
            {
                AddGymPostDTO gymPostDto => new GymPost(gymPostDto),
                AddCoachPostDTO coachPostDto => new CoachPost(coachPostDto),
                AddShopPostDTO shopPostDto => new ShopPost(shopPostDto),
                _ => null
            };
            if (newPost == null)
            {
                return new IntResult { Massage = "Invalid post type" };
            }
            var uploadedFilePaths = new List<string>();
            _context.Posts.Add(newPost);
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!postDto.Images.IsNullOrEmpty())
                    {
                        foreach (var image in postDto.Images)
                        {
                            var filePath = chickImagePath(image);
                            if (!string.IsNullOrEmpty(filePath.Massage))
                            {
                                throw new Exception(filePath.Massage);
                            }
                            using (var stream = new FileStream(filePath.Id, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                                uploadedFilePaths.Add(filePath.Id);
                            }
                            newPost.PictureUrls.Add(new PostPictureUrl { Url = filePath.Id });
                        }
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    foreach (var path in uploadedFilePaths)
                    {
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    return new IntResult { Massage = ex.Message };
                }
            }
            return new IntResult { Id = newPost.Id };
        }
        public IntResult Delete(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return new IntResult { Massage = "No Post has this Id." };
            }
            var postWithPictures=_context.Posts.Include(x=>x.PictureUrls).Where(x=>x.Id==id).FirstOrDefault();
            var uploadedFilePaths=new List<string>();
            if (!postWithPictures.PictureUrls.IsNullOrEmpty())
            {
                uploadedFilePaths = post.PictureUrls.Select(x => x.Url).ToList();
            }
            var backupDirectory = Path.Combine(_storagePath, "Backup");
            try
            {
                if (!Directory.Exists(backupDirectory))
                {
                    Directory.CreateDirectory(backupDirectory);
                }
                var backupFiles = new List<string>();
                foreach (var path in uploadedFilePaths)
                {
                    if (File.Exists(path))
                    {
                        var backupPath = Path.Combine(backupDirectory, Path.GetFileName(path));
                        File.Move(path, backupPath);
                        backupFiles.Add(backupPath);
                    }
                }
                _context.Posts.Remove(post);
                _context.SaveChanges();
                foreach (var backupPath in backupFiles)
                {
                    if (File.Exists(backupPath))
                    {
                        File.Delete(backupPath);
                    }
                }
            }
            catch (Exception ex)
            {
                foreach (var backupPath in Directory.GetFiles(backupDirectory))
                {
                    var originalPath = Path.Combine(_storagePath, Path.GetFileName(backupPath));
                    File.Move(backupPath, originalPath);
                }

                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }


        /*
        public ShowPostDTO Get(int id)
        {
            var post = GetPost(id);
            if (post is null)
            {
                return null;
            }

            return post switch
            {
                CoachPost coachPost => new ShowCoachPostDTO(coachPost),
                GymPost gymPost => new ShowGymPostDTO(gymPost),
                ShopPost shopPost => new ShowShopPostDTO(shopPost),
                _ => null
            };
        }
        /*
        public List<ShowPostDTO> ShowPagination(string traineId, int page, int pageSize)
        {
            /*
            return posts.Select(post => post switch
            {
                CoachPost coachPost => new ShowCoachPostDTO(coachPost),
                GymPost gymPost => new ShowGymPostDTO(gymPost),
                ShopPost shopPost => new ShowShopPostDTO(shopPost),
                _ => throw new InvalidOperationException("Unknown post type")
            }).ToList();
            return null;
        }
    */
        private Post GetPost(int id) => _context.Posts.Find(id);
        StringResult chickImagePath(IFormFile file)
        {
            if (file is null)
            {
                return new StringResult();
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var contentType = file.ContentType.ToLower();
            if (!allowedExtensions.Contains(fileExtension) || !contentType.StartsWith("image/"))
            {
                return new StringResult { Massage = "Invalid file type, Only images are allowed" };
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_storagePath, fileName);
            return new StringResult { Id = filePath };
        }
    }

}
