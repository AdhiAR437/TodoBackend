using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoBackend.Models
{
	
	public class TasksModel
	{
		[Key]
		public int Id { get; set; }

		public string TaskName { get; set; }=String.Empty;

		public bool Status { get; set; }

		public DateTime CreatedDate { get; set; }= DateTime.Now;

		public DateTime CompletedDate {  get; set; }= DateTime.Now;
		public int UserId { get; set; }




		//Foreign Key
		[ForeignKey("UserId")]
		public virtual UserModel Tasks { get; set; }
	}
}
