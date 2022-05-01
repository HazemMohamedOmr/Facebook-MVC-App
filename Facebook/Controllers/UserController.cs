using Facebook.Data;
using Facebook.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Facebook.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext context;

        public UserController(ApplicationDBContext cont)
        {
            context = cont;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserData") == null)
                return RedirectToAction("Login");
            User user = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("UserData")); 
            return View(user);
            //HttpContext.Session.Remove("UserData");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddUser([Bind("FirstName,SecondName,Email,Password,ConfirmPassword,Phone")] User user)
        {
            if (ModelState.IsValid)
            {
                if(context.Users.FirstOrDefault(u => u.Email == user.Email) == null)
                {
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
        public IActionResult GetUser([Bind("Email,Password")] User user)
        {
            User newUser = context.Users.SingleOrDefault(x => x.Email == user.Email && x.Password == user.Password);
            if (newUser != null)
            {
                HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(newUser));
                return RedirectToAction("Index");
            }
            ViewBag.User = "User Not Found.";

            return View("Login", user);
        }
    }
}
