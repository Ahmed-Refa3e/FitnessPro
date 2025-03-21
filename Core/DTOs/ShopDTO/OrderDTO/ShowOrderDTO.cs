namespace Core.DTOs.ShopDTO
{
    public class ShowOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsRecieved { get; set; }
        public List<ShowOrderItemDTO>? OrderItems { get; set; }
        public string UserName { get; set; }
        public ShowOrderDTO()
        {

        }
        /*public ShowOrderDTO()
        {
            
        }*/
    }
}
