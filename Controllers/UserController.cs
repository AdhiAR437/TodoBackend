//using Microsoft.AspNetCore.Mvc;
//using TodoBackend.Data;
//using TodoBackend.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.Data;

//namespace TodoBackend.Controllers
//{
//	[ApiController]
//	[Route("api/[controller]")]
//	public class UserController : Controller
//	{
//		private readonly ApplicationDbContext _context;

//		public UserController(ApplicationDbContext context)
//		{
//			_context = context;
//		}






//		[HttpPost("Signup")]
//		public async Task<IActionResult> PostUser(UserModel user)
//		{
//			if (user == null) { return BadRequest(); }
//			/*if (ModelState.IsValid)*/
//			{
//				await _context.Users_tb.AddAsync(user);
//				await _context.SaveChangesAsync();
//				return Ok("posted");
//			}
//			return BadRequest();
//		}


//		[HttpPost("Login")]
//		public IActionResult Login([FromBody] LoginRequest request)
//		{
//			if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
//			{
//				return BadRequest("Email and password are required.");
//			}

//			var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email && u.Password==request.Password);
//			if (user == null)
//			{
//				return Unauthorized("Invalid email or password.");
//			}
//			return Ok(request.Email); // Return a token or user details as needed
//		}
//	}
//}



using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using TodoBackend.Data;
using TodoBackend.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;

namespace TodoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        // Dictionary to store tokens and expiry times in memory    
        private static readonly ConcurrentDictionary<string, (string Token, DateTime Expiry)> ResetTokens = new ConcurrentDictionary<string, (string Token, DateTime Expiry)>();


        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> PostUser(UserModel user)
        {
            if (user == null) { return BadRequest(); }

            await _context.Users_tb.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok("User successfully registered.");
        }

     


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Create the LoginResponse object with necessary data
            var loginResponse = new LoginResponse
            {
                UserId = user.UserId,
                Email = user.Email
            };

            return Ok(loginResponse); // Return the response as JSON
        }




        //[HttpPost("ForgotPassword")]
        //public async Task<IActionResult> ForgotPassword([FromBody] TodoBackend.Models.ForgotPasswordRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Email))
        //    {
        //        return BadRequest("Email is required.");
        //    }



        //    var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email);
        //    if (user == null)
        //    {
        //        return NotFound("User with this email does not exist.");
        //    }

        //    // Generate a reset token and store it in memory with a 1-hour expiry
        //    string resetToken = GenerateToken();
        //    ResetTokens[request.Email] = (resetToken, DateTime.Now.AddHours(1));

        //    // Send the reset link to the user's email
        //    //var resetLink = $"{Request.Scheme}://{Request.Host}/api/user/ResetPassword?token={resetToken}&email={request.Email}";
        //    var frontendUrl = _configuration["FrontendUrl"];
        //    var resetLink = $"{frontendUrl}/User/ResetPassword?token={resetToken}&email={request.Email}";



        //    await SendPasswordResetEmail(user.Email, resetLink);

        //    return Ok("Password reset link has been sent to your email.");
        //}

        //[HttpPost("ResetPassword")]
        //public IActionResult ResetPassword([FromBody] TodoBackend.Models.ResetPasswordRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
        //    {
        //        return BadRequest("Email, token, and new password are required.");
        //    }

        //    // Verify the reset token
        //    if (!ResetTokens.TryGetValue(request.Email, out var tokenData) || tokenData.Token != request.Token || tokenData.Expiry < DateTime.Now)
        //    {
        //        return BadRequest("Invalid or expired token.");
        //    }

        //    // Retrieve the user and update the password
        //    var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email);
        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    user.Password = request.NewPassword;  // Apply hashing if needed
        //    _context.SaveChanges();

        //    // Remove the token after use
        //    ResetTokens.TryRemove(request.Email, out _);

        //    return Ok("Password has been reset successfully.");
        //}



        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] TodoBackend.Models.ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("Email is required.");
            }

            var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("User with this email does not exist.");
            }

            // Generate a reset token and store it in memory with a 1-hour expiry
            string resetToken = GenerateToken();
            ResetTokens[request.Email] = (resetToken, DateTime.Now.AddHours(1));

            var frontendUrl = _configuration["FrontendUrl"];
            //var resetLink = $"{frontendUrl}/User/ResetPassword?token={resetToken}&email={request.Email}";
            var resetLink = $"{frontendUrl}/User/ResetPassword?token={Uri.EscapeDataString(resetToken)}&email={request.Email}";




            await SendPasswordResetEmail(user.Email, resetLink);

            return Ok("Password reset link has been sent to your email.");
        }


        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword([FromBody] TodoBackend.Models.ResetPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Email, token, and new password are required.");
            }

            // Check if the token is in the dictionary and validate it
            if (!ResetTokens.TryGetValue(request.Email, out var tokenData) || tokenData.Token != request.Token || tokenData.Expiry < DateTime.Now)
            {
                return BadRequest("Invalid or expired token.");
            }

            // Retrieve the user and update the password
            var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Password = request.NewPassword;  // Remember to hash this password
            _context.SaveChanges();

            // Remove the token from the dictionary after use
            ResetTokens.TryRemove(request.Email, out _);

            return Ok("Password has been reset successfully.");
        }



        private string GenerateToken()
        {
            using var generator = new RNGCryptoServiceProvider();
            byte[] tokenBytes = new byte[32];
            generator.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }
        
    private async Task SendPasswordResetEmail(string toEmail, string resetLink)
        {
            using (var smtpClient = new SmtpClient("smtp.gmail.com"))  // Corrected SMTP server for Gmail
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("myecomerceinfo@gmail.com", "ohkn uejs ewnq xykp"); // Use app password
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("myecomerceinfo@gmail.com"),
                    Subject = "Password Reset Request",
                    Body = $"Please reset your password by clicking on the following link: {resetLink}",
                    IsBodyHtml = false
                };
                mailMessage.To.Add(toEmail);

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (SmtpException smtpEx)
                {
                    // Log the detailed SMTP error message here
                    throw new Exception("Error sending email", smtpEx);
                }
            }
        }





        //[HttpGet("FindByEmail")]
        //public async Task<IActionResult> FindByEmail(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest("Email is required.");
        //    }

        //    var user = await FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return NotFound("User not found.");
        //    }

        //    // Return user details as needed. Avoid returning sensitive information.
        //    return Ok(new { user.Email, user.UserName, user.Id });
        //}
    }
}

