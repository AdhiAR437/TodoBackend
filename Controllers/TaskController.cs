using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using TodoBackend.Data;
using TodoBackend.Models;

namespace TodoBackend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TaskController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public TaskController(ApplicationDbContext context)
		{
			_context = context;
		}

        //// POST: api/Task
        //[HttpPost]
        //public async Task<IActionResult> PostTask(TasksModel task)
        //{
        //	if (task == null)
        //	{
        //		return BadRequest("Task data is null");
        //	}
        //	// Model validation check
        //	if (ModelState.IsValid)
        //	{
        //		// Save the task to the database
        //		await _context.Tasks_tb.AddAsync(task);
        //		await _context.SaveChangesAsync();

        //		// Return the created task and a 201 Created status
        //		return Ok("task added");
        //	}

        //	return BadRequest("Invalid task data");
        //}

        // POST: api/Task
        [HttpPost]
        public async Task<IActionResult> PostTask([FromBody] TasksModel task)
        {
            if (task == null)
            {
                return BadRequest("Task data is null");
            }

            // Model validation check
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid task data");
            }

            // Ensure required fields are set
            if (task.UserId == 0)
            {
                return BadRequest("UserId is required");
            }

            // Set default values if needed
            task.Status = false;  // Default to "incomplete" if not set
            task.CreatedDate = DateTime.Now;

            try
            {
                // Save the task to the database
                await _context.Tasks_tb.AddAsync(task);
                await _context.SaveChangesAsync();

                // Return the created task and a 201 Created status
                return CreatedAtAction(nameof(PostTask), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                // Log the exception and return a server error
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			var task = await _context.Tasks_tb.FindAsync(id);
			if (task == null)
			{
				return NotFound("task not found");
			}

			_context.Tasks_tb.Remove(task);
			await _context.SaveChangesAsync();


			return Ok("Task deleted sucessfully");
		}


        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTask(int id, TasksModel updatedTask)
        //{
        //	// Check if the provided task ID matches the ID of the task being updated
        //	if (id != updatedTask.Id)
        //	{
        //		return BadRequest("Task ID mismatch.");
        //	}


        //	// Find the existing task by ID
        //	var task = await _context.Tasks_tb.FindAsync(id);
        //	//if (task.UserId != updatedTask.UserId)
        //	//{
        //	//	return BadRequest("User ID mismatch.");
        //	//}
        //	if (task == null)
        //	{
        //		return NotFound("Task not found.");
        //	}

        //	// Update the task's properties
        //	task.TaskName = updatedTask.TaskName;
        //	task.Status = updatedTask.Status;
        //	task.CompletedDate = updatedTask.CompletedDate;

        //	// Save changes to the database
        //	await _context.SaveChangesAsync();

        //	// Return a success response
        //	return Ok("Task updated successfully.");

        //}

        [HttpPut("byId/{tid}")]
        public async Task<IActionResult> UpdateTaskById(int tid, [FromBody] TasksModel updatedTask)
        {
            if (tid != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch.");
            }

            // Find and update the task in the database
            var task = await _context.Tasks_tb.FindAsync(tid);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            // Update the task properties
            task.TaskName = updatedTask.TaskName;
            task.Status = updatedTask.Status;
            task.CompletedDate = updatedTask.CompletedDate;

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(task);
        }


        [HttpGet("byId/{tid}")]
        public async Task<IActionResult> GetTaskById(int tid)
        {
            // Find the task by ID
            var task = await _context.Tasks_tb.FindAsync(tid);

            // Check if the task exists
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            return Ok(task); // Return the task details
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTasksByUserId(int id)
        {
            // Fetch tasks associated with the specified userId
            var tasks = await _context.Tasks_tb
                                      .Where(t => t.UserId == id)
                                      .ToListAsync();

            // Check if any tasks were found
            if (tasks == null || tasks.Count == 0)
            {
                return NotFound("No tasks found for the specified user.");
            }

            // Return the list of tasks
            return Ok(tasks);
        }

        //[HttpPut("mark-done/{id}")]
        //public async Task<IActionResult> MarkTaskDone(int id)
        //{
        //    var task = await _context.Tasks_tb.FindAsync(id);
        //    if (task == null)
        //    {
        //        return NotFound("Task not found");
        //    }
        //    task.Status = true;
        //    task.CompletedDate = DateTime.Now;
        //    await _context.SaveChangesAsync();
        //    return Ok("Task marked as done successfully.");
        //}

        //[HttpPut("mark-done/{id}")]
        //public async Task<IActionResult> MarkTaskDone(int id)
        //{
        //    var task = await _context.Tasks_tb.FindAsync(id);
        //    if (task == null) { return NotFound("Task not found"); }

        //    task.Status = true;
        //    task.CompletedDate = DateTime.Now;

        //    await _context.SaveChangesAsync();

        //    // Log or confirm status
        //    var updatedTask = await _context.Tasks_tb.FindAsync(id);
        //    Console.WriteLine($"Task ID: {id}, Status: {updatedTask.Status}"); // Should print "true"

        //    return Ok("Task marked as done successfully.");
        //}


        [HttpPut("toggle-status/{id}")]
        public async Task<IActionResult> ToggleTaskStatus(int id)
        {
            var task = await _context.Tasks_tb.FindAsync(id);
            if (task == null)
            {
                return NotFound("Task not found");
            }

            // Toggle the status: if true, set to false; if false, set to true
            task.Status = !task.Status;

            //// Update CompletedDate only if the task is marked as done (true)
            //task.CompletedDate = task.Status ? DateTime.Now : null;

            await _context.SaveChangesAsync();

            // Log the new status
            Console.WriteLine($"Task ID: {id}, Status: {task.Status}"); // Should print "true" or "false" based on toggle

            return Ok($"Task status toggled to {(task.Status ? "done" : "not done")} successfully.");
        }


    

	}


}



