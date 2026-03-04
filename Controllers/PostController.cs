using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Common;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        #region Queries

        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context, [FromQuery] PaginationQuery query)
        {
            query.Normalize();

            try
            {
                var count = await context.Posts.AsNoTracking().CountAsync();
                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .Include(x => x.Author)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(x => x.LastUpdateDate)
                    .Skip((query.Page -1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    query.Page,
                    query.PageSize,
                    posts
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, 
                    new ResultViewModel<List<Post>>("05XE9 - Falha interna no servidor."));
            }
        }

        [HttpGet("v1/posts/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var post = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado"));

                return Ok(new ResultViewModel<Post>(post));
            }
            catch (Exception ex)
            {
                return StatusCode(500, 
                    new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetByCategoryAsync([FromRoute] string category, [FromQuery] PaginationQuery query, [FromServices] BlogDataContext context)
        {
            query.Normalize();

            try
            {
                var count = await context.Posts
                    .AsNoTracking()
                    .Where(x => x.Category.Slug == category)
                    .CountAsync();

                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x => x.Category.Slug == category)
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(x => x.LastUpdateDate)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();
                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    query.Page,
                    query.PageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500, 
                    new ResultViewModel<List<Category>>("05X04 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/posts/tag/{tag}")]
        public async Task<IActionResult> GetByTagAsync([FromRoute] string tag, [FromQuery] PaginationQuery query, [FromServices] BlogDataContext context)
        {
            query.Normalize();

            try
            {
                var count = await context.Posts
                    .AsNoTracking()
                    .Where(x => x.Tags.Any(t => t.Slug == tag))
                    .CountAsync();

                var posts = await context
                    .Posts
                    .AsNoTracking()
                    .Include(x => x.Author)
                    .Include(x => x.Category)
                    .Where(x => x.Tags.Any(t => t.Slug == tag))
                    .Select(x => new ListPostsViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        LastUpdateDate = x.LastUpdateDate,
                        Category = x.Category.Name,
                        Author = $"{x.Author.Name} ({x.Author.Email})"
                    })
                    .OrderByDescending(x => x.LastUpdateDate)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    query.Page,
                    query.PageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<List<Tag>>("05X05 - Falha interna no servidor"));
            }
        }

        #endregion

        #region Commands

        [Authorize(Roles = "author,admin")]
        [HttpPost("v1/posts")]
        public async Task<IActionResult> PostAsync([FromBody] EditorPostViewModel model, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Post>(ModelState.GetErrors()));

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null)
                return Unauthorized(new ResultViewModel<Post>("Usuário inválido"));

            var category = await context.Categories
                .FirstOrDefaultAsync(x => x.Id == model.CategoryId);

            if (category == null)
                return BadRequest(new ResultViewModel<Post>("Categoria inválida"));

            var post = new Post
            {
                Title = model.Title,
                Summary = model.Summary,
                Body = model.Body,
                Slug = model.Title.ToLower().Replace(" ", "-"),
                CreateDate = DateTime.UtcNow,
                LastUpdateDate = DateTime.UtcNow,
                Author = user,
                Category = category
            };

            try
            {
                await context.Posts.AddAsync(post);
                await context.SaveChangesAsync();

                return Created($"v1/posts/{post.Id}",
                    new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Post>("05X20 - Falha ao criar post"));
            }
        }

        [Authorize(Roles = "author,admin")]
        [HttpPut("v1/posts/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorPostViewModel model, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Post>(ModelState.GetErrors()));

            var post = await context.Posts
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
                return NotFound(new ResultViewModel<Post>("Post não encontrado"));

            if (!User.IsInRole("admin") && post.Author.Email != User.Identity.Name)
                return Forbid();

            var category = await context.Categories
                .FirstOrDefaultAsync(x => x.Id == model.CategoryId);

            if (category == null)
                return BadRequest(new ResultViewModel<Post>("Categoria inválida"));

            post.Title = model.Title;
            post.Summary = model.Summary;
            post.Body = model.Body;
            post.Category = category;
            post.LastUpdateDate = DateTime.UtcNow;

            try
            {
                context.Posts.Update(post);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Post>("05X21 - Falha ao atualizar post"));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("v1/posts/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            var post = await context.Posts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
                return NotFound(new ResultViewModel<Post>("Post não encontrado"));

            try
            {
                context.Posts.Remove(post);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Post>("05X22 - Falha ao remover post"));
            }
        }

        #endregion
    }
}
