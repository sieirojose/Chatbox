using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JoseSieiro_Chatbox_CalES.ViewModels
{
    public class RegisterVM
    {

        [Required, MinLength(5)]
        public string Username { get; set; }

		[Required, MinLength(5)]
		public string FullName { get; set; }

		[Required, EmailAddress]
		public string Email { get; set; }

		[Required, MinLength(5), DataType(DataType.Password)]
        public string Password { get; set; }


        [Compare("Password"), DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public IFormFile ImageUrl { get; set; }

    }
}
