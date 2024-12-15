using System.ComponentModel.DataAnnotations;

namespace API_BackEnd.Model
{
    public class SingInModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PassWord { get; set; } = null!;
    }
}
