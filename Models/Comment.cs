using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheWall.Models
{
    public class Comment
    {
        [Key]
        public int Id {get; set;}

        [Required(ErrorMessage = "Comment field can't be blank")]
        public string Cmt {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;

        public int UserId {get; set;}
        public User User {get; set;}

        public int MessageId {get; set;}
        public Message Message {get; set;}

        public string TimeSpan {
            get {
                TimeSpan timeSpan = DateTime.Now.Subtract(this.CreatedAt);
                if (timeSpan.TotalSeconds < 60)
                {
                    return $"{Math.Round(timeSpan.TotalSeconds)} seconds ago";
                }
                else if (timeSpan.TotalMinutes < 60)
                {
                    return $"{Math.Round(timeSpan.TotalMinutes)} minutes ago";
                }
                else if (timeSpan.TotalHours < 24)
                {
                    return $"{Math.Round(timeSpan.TotalHours)} hours ago";
                }
                else if (timeSpan.Days <= 7)
                {
                    return $"{timeSpan.Days} days ago";
                }
                else
                {
                    return null;
                }
            }
        }
    }
}