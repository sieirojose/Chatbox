using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JoseSieiro_Chatbox_CalES.Models
{
    public class Reply
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? CreatedOn { get; set; }

        public int UserId { get; set; }

        public int CommentId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("CommentId")]
        public virtual Comment comment { get; set; }

    }
}