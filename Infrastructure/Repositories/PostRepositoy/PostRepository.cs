using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Entities.ShopEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrastructure.Repositories.PostRepositoy
{
    public abstract class PostRepository : IPostRepository
    {
        protected readonly FitnessContext _context;
        protected readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImagesForPosts");
        public PostRepository(FitnessContext context)
        {
            _context = context;
        }

        public abstract Task<IntResult> Add(AddPostDTO postDto);
        
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
        protected async Task<IntResult> AddPicturesToPost(AddPostDTO post ,Post newPost)
        {
            var uploadedFilePaths = new List<string>();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (!post.Images.IsNullOrEmpty())
                    {
                        foreach (var image in post.Images)
                        {
                            var filePath = AddImageHelper.chickImagePath(image, _storagePath);

                            if (!string.IsNullOrEmpty(filePath.Massage))
                            {
                                throw new Exception(filePath.Massage);
                            }
                            using (var stream = new FileStream(filePath.Id, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }
                            uploadedFilePaths.Add(filePath.Id);
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
         Post GetPost(int id) => _context.Posts.Find(id);
    }

}
