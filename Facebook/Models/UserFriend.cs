namespace Facebook.Models {
    public class UserFriend {
        public int FriendId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
