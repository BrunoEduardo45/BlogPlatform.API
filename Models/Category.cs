using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Blog.Models
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        
        public IList<Post> Posts { get; set; }
    }
}