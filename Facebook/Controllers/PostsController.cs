using Facebook.Data;
using Facebook.Models;
using Facebook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Facebook.Controllers {
    public class PostsController : Controller {
        private readonly ApplicationDBContext context;
        private IWebHostEnvironment WebHostEnvironment;
        public PostsController(ApplicationDBContext cont, IWebHostEnvironment env) {
            context = cont;
            WebHostEnvironment = env;
        }
        [HttpPost]
        public IActionResult CreatePost(Post post, IFormFile? postImg) {

            if (post.PostContent == null || postImg == null) {
                return RedirectToAction("Index", "User");

            }
            if (postImg != null) {
                string imgExtenstion = Path.GetExtension(postImg.FileName);
                Guid guid = Guid.NewGuid();
                string imgName = guid + imgExtenstion;
                string imgUrl = "\\img\\" + imgName;
                post.PostImage = imgUrl;
                string imgPath = WebHostEnvironment.WebRootPath + imgUrl;
                FileStream imgStream = new FileStream(imgPath, FileMode.Create);
                postImg.CopyTo(imgStream);
                imgStream.Dispose();
            }
            //post.PostId = post.UserId + context.Posts.ToList().Count();
            context.Posts.Add(post);
            context.SaveChanges();
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        public IActionResult React(PostLike like, bool likeStatus) {
            Post post = context.Posts.FirstOrDefault(p => p.PostId == like.PostId);
            PostLike postLike = context.PostLikes.SingleOrDefault(lid => lid.PostId == like.PostId && lid.UserId == like.UserId);
            if (postLike == null) {
                like.UserId = like.UserId;
                like.PostId = like.PostId;
                like.LikeStatus = likeStatus;
                if (likeStatus == true) {
                    post.NumOfLike = post.NumOfLike + 1;
                } else if (likeStatus == false) {
                    post.NumOfDisLike = post.NumOfDisLike + 1;
                }
                context.PostLikes.Add(like);
                context.Posts.Update(post);
                context.SaveChanges();
            } else if (postLike.LikeStatus == likeStatus) {
                if (likeStatus == true) {
                    post.NumOfLike = post.NumOfLike - 1;
                } else if (likeStatus == false) {
                    post.NumOfDisLike = post.NumOfDisLike - 1;
                }
                context.PostLikes.Remove(postLike);
                context.Posts.Update(post);
                context.SaveChanges();
            } else {
                if (likeStatus == true) {
                    post.NumOfDisLike = post.NumOfDisLike - 1;
                    post.NumOfLike = post.NumOfLike + 1;

                } else if (likeStatus == false) {
                    post.NumOfLike = post.NumOfLike - 1;
                    post.NumOfDisLike = post.NumOfDisLike + 1;
                }
                postLike.LikeStatus = likeStatus;
                context.PostLikes.Update(postLike);
                context.Posts.Update(post);
                context.SaveChanges();
            }
            return RedirectToAction("Index", "User");
        }
        [HttpPost]
        public IActionResult AddComment(PostComment comment) {
            Post post = context.Posts.FirstOrDefault(p => p.PostId == comment.PostId);
            if (comment.CommentText == null) {
                return RedirectToAction("Index", "User");
            }
            post.NumOfComment = post.NumOfComment + 1;
            context.PostComments.Add(comment);
            context.Posts.Update(post);
            context.SaveChanges();
            return RedirectToAction("Index", "User");
        }

    }

}


