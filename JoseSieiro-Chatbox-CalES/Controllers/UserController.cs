using JoseSieiro_Chatbox_CalES.Models;
using JoseSieiro_Chatbox_CalES.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace JoseSieiro_Chatbox_CalES.Controllers
{
    public class UserController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public UserController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult UserProfile()
        {

            // Obtain the ID of the user of the HTTP session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                //if the user is not present in the session, redirect him to the login view
                return RedirectToAction("Login", "Account");
            }

            // search user in the database
            User user = _context.Users
                .Find(userId);


            return View(user);
        }

        //Post: ChatRoom/PostComment
        [HttpPost]
        public async Task<IActionResult> UpdatePicture(PictureVM picturevm)
        {

            // Obtain the ID of the user of the HTTP session
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                //if the user is not present in the session, redirect him to the login view
                return RedirectToAction("Login", "Account");
            }

            var file = picturevm.Picture;

            // search user in the database
            User user = _context.Users
                .Find(userId);

            if (file != null) {
                var extension = Path.GetExtension(file.FileName);
                string idAndExtension = userId + extension;
                string imageUrl = "/images/" + idAndExtension;
                user.ImageUrl = imageUrl;

                _context.Update(user);
                await _context.SaveChangesAsync();

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "images");

                // create the directory if it doesnt exist
                //if (!Directory.Exists(path))
                //{
                //    Directory.CreateDirectory(path);
                //}

                // create the complete route of the path
                string filePath = Path.Combine(path, idAndExtension);
            
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                //file.SaveAs((path + idAndExtension));

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }



            return RedirectToAction("UserProfile");
        }
    }
}
