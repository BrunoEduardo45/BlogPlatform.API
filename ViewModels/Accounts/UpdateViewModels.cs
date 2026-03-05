using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class UpdateViewModels
    {
        [Required]
        [MaxLength(80)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Bio { get; set; }
    }
}
