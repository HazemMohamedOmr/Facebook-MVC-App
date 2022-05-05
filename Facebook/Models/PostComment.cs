namespace Facebook.Models {
    public class PostComment {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
        public string CommentText { get; set; }
        public Post Post { get; set; }

    }
}
