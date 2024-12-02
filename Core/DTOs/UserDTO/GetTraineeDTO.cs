namespace Core.DTOs.UserDTO
{
    public class GetTraineeDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public string Gender { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}
