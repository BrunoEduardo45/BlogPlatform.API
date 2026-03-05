using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContextSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrador", Slug = "admin" },
                new Role { Id = 2, Name = "Autor", Slug = "author" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@blog.com",
                    Slug = "admin",
                    PasswordHash = "HASH_FIXO_AQUI",
                    Bio = "Administrador do sistema",
                    Image = null
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Backend", Slug = "backend" },
                new Category { Id = 2, Name = "Frontend", Slug = "frontend" },
                new Category { Id = 3, Name = "DevOps", Slug = "devops" }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "ASP.NET", Slug = "aspnet" },
                new Tag { Id = 2, Name = "CSharp", Slug = "csharp" }
            );

            modelBuilder.Entity<Post>().HasData(
                new Post
                {
                    Id = 1,
                    Title = "Primeiro Post do Sistema",
                    Slug = "primeiro-post",
                    Summary = "Post inicial de exemplo",
                    Body = "Conteúdo completo do primeiro post.",
                    CategoryId = 1,
                    AuthorId = 1,
                    CreateDate = new DateTime(2024, 1, 1),
                    LastUpdateDate = new DateTime(2024, 1, 1)
                }
            );

            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.HasData(
                    new { UserId = 1, RoleId = 1 }
                ));

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(j => j.HasData(
                    new { PostId = 1, TagId = 1 }
                ));
        }
    }
}
