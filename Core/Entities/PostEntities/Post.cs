namespace Core.Entities.PostEntities
{
    public class Post
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<PostPictureUrl>? PictureUrls { get; set; } = [];
        public List<PostLike>? Likes { get; set; } = [];
        public List<PostComment>? Comments { get; set; } = [];
    }
}
