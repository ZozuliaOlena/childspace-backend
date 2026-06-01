using System.ComponentModel.DataAnnotations;

namespace childspace_backend.Models.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Поле Email не може бути порожнім.")]
        [EmailAddress(ErrorMessage = "Невірний формат Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Пароль не може бути порожнім.")]
        public string Password { get; set; }
    }
}
