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
            var shop = await _context.Shops.FindAsync(newPost.ShopId);
            if (shop is null||shop.OwnerID != userId)
            {
                return new IntResult { Massage = "Id is not valid." };
            }
            var shopPost = new ShopPost { Content = newPost.Content, ShopId = newPost.ShopId };
            await _context.ShopPosts.AddAsync(shopPost);
            return await AddPicturesToPost(post, shopPost);
        }
    }
}
