using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Common;
using Blog.ViewModels.Tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class TagController : ControllerBase
{
    #region Queries

    [HttpGet("v1/tags")]
    public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
    {
        try
        {
            var tags = await context
                .Tags
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return Ok(new ResultViewModel<List<Tag>>(tags));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<List<Tag>>("05X30 - Falha ao listar tags"));
        }
    }

    [HttpGet("v1/tags/{slug}")]
    public async Task<IActionResult> GetBySlugAsync([FromRoute] string slug, [FromServices] BlogDataContext context)
        {
            try
            {
                var tag = await context
                    .Tags
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Slug == slug);

                if (tag == null)
                    return NotFound(new ResultViewModel<Tag>("Tag não encontrada"));

                return Ok(new ResultViewModel<Tag>(tag));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Tag>("05X31 - Falha ao buscar tag"));
            }
        }

    #endregion

    #region Commands

    [Authorize(Roles = "admin")]
    [HttpPost("v1/tags")]
    public async Task<IActionResult> PostAsync([FromBody] EditorTagViewModel model, [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Tag>(ModelState.GetErrors()));

        var tag = new Tag
        {
            Name = model.Name,
            Slug = model.Slug.ToLower()
        };

        try
        {
            await context.Tags.AddAsync(tag);
            await context.SaveChangesAsync();

            return Created($"v1/tags/{tag.Id}",
                new ResultViewModel<Tag>(tag));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500,
                new ResultViewModel<Tag>("Essa tag já existe"));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Tag>("05X32 - Falha ao criar tag"));
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPut("v1/tags/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorTagViewModel model, [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Tag>(ModelState.GetErrors()));

        var tag = await context.Tags.FirstOrDefaultAsync(x => x.Id == id);

        if (tag == null)
            return NotFound(new ResultViewModel<Tag>("Tag não encontrada"));

        tag.Name = model.Name;
        tag.Slug = model.Slug.ToLower();

        try
        {
            context.Tags.Update(tag);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Tag>(tag));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Tag>("05X33 - Falha ao atualizar tag"));
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("v1/tags/{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag == null)
                return NotFound(new ResultViewModel<Tag>("Tag não encontrada"));

            var hasPosts = await context.Posts
                .AnyAsync(x => x.Tags.Any(t => t.Id == id));

            if (hasPosts)
                return BadRequest(
                    new ResultViewModel<Tag>("Tag vinculada a posts"));

            try
            {
                context.Tags.Remove(tag);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Tag>(tag));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Tag>("05X34 - Falha ao remover tag"));
            }
        }

    #endregion

    #region Tag in Posts

    [Authorize(Roles = "author,admin")]
    [HttpPost("v1/posts/{postId:int}/tags/{tagId:int}")]
    public async Task<IActionResult> AddTagAsync([FromRoute] int postId, [FromRoute] int tagId, [FromServices] BlogDataContext context)
    {
        var post = await context.Posts
            .Include(x => x.Tags)
            .Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == postId);

        if (post == null)
            return NotFound(new ResultViewModel<Post>("Post não encontrado"));

        if (!User.IsInRole("admin") && post.Author.Email != User.Identity.Name)
            return Forbid();

        var tag = await context.Tags
            .FirstOrDefaultAsync(x => x.Id == tagId);

        if (tag == null)
            return NotFound(new ResultViewModel<Post>("Tag não encontrada"));

        if (post.Tags.Any(x => x.Id == tagId))
            return BadRequest(new ResultViewModel<Post>("Tag já vinculada ao post"));

        try
        {
            post.Tags.Add(tag);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Post>(post));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Post>("05X40 - Falha ao adicionar tag"));
        }
    }

    [Authorize(Roles = "author,admin")]
    [HttpDelete("v1/posts/{postId:int}/tags/{tagId:int}")]
    public async Task<IActionResult> RemoveTagAsync([FromRoute] int postId, [FromRoute] int tagId, [FromServices] BlogDataContext context)
        {
            var post = await context.Posts
                .Include(x => x.Tags)
                .Include(x => x.Author)
                .FirstOrDefaultAsync(x => x.Id == postId);

            if (post == null)
                return NotFound(new ResultViewModel<Post>("Post não encontrado"));

            if (!User.IsInRole("admin") && post.Author.Email != User.Identity.Name)
                return Forbid();

            var tag = post.Tags.FirstOrDefault(x => x.Id == tagId);

            if (tag == null)
                return NotFound(new ResultViewModel<Post>("Tag não vinculada ao post"));

            try
            {
                post.Tags.Remove(tag);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Post>(post));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<Post>("05X41 - Falha ao remover tag"));
            }
        }

    #endregion
}