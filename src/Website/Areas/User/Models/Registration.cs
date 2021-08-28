using System;
using System.ComponentModel.DataAnnotations;

namespace Headlight.Areas.User.Models
{
    public class Registration
    {
        [Required]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
    }
}