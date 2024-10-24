using Microsoft.AspNetCore.Mvc;
using TodoBackend.Data;
using TodoBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace TodoBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _context;
		
		public UserController(ApplicationDbContext context)
		{
			_context = context;
		}






		[HttpPost("Signup")]
		public async Task<IActionResult> PostUser(UserModel user)
		{
			if (user == null) { return BadRequest(); }
			/*if (ModelState.IsValid)*/
			{
				await _context.Users_tb.AddAsync(user);
				await _context.SaveChangesAsync();
				return Ok("posted");
			}
			return BadRequest();
		}


		[HttpPost("Login")]
		public IActionResult Login([FromBody] LoginRequest request)
		{
			if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
			{
				return BadRequest("Email and password are required.");
			}

			var user = _context.Users_tb.SingleOrDefault(u => u.Email == request.Email && u.Password==request.Password);
			if (user == null)
			{
				return Unauthorized("Invalid email or password.");
			}
			return Ok(request.Email); // Return a token or user details as needed
		}
	}
}
