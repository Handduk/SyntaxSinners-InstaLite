using System.ComponentModel.DataAnnotations;

namespace Api.Dtos
{
    public class UpdateUserModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
