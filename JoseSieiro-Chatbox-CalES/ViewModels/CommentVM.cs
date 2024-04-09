using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using JoseSieiro_Chatbox_CalES.Models;

namespace JoseSieiro_Chatbox_CalES.ViewModels
{
	public class CommentVM
	{
		public IEnumerable<Comment> Comments { get; set; }
		public string FullName { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }

	}
}
