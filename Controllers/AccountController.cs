using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Login

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, [FromServices] BlogDataContext context, [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context
                .Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        #endregion

        #region Queries

        [Authorize(Roles = "admin")]
        [HttpGet("v1/accounts")]
        public async Task<IActionResult> GetAll([FromServices] BlogDataContext context)
        {
            try
            {
                var users = await context
                    .Users
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                return Ok(new ResultViewModel<List<User>>(users));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<List<User>>("05X60 - Falha ao listar usuários"));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("v1/accounts/{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            try
            {
                var user = await context
                    .Users
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

                return Ok(new ResultViewModel<User>(user));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<User>("05X61 - Falha ao buscar usuário"));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("v1/accounts/role/{role}")]
        public async Task<IActionResult> GetByRole([FromRoute] string role, [FromServices] BlogDataContext context)
        {
            try
            {
                var users = await context.Users
                    .AsNoTracking()
                    .Include(x => x.Roles)
                    .Where(x => x.Roles.Any(r => r.Slug == role))
                    .ToListAsync();

                return Ok(new ResultViewModel<List<User>>(users));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<List<User>>("05X62 - Falha ao buscar usuários por role"));
            }
        }

        #endregion

        #region Commands

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModels model, [FromServices] BlogDataContext context, [FromServices] EmailService emailService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                emailService.Send
                (
                    user.Name, user.Email,
                    "Bem vindo ao blog!",
                    $"Sua senha é {password}"
                );

                return Ok(new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400,
                    new ResultViewModel<string>("05X99 - Este E-mail já está cadastrado"));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<string>("05X04 - Falha interna no servidor"));
            }
        }

        [Authorize]
        [HttpPut("v1/accounts/{id:int}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] RegisterViewModels model, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<User>(ModelState.GetErrors()));

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            if (!User.IsInRole("admin") && user.Email != User.Identity.Name)
                return Forbid();

            user.Name = model.Name;
            user.Email = model.Email;

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<User>(user));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<User>("05X63 - Falha ao atualizar usuário"));
            }
        }

        [Authorize]
        [HttpDelete("v1/accounts/{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, [FromServices] BlogDataContext context)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            if (!User.IsInRole("admin") && user.Email != User.Identity.Name)
                return Forbid();

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<User>(user));
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<User>("05X64 - Falha ao remover usuário"));
            }
        }

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model, [FromServices] BlogDataContext context)
        {
            if (string.IsNullOrEmpty(model.Base64Image))
                return BadRequest(new ResultViewModel<string>("Imagem inválida"));

            if (!model.Base64Image.StartsWith("data:image/"))
                return BadRequest(new ResultViewModel<string>("Formato de imagem inválido"));

            var fileName = $"{Guid.NewGuid()}.jpg";
            var data = new Regex(@"^data:image\/[a-z]+;base64,")
                .Replace(model.Base64Image, "");
            var bytes = Convert.FromBase64String(data);

            if (bytes.Length > 5 * 1024 * 1024)
                return BadRequest(new ResultViewModel<string>("Imagem muito grande (máx 5MB)"));

            try
            {
                await System.IO.File.WriteAllBytesAsync(
                    $"wwwroot/images/{fileName}", bytes);
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<string>("05X65 - Falha ao salvar imagem"));
            }

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

            if (user == null)
                return NotFound(new ResultViewModel<User>("Usuário não encontrado"));

            user.Image = $"https://localhost:0000/images/{fileName}";

            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }
            catch
            {
                return StatusCode(500,
                    new ResultViewModel<string>("05X66 - Falha ao atualizar imagem"));
            }

            return Ok(new ResultViewModel<string>("Imagem alterada com sucesso!"));
        }

        #endregion
    }
}