using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MicroSocialPlatform.Models
{
    public class Member
    {
        [Key, Column(Order = 0)]
        public int GroupId { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}