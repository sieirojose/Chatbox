using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace JoseSieiro_Chatbox_CalES.ViewModels
{
    public class PictureVM
    {
        [Required]
        public IFormFile Picture { get; set; }
    }
}
