using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoBackend.Models
{
	
	public class TasksModel
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string TaskName { get; set; }=String.Empty;

		[JsonIgnore]
		public bool Status { get; set; } = false;

		public DateTime CreatedDate { get; set; }= DateTime.Now;

		public DateTime CompletedDate {  get; set; }= DateTime.Now;
		public int UserId { get; set; }



		[JsonIgnore]
		//Foreign Key
		[ForeignKey("UserId")]
		public virtual UserModel? User { get; set; }
	}
}