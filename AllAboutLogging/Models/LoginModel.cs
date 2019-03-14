using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Destructurama.Attributed;

namespace AllAboutLogging.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
       
        //[NotLogged]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
