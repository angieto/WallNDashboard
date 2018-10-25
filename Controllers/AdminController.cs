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
    public class AdminController : Controller
    {
        // Context Service
        private WallContext dbContext;
        public AdminController(WallContext context)
        {
            dbContext = context;
        }

        // Routes
        [HttpGet("/dashboard/admin")]
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
                List<User> AllUsers = dbContext.Users.Where(user => user.UserId != UserId).OrderBy(user => user.CreatedAt).ToList();
                ViewBag.AllUsers = AllUsers;
                return View("Dashboard");
            }
        }

        [HttpGet("users/new")]
        public IActionResult New()
        {
            return View("New");
        }

        [HttpPost("users/new")]
        public IActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("Dashboard");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>(); 
                user.Password = Hasher.HashPassword(user, user.Password); 
                // Add User
                user.Level = 1; 
                user.Description = "";
                dbContext.Add(user);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("New");
        }

        [HttpGet("admin/users/{id}")]
        public IActionResult UserDetail(int id)
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
                return View("Detail");
            }
        }

        [HttpGet("users/edit/{id}")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null) 
            {
                return RedirectToAction("Index", "Home");
            } 
            else 
            {
                int? UserId = HttpContext.Session.GetInt32("UserId");
                ViewBag.UserId = UserId;
                User SelectedUser = dbContext.Users.FirstOrDefault(user => user.UserId == id);
                ViewBag.SelectedUser = SelectedUser;
                return View("Edit", SelectedUser);
            }
        }

        [HttpPost("users/edit/{id}")]
        public IActionResult Update(User selectedUser, int id)
        {
            User SelectedUser = dbContext.Users.FirstOrDefault(user => user.UserId == id);
            SelectedUser.FirstName = selectedUser.FirstName;
            SelectedUser.LastName = selectedUser.LastName;
            SelectedUser.Email = selectedUser.Email;
            SelectedUser.Level = selectedUser.Level;
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("remove/{id}")]
        public IActionResult Remove(int id)
        {
            User SelectedUser = dbContext.Users.FirstOrDefault(user => user.UserId == id);
            System.Console.WriteLine("Here's your selected user's id: " + id);
            dbContext.Remove(SelectedUser);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost("admin/createMsg/{recipientId}")]
        public IActionResult CreateMsg(Message message, int recipientId)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(message);
                dbContext.SaveChanges();
                return RedirectToAction("UserDetail", new { id = recipientId });
            }
            return View("UserDetail", new { id = recipientId });
        }

        [HttpPost("admin/createCmt/{recipientId}")]
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
    }
}