using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheWall.Models
{
    public class User 
    {
        // [Key]
        public int UserId {get; set;}

        [Required(ErrorMessage = "User first name is required")]
        [MinLength(2, ErrorMessage = "First name should be 2 characters or longer")]
        [MaxLength(15, ErrorMessage = "First name too long!")]
        public string FirstName {get; set;}

        [Required(ErrorMessage = "User last name is required")]
        [MinLength(2, ErrorMessage = "Last name should be 2 characters or longer")]
        [MaxLength(15, ErrorMessage = "Last name too long!")]
        public string LastName {get; set;}

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email {get; set;}

        [Required(ErrorMessage = "Missing password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters or longer")]
        [DataType(DataType.Password)]
        public string Password {get; set;}

        public string Description {get; set;}

        public int Level {get; set;} // 1: normal vs. 9: admin

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        // Won't be mapped to user table
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get; set;}

        public List<Comment> Comments {get; set;}
        

        [InverseProperty("User")]
        public List<Message> CreatedMessages {get; set;}

        [InverseProperty("Recipient")]
        public List<Message> ReceivedMessages {get; set;}

    }
}