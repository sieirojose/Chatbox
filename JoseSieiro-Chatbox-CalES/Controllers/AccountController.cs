using Microsoft.AspNetCore.Mvc;
using JoseSieiro_Chatbox_CalES.ViewModels;
using JoseSieiro_Chatbox_CalES.Models;
using Konscious.Security.Cryptography;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

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


            //getting the extension, combining with the userid and generating the filepath
            //then insert the path to the database
            var extension = Path.GetExtension(file.FileName);
            string idAndExtension = "1" + userId + extension;
            string imageUrl = "/images/" + idAndExtension;
            user.ImageUrl = imageUrl;

            _context.Update(user);
            await _context.SaveChangesAsync();


            //getting the path of the folder images in wwwroot
            var path = Path.Combine(_hostingEnvironment.WebRootPath, "images");

            // create the complete route of the path
            string filePath = Path.Combine(path, idAndExtension);
            
            //if user had already an image, delete the one he had
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //file.SaveAs((path + idAndExtension));

            // Save the image
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            


            return RedirectToAction("Login", "Account");
        }

		private string HashPassword(string password)
		{
			// Generate the salt
			byte[] salt = new byte[16];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(salt);
			}

			// Configure Argon2 object
			var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
			{
				Salt = salt, // Sal generated
				DegreeOfParallelism = 8, // Number of threads to paralelize
				MemorySize = 8192, // size of the memory in KiB
				Iterations = 4 // Number of iterations
			};

			// Calculate the hash
			var hash = argon2.GetBytes(32); // Length of the hash in bytes

			// Concatenate the hash and the salt
			byte[] hashWithSalt = new byte[salt.Length + hash.Length];
			Array.Copy(salt, 0, hashWithSalt, 0, salt.Length);
			Array.Copy(hash, 0, hashWithSalt, salt.Length, hash.Length);

			// Convert to Base64 to store in the database
			return Convert.ToBase64String(hashWithSalt);
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
			// Convertir la cadena Base64 del hash almacenado en un arreglo de bytes
			byte[] hashWithSalt = Convert.FromBase64String(storedPasswordHash);

			// Obtener la sal y el hash almacenados del hash combinado
			byte[] salt = new byte[16]; // La sal tiene una longitud de 16 bytes
			byte[] storedHash = new byte[hashWithSalt.Length - salt.Length];
			Array.Copy(hashWithSalt, 0, salt, 0, salt.Length);
			Array.Copy(hashWithSalt, salt.Length, storedHash, 0, storedHash.Length);

			// Configurar el objeto Argon2 con la misma sal y parámetros utilizados para hashear la contraseña original
			var argon2 = new Argon2id(Encoding.UTF8.GetBytes(enteredPassword))
			{
				Salt = salt,
				DegreeOfParallelism = 8,
				MemorySize = 8192,
				Iterations = 4
			};

			// Calcular el hash del password entrado
			var enteredPasswordHash = argon2.GetBytes(32); // Longitud del hash en bytes

			// Comparar el hash calculado del password entrado con el hash almacenado
			return enteredPasswordHash.SequenceEqual(storedHash);
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
