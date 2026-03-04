using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Roles;

public class EditorRoleViewModel
{
    [Required(ErrorMessage = "O nome da role é obrigatório")]
    public string Name { get; set; }

    [Required(ErrorMessage = "O slug da role é obrigatório")]
    public string Slug { get; set; }
}