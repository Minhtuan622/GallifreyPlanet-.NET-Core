﻿namespace GallifreyPlanet.Models
{
    public class Reply
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}