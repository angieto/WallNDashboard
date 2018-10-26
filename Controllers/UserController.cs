using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheWall.Models;
// add these lines for validation
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
// add these lines for session
using Microsoft.AspNetCore.Http;

namespace TheWall.Controllers
{
    public class UserController : Controller
    {
        // Context Service
        private WallContext dbContext;
        public UserController(WallContext context)
        {
            dbContext = context;
        }

        // Routes
        [HttpGet("/dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) 
            {
                return RedirectToAction("Index", "Home");
            } 
            else 
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                ViewBag.UserId = UserId;
                List<User> AllUsers = dbContext.Users.OrderBy(user => user.CreatedAt).ToList();
                ViewBag.AllUsers = AllUsers;
                return View("Dashboard");
            }
        }

        [HttpGet("/users/{id}")]
        public IActionResult Detail(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) 
            {
                return RedirectToAction("Index", "Home");
            } 
            else 
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                ViewBag.UserId = UserId;
                // Selected User
                User SelectedUser = dbContext.Users.Include(user => user.CreatedMessages).FirstOrDefault(user => user.UserId == id);
                ViewBag.SelectedUser = SelectedUser;
                // All messages
                List<Message> AllMsg = dbContext.Messages
                                        .Include(msg => msg.User)
                                        .Include(msg => msg.Comments)
                                        .ThenInclude(cmt => cmt.Message)
                                        .Include(msg => msg.Comments)
                                        .ThenInclude(cmt => cmt.User)
                                        .Where(msg => msg.Recipient.UserId == SelectedUser.UserId)
                                        .OrderByDescending(msg => msg.CreatedAt)
                                        .ToList();
                ViewBag.AllMsg = AllMsg;
                // Calculate dates apart
                @ViewBag.Now = DateTime.Now;
                System.Console.WriteLine("DATETIME.NOW" + DateTime.Now);
                return View("Detail");
            }
        }

        [HttpPost("/createMsg/{recipientId}")]
        public IActionResult CreateMsg(Message message, int recipientId)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(message);
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { id = recipientId });
            }
            return View("Detail", new { id = recipientId });
        }

        [HttpPost("/createCmt/{recipientId}")]
        public IActionResult CreateCmt(Comment comment, int recipientId)
        {
            // form a relationship btw the msg and the user who commented...
            System.Console.WriteLine($"The msgId is {comment.MessageId}");
            int? UserId = HttpContext.Session.GetInt32("UserId");
            Message selectedMsg = dbContext.Messages
                                .Include(m => m.Comments)
                                .ThenInclude(cmt => cmt.User)
                                .Include(m => m.Comments)
                                .ThenInclude(cmt => cmt.Message)
                                .FirstOrDefault(m => m.MessageId == comment.MessageId);
            Comment connection = new Comment()
            {
                Cmt = comment.Cmt,
                UserId = (int)UserId,
                MessageId = comment.MessageId
            };
            dbContext.Comments.Add(connection);
            dbContext.SaveChanges();
            return RedirectToAction("Detail", new { id = recipientId });
        }

        [HttpGet("/users/edit")]
        public IActionResult UserProfile()
        {
            if (HttpContext.Session.GetInt32("UserId") == null) 
            {
                return RedirectToAction("Index", "Home");
            } 
            else 
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                User SelectedUser = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
                ViewBag.User = SelectedUser;
                return View("Profile", SelectedUser);
            }
        }

        [HttpPost("/users/edit/")]
        public IActionResult UpdateProfile(User newUser)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            if (ModelState.IsValid)
            {
                user.FirstName = newUser.FirstName;
                user.LastName = newUser.LastName;
                user.Email = newUser.Email;
                user.Password = newUser.Password;
                user.Description = newUser.Description;
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { id = user.UserId });
            }
            return View("Profile", user);
        }

        [HttpPost("/img")]
        public IActionResult AddAvarta(string AvartaUrl)
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            User user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            if (ModelState.IsValid)
            {
                user.AvartaUrl = AvartaUrl;
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { id = user.UserId });
            }
            return View("Profile", user);
        }

        // [HttpPost("/img")]
        // public IActionResult UploadImg(IFormFile file)
        // {
        //     if (file == null || file.Length == 0) return Content("file not selected");
            // get Path
            // string pathRoot = _appEnvironment.WebRootPath; (not working?)
            // string pathToImg = $"{pathRoot}\User_Files\Images\{file.FielName}";
            // copy file to target
            // using (var stream = new FileStreamResult(pathToImg, FileMode.Create))
            // {
            //     await file.CopyToAsync(stream);
            // }
            // // output
            // ViewData["FilePath"] = pathToImg;
            // return View("");
        // }
    }
}