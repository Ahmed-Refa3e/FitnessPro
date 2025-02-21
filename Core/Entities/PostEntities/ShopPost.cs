using Core.Entities.ShopEntities;

namespace Core.Entities.PostEntities
{
    public class ShopPost : Post
    {
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
    }
}
