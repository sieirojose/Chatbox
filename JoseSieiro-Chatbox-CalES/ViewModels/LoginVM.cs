using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JoseSieiro_Chatbox_CalES.ViewModels
{
    public class LoginVM
    {
		[Required, EmailAddress]
		public string Email { get; set; }

		[Required,DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
