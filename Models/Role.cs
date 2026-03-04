using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Blog.Models
{
    [Index(nameof(Slug), IsUnique = true)]
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public IList<User> Users { get; set; }
    }
}