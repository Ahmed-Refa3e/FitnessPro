namespace Core.Entities.Identity
{
    public class Coach : ApplicationUser
    {
        public Gym? Gym { get; set; }
        public ICollection<OnlineTraining>? OnlineTrainings { get; set; }
        public bool AvailableForOnlineTraining { get; set; }

    }
}
