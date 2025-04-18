using Core.DTOs.GeneralDTO;
using Core.DTOs.PostDTO;
using Core.Entities.PostEntities;
using Infrastructure.Data;

namespace Infrastructure.Repositories.PostRepositoy
{
    public class ShopPostRepository : GeneralPostRepository
    {
        public ShopPostRepository(FitnessContext context) : base(context)
        {
        }
        public override async Task<IntResult> Add(AddPostDTO post, string userId)
        {
            var newPost = post as AddShopPostDTO;
            if (post is null)
            {
                return new IntResult { Massage = "The post is not valid" };
            }
            if (_context.Shops.Find(newPost.ShopId).OwnerID != userId)
            {
                return new IntResult { Massage = "you are not the Owner of this shop to add post" };
            }
            var shopPost = new ShopPost { Content = newPost.Content, ShopId = newPost.ShopId };
            _context.ShopPosts.Add(shopPost);
            return await AddPicturesToPost(post, shopPost);
        }
    }
}
