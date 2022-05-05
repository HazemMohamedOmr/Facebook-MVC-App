namespace Facebook.Models {
    public class Post {

        public int PostId { get; set; }
        public int UserId { get; set; }
        public string PostContent { get; set; }
        public DateTime PostDate { get; set; } = DateTime.Now;
        public int PostStatus { get; set; } = 1;
        public List<PostLike>? Likes { get; set; }
        public List<PostComment>? Comments { get; set; }
        public User User { get; set; }

    }
}
