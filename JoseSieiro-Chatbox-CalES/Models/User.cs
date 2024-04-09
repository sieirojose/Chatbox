using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JoseSieiro_Chatbox_CalES.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MinLength(5)]
        [Column(TypeName = "VARCHAR")]
        public string Username { get; set; }

		[Required, MinLength(5)]
		[Column(TypeName = "VARCHAR")]
		[Display(Name = "Full Name")]
		public string FullName { get; set; }

		[Required, MinLength(5), DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, EmailAddress]
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Picture")]
        public string ImageUrl { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Account Created")]
        public DateTime? CreatedOn { get; set; }
    }
}
