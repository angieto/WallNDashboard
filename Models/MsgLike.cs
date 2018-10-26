using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace TheWall.Models
{
    public class MsgLike
    {
        public int MsgLikeId {get; set;}

        // navigation properties
        public int UserId {get; set;}
        public User User {get; set;}

        public int MessageId {get; set;}
        public Message Message {get; set;}

        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}