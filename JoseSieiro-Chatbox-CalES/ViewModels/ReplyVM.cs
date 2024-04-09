using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JoseSieiro_Chatbox_CalES.ViewModels
{
    public class ReplyVM
    {
        public string Reply {  get; set; }

        public int CommentId { get; set; }
    }
}
