using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_BackEnd.Model
{
    public class SignUpModel
    {
        [Required]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [JsonPropertyName("password")]
        public string PassWord { get; set; } = null!;  // Map "password" từ JSON thành PassWord

        [Required]
        [JsonPropertyName("confirmPassword")]
        public string ComfirmPassWord { get; set; } = null!; // Map "confirmPassword" từ JSON thành ComfirmPassWord
    }
}
