using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public string ImageComment { get; set; }
        public DateTime CreatedAt { get; set; }


        public int PostId { get; set; }
        public Post Post { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }
        public string Username { get; set; }
    }
}
