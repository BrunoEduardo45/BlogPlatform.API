using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Posts
{
    public class EditorPostViewModel
    {
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(120, MinimumLength = 5)]
        public string Title { get; set; }

        [Required(ErrorMessage = "O resumo é obrigatório")]
        [StringLength(255)]
        public string Summary { get; set; }

        [Required(ErrorMessage = "O corpo do post é obrigatório")]
        public string Body { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoryId { get; set; }
    }
}
