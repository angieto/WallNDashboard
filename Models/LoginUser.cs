using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheWall.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Missing email address")]
        public string Email {get; set;}

        [Required(ErrorMessage = "Password can't be empty")]
        public string Password {get; set;}
    }
}