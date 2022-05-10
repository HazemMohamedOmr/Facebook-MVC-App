using Facebook.Data;
using Facebook.Models;
using Facebook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Facebook.Controllers {
    public class UserController : Controller {
        private readonly ApplicationDBContext context;
        private IWebHostEnvironment WebHostEnvironment;

        public UserController(ApplicationDBContext cont, IWebHostEnvironment env) {
            context = cont;
            WebHostEnvironment = env;
        }

        public IActionResult Index() {
            if (HttpContext.Session.GetString("UserData") == null)
                return RedirectToAction("Login");
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserData"));
            List<User> allUsers = context.Users.ToList();
            List<Post> posts = context.Posts.Where(p => p.UserId == user.Id).ToList();
            List<PostLike> likes = context.PostLikes.Where(p => p.UserId == user.Id).ToList();
            List<PostComment> comments = context.PostComments.ToList();
            UserProfileViewModel userProfile = new UserProfileViewModel() {
                _Header = new _HeaderModel(),
                _ProfileThumb = new _ProfileThumbModel() {
                    user = user,
                    ThumbBtnVis = true
                },
                User = user,
                Post = new Post() {
                    User = user,
                },
                Like = new PostLike(),
                Comment = new PostComment(),
                PostModel = new _PostModel() {
                    Posts = posts,
                    Likes = likes,
                    Comments = comments,
                    Users = allUsers,
                }
            };
            return View(userProfile);
            //HttpContext.Session.Remove("UserData");
        }

        public IActionResult Login() {
            return View();
        }

        public IActionResult Register() {
            return View();
        }

        public IActionResult EditProfile() {
            if (HttpContext.Session.GetString("UserData") == null)
                return RedirectToAction("Login");
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserData"));
            EditViewModel editViewModel = new EditViewModel() {
                _Header = new _HeaderModel(),
                _ProfileThumb = new _ProfileThumbModel() {
                    user = user,
                    ThumbBtnVis = false
                },
                User = user
            };
            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser([Bind("FirstName,SecondName,Email,Password,ConfirmPassword,Phone")] User user) {
            if (ModelState.IsValid) {
                if (context.Users.FirstOrDefault(u => u.Email == user.Email) == null) {
                    user.ProfileCover = "\\img\\defaultCover.jpg";
                    user.ProfileImage = "\\img\\defaultProfile.jpg";

                    context.Users.Add(user);
                    context.SaveChanges();

                    return RedirectToAction("Login");
                }
                ViewBag.User = "Email Already Used.";
            }
            return View("Register", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetUser([Bind("Email,Password")] User user) {
            User newUser = context.Users.SingleOrDefault(x => x.Email == user.Email && x.Password == user.Password);
            if (newUser != null) {
                HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(newUser));
                return RedirectToAction("Index");
            }
            ViewBag.User = "User Not Found.";

            return View("Login", user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser([Bind("Id,FirstName,SecondName,Email,Password,ConfirmPassword,Phone,Country,City,ProfileImage,ProfileCover")]
        User user, IFormFile? profImg, IFormFile? profCover, int id) {
            //if (id != user.Id)
            //    return BadRequest();

            if (ModelState.IsValid) {
                if (profImg != null) {
                    if (user.ProfileImage != "\\img\\defaultProfile.jpg") {
                        string oldImgPath = WebHostEnvironment.WebRootPath + user.ProfileImage;
                        if (System.IO.File.Exists(oldImgPath))
                            System.IO.File.Delete(oldImgPath);
                    }

                    string imgExtenstion = Path.GetExtension(profImg.FileName);
                    Guid guid = Guid.NewGuid();
                    string imgName = guid + imgExtenstion;
                    string imgUrl = "\\img\\" + imgName;
                    user.ProfileImage = imgUrl;

                    string imgPath = WebHostEnvironment.WebRootPath + imgUrl;

                    FileStream imgStream = new FileStream(imgPath, FileMode.Create);
                    profImg.CopyTo(imgStream);
                    imgStream.Dispose();
                } else {
                    user.ProfileImage = "\\img\\defaultProfile.jpg";
                }

                if (profCover != null) {
                    if (user.ProfileCover != "\\img\\defaultCover.jpg") {
                        string oldImgPath = WebHostEnvironment.WebRootPath + user.ProfileCover;
                        if (System.IO.File.Exists(oldImgPath))
                            System.IO.File.Delete(oldImgPath);
                    }

                    string imgExtenstion = Path.GetExtension(profCover.FileName);
                    Guid guid = Guid.NewGuid();
                    string imgName = guid + imgExtenstion;
                    string imgUrl = "\\img\\" + imgName;
                    user.ProfileCover = imgUrl;

                    string imgPath = WebHostEnvironment.WebRootPath + imgUrl;

                    FileStream imgStream = new FileStream(imgPath, FileMode.Create);
                    profCover.CopyTo(imgStream);
                    imgStream.Dispose();
                } else {
                    user.ProfileCover = "\\img\\defaultCover.jpg";
                }

                HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(user));
                context.Users.Update(user);
                context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View("EditProfile", user);
        }
    }
}
