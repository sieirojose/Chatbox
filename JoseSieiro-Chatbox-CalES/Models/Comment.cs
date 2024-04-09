using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace JoseSieiro_Chatbox_CalES.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? CreatedOn { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

		public string? AttachedDocument { get; set; }

		public ICollection<Reply> Replies { get; set; }


    }
}
