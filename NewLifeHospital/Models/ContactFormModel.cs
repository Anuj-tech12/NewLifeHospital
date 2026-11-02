using System.ComponentModel.DataAnnotations;

namespace NewLifeHospital.Models
{
    public class ContactFormModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;


    [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        [StringLength(150)]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Please enter your message.")]
        [StringLength(2000, MinimumLength = 5)]
        public string Message { get; set; } = string.Empty;
    }


}
