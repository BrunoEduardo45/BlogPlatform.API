using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Common;
using Blog.ViewModels.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
public class RoleController : ControllerBase
{
    #region Queries

    [HttpGet("v1/roles")]
    public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
    {
        try
        {
            var roles = await context
                .Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToListAsync();

            return Ok(new ResultViewModel<List<Role>>(roles));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<List<Role>>("05X50 - Falha ao listar roles"));
        }
    }

    [HttpGet("v1/roles/{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
    {
        try
        {
            var role = await context
                .Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role == null)
                return NotFound(new ResultViewModel<Role>("Role não encontrada"));

            return Ok(new ResultViewModel<Role>(role));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Role>("05X51 - Falha ao buscar role"));
        }
    }

    #endregion

    #region Commands

    [HttpPost("v1/roles")]
    public async Task<IActionResult> PostAsync([FromBody] EditorRoleViewModel model, [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Role>(ModelState.GetErrors()));

        var role = new Role
        {
            Name = model.Name,
            Slug = model.Slug.ToLower()
        };

        try
        {
            await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();

            return Created($"v1/roles/{role.Id}",
                new ResultViewModel<Role>(role));
        }
        catch (DbUpdateException)
        {
            return StatusCode(500,
                new ResultViewModel<Role>("Essa role já existe"));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Role>("05X52 - Falha ao criar role"));
        }
    }

    [HttpPut("v1/roles/{id:int}")]
    public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorRoleViewModel model, [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<Role>(ModelState.GetErrors()));

        var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);

        if (role == null)
            return NotFound(new ResultViewModel<Role>("Role não encontrada"));

        role.Name = model.Name;
        role.Slug = model.Slug.ToLower();

        try
        {
            context.Roles.Update(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Role>(role));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Role>("05X53 - Falha ao atualizar role"));
        }
    }

    [HttpDelete("v1/roles/{id:int}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context)
    {
        var role = await context.Roles.FirstOrDefaultAsync(x => x.Id == id);

        if (role == null)
            return NotFound(new ResultViewModel<Role>("Role não encontrada"));

        if (role.Slug == "admin")
            return BadRequest(new ResultViewModel<Role>("Não é possível remover a role admin"));

        try
        {
            context.Roles.Remove(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Role>(role));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<Role>("05X54 - Falha ao remover role"));
        }
    }

    #endregion

    #region Role in Users

    [HttpPost("v1/users/{userId:int}/roles/{roleId:int}")]
    public async Task<IActionResult> AddRoleToUserAsync([FromRoute] int userId, [FromRoute] int roleId, [FromServices] BlogDataContext context)
    {
        var user = await context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

        var role = await context.Roles
            .FirstOrDefaultAsync(x => x.Id == roleId);

        if (role == null)
            return NotFound(new ResultViewModel<User>("Role não encontrada"));

        if (user.Roles.Any(x => x.Id == roleId))
            return BadRequest(new ResultViewModel<User>("Usuário já possui essa role"));

        try
        {
            user.Roles.Add(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<User>("05X55 - Falha ao vincular role ao usuário"));
        }
    }

    [HttpDelete("v1/users/{userId:int}/roles/{roleId:int}")]
    public async Task<IActionResult> RemoveRoleFromUserAsync([FromRoute] int userId, [FromRoute] int roleId, [FromServices] BlogDataContext context)
    {
        var user = await context.Users
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

        var role = user.Roles.FirstOrDefault(x => x.Id == roleId);

        if (role == null)
            return NotFound(new ResultViewModel<User>("Role não vinculada ao usuário"));

        try
        {
            user.Roles.Remove(role);
            await context.SaveChangesAsync();

            return Ok(new ResultViewModel<User>(user));
        }
        catch
        {
            return StatusCode(500,
                new ResultViewModel<User>("05X56 - Falha ao remover role do usuário"));
        }
    }

    #endregion
}