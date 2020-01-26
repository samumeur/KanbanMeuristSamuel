using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kanban.Models;

namespace Kanban.Controllers
{
    [Route("Tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly DBContext _context;

        public TasksController(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all tasks from all boards
        /// </summary>
        /// <returns>List of tasks</returns>
        [HttpGet("GetTasks")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        /// <summary>
        /// Get a task by id
        /// </summary>
        /// <param name="id">Id of the task</param>
        /// <returns>Found task</returns>
        [HttpGet("GetTask/{id}")]
        public async Task<ActionResult<Task>> GetTask(long id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        /// <param name="id">Id of the task to update</param>
        /// <param name="task">New properties</param>
        /// <returns>Nothing</returns>
        [HttpPut("PutTask/{id}")]
        public async Task<IActionResult> PutTask(long id, Task task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }
            _context.Entry(task).State = EntityState.Modified;

            //Avoid change of the status
            _context.Entry(task).Property(o => o.Status).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        /// <param name="task">Task's properties to create</param>
        /// <returns>Task created</returns>
        [HttpPost("PostTask")]
        public async Task<ActionResult<Task>> PostTask(Task task)
        {
            task.Status = Task.eStatus.TODO;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Delete a task by id
        /// </summary>
        /// <param name="id">Id of the task to delete</param>
        /// <returns>Task deleted</returns>
        [HttpDelete("DeleteTask/{id}")]
        public async Task<ActionResult<Task>> DeleteTask(long id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return task;
        }

        /// <summary>
        /// Check if a task exist
        /// </summary>
        /// <param name="id">Id of the task to check</param>
        /// <returns>True if exist / false if not</returns>
        private bool TaskExists(long id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        /// <summary>
        /// Get all tasks by board Id
        /// </summary>
        /// <param name="idBoard">Id of the board</param>
        /// <returns>List of tasks</returns>
        [HttpGet("GetTasksByBoard/{idBoard}")]
        public async Task<ActionResult<IEnumerable<Task>>> GetTasksByBoard(long idBoard)
        {
            return await _context.Tasks.Where(m => m.BoardId == idBoard).ToListAsync();
        }

        /// <summary>
        /// Upgrade the task's status
        /// </summary>
        /// <param name="id">Id of the task to change</param>
        /// <returns>Task updated</returns>
        [HttpPut("UpgradeTask/{id}")]
        public async Task<ActionResult<Task>> UpgradeTask(long id)
        {
            Task task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }
            else
            {
                switch (task.Status)
                {
                    case Task.eStatus.TODO:
                        task.Status = Task.eStatus.DOING;
                        break;
                    case Task.eStatus.DOING:
                        task.Status = Task.eStatus.DONE;
                        break;
                    case Task.eStatus.DONE:
                        break;
                }

                _context.Entry(task).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return task;
        }

        /// <summary>
        /// Downgrade task's status
        /// </summary>
        /// <param name="id">Id of the task to change</param>
        /// <returns>Task updated</returns>
        [HttpPut("DowngradeTask/{id}")]
        public async Task<ActionResult<Task>> DowngradeTask(long id)
        {
            Task task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }
            else
            {
                switch (task.Status)
                {
                    case Task.eStatus.TODO:
                        break;
                    case Task.eStatus.DOING:
                        task.Status = Task.eStatus.TODO;
                        break;
                    case Task.eStatus.DONE:
                        task.Status = Task.eStatus.DOING;
                        break;
                }

                _context.Entry(task).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return task;
        }
    }
}