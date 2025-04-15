using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Core.Interfaces.Repositories.PostRepositories;
using Core.Utilities;
using Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Repositories.PostRepositoy
{
    public abstract class GeneralPostRepository : IGeneralPostRepository
    {
        protected readonly FitnessContext _context;
        protected readonly string _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImagesForPosts");
        public GeneralPostRepository(FitnessContext context)
        {
            _context = context;
        }

        public abstract Task<IntResult> Add(AddPostDTO postDto);
        protected async Task<IntResult> AddPicturesToPost(AddPostDTO post, Post newPost)
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
