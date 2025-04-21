namespace Core.DTOs.ShopDTO
{
    public class ShowShopDTO
    {
        public int GymId { get; set; }
        public string GymName { get; set; }
        public string? PictureUrl { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? OwnerID { get; set; }
        public string? OwnerName { get; set; }
        public int FollowerNumber {  get; set; }
    }
}
