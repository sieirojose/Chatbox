using Microsoft.AspNetCore.Mvc;
using JoseSieiro_Chatbox_CalES.ViewModels;
using JoseSieiro_Chatbox_CalES.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Diagnostics;

namespace JoseSieiro_Chatbox_CalES.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Constructor to initialize ApplicationDbContext
        public AccountController(IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        //Get: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        //Post: Account/Register
        [HttpPost]
        public async  Task<IActionResult> Register(RegisterVM registervm)
        {
            //Comparing if the Username of the User that is going to be created has been previously registered
            bool userExists = _context.Users.Any(user => user.Username == registervm.Username);
            if (userExists) {
                ViewBag.UsernameMessage = "This username is already in use, try another";
                return View();
            }
            //Comparing if the Email of the User that is going to be created has been previously registered
            bool emailExists = _context.Users.Any(user => user.Email == registervm.Email);
            if (emailExists)
            {
                ViewBag.EmailMessage = "This email is already in use, try another";
                return View();
            }

			// Generate password hash
			string hashedPassword = HashPassword(registervm.Password);




            //if the username and email are unique, Add the user to the database
            var user = new User
            {
                FullName = registervm.FullName,
                Username = registervm.Username,
                Password = hashedPassword,
                Email = registervm.Email,
                ImageUrl = "",
                CreatedOn = DateTime.Now,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Now user.Id will have the ID of the newly created user
            int userId = user.Id;
            var file = registervm.ImageUrl;


            Debug.WriteLine("userId: " + userId);
            Debug.WriteLine("file: " + file);


            var extension = Path.GetExtension(file.FileName);
            string idAndExtension = "1" + userId + extension;
            string imageUrl = "/images/" + idAndExtension;
            user.ImageUrl = imageUrl;

            _context.Update(user);
            await _context.SaveChangesAsync();



            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images");

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
            


            return RedirectToAction("Login", "Account");
        }

		private string HashPassword(string password)
		{
			// Generate a 128-bit salt using a secure PRNG
			byte[] salt = new byte[128 / 8];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}

			// Derive a 256-bit subkey (use HMACSHA1 with 10000 iterations)
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 10000,
				numBytesRequested: 256 / 8));

			// Concatenate the salt and the hash
			return $"{Convert.ToBase64String(salt)}:{hashed}";
		}

		//Get: Account/Login
		[HttpGet]
        public IActionResult Login()
        {
            return View();
        }

		//Post: Account/Login
		[HttpPost]
		public async Task<IActionResult> Login(LoginVM loginvm)
		{
			// Find user with the given username
			User user = _context.Users.FirstOrDefault(user => user.Email == loginvm.Email);

			// Check if user exists and verify password
			if (user != null && VerifyPassword(loginvm.Password, user.Password))
			{
				// store the user info in the http session
				HttpContext.Session.SetInt32("UserId", user.Id);
				HttpContext.Session.SetString("UserImageUrl", user.ImageUrl);

				return RedirectToAction("Index", "ChatRoom");
			}

			//if the credentials are invalid
			ViewBag.Message = "Invalid Credentials";
			return View();
		}

		private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
		{
			// Split the stored password hash into its components (salt and hash)
			string[] hashParts = storedPasswordHash.Split(':');
			byte[] salt = Convert.FromBase64String(hashParts[0]);
			string storedHash = hashParts[1];

			// Compute the hash of the entered password using the same salt and compare it with the stored hash
			string hashedEnteredPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: enteredPassword,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 10000,
				numBytesRequested: 256 / 8));

			return storedHash == hashedEnteredPassword;
		}

		//Get: Account/Logout
		[HttpGet]
        public IActionResult Logout()
        {

            // Clean the session removing "UserId"
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login", "Account");
        }


    }
}
