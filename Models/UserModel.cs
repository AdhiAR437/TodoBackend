using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoBackend.Models
{
	public class UserModel
	{
		[Key]
		public int USerId { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }=string.Empty;

		public string Password { get; set; }= string.Empty;

		public string Username { get; set; } = string.Empty;


		
		//References from Foreign Key
		
		public virtual ICollection<TasksModel> Tasks { get; set; }



	}
}
