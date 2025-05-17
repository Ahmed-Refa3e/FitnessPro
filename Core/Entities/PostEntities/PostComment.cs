namespace Core.Entities.PostEntities
{
    public class PostComment : Comment
    {
        public int? PostId { get; set; }
        public Post? Post { get; set; }
    }
}
