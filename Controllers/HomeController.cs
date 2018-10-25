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
    public class HomeController : Controller
    {
        // Context Service
        private WallContext dbContext;
        public HomeController(WallContext context)
        {
            dbContext = context;
        }

        // Routes
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost("/register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("Register");
                }
                // check if the email matches with any email from the database
                
                PasswordHasher<User> Hasher = new PasswordHasher<User>(); 
                user.Password = Hasher.HashPassword(user, user.Password); 
                // The first person that registers will automatically become an Admin
                if (dbContext.Users.Count() == 0) 
                    user.Level = 9; // Admin
                else 
                    user.Level = 1; // Normal
                // Add User
                dbContext.Add(user);
                dbContext.SaveChanges();
                // Add user Id to session
                HttpContext.Session.SetInt32("UserId", user.UserId);
                // redirect to different Dashboard based on user's level
                if(user.Level == 9) 
                    return RedirectToAction("Dashboard", "Admin");
                else 
                    return RedirectToAction("Dashboard", "User");

            }
            return View("Register");
        }

        [HttpGet("/signin")]
        public IActionResult SignIn()
        {
            return View("SignIn");
        }

        [HttpPost("/signin")]
        public IActionResult SignIn(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                Console.WriteLine("Current login user: " + userInDb);
                // if user does not exist in db
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Email does not exist");
                    return View("SignIn");
                }
                else 
                {
                    // initialize hasher obj
                    var hasher = new PasswordHasher<LoginUser>();
                    // varify input pw against hash in db
                    var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                    // if wrong password
                    if (result == 0)
                    {
                        ModelState.AddModelError("Email", "Invalid Password");
                        return View("SignIn");
                    }
                    else 
                    {
                        // else, store current user id in session
                        HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                        // redirect to different Dashboard based on user's level
                        if(userInDb.Level == 9) 
                            return RedirectToAction("Dashboard", "Admin");
                        else 
                            return RedirectToAction("Dashboard", "User");
                    }
                }
            } 
            return View("SignIn");
        }

        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
