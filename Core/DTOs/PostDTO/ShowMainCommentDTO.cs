﻿namespace Core.DTOs.PostDTO
{
    public class ShowMainCommentDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PictureUrl { get; set; }
        public string Content { get; set; }
        public dynamic Date { get; set; }
        public LikesDetailsDTO LikesDetails { get; set; }
        public List<ShowCommentDTO> Comments { get; set; } = new List<ShowCommentDTO>();
    }
}
