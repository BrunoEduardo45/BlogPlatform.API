using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Tags
{
    public class EditorTagViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O slug é obrigatório")]
        [StringLength(40, MinimumLength = 2)]
        public string Slug { get; set; }
    }
}