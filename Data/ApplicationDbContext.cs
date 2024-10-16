using Microsoft.EntityFrameworkCore;
using TodoBackend.Models;

namespace TodoBackend.Data
{
	public class ApplicationDbContext:Microsoft.EntityFrameworkCore.DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<UserModel> Users_tb { get; set; }
		public DbSet<TasksModel> Tasks_tb { get; set; }
	}
}
