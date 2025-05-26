using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Blog.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? AuthorId { get; set; }
        public IdentityUser? Author { get; set; }
    }
}
