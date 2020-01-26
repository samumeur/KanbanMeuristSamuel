using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kanban.Models;

namespace Kanban.Controllers
{
    [Route("Boards")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly DBContext _context;

        public BoardsController(DBContext context)
        {
            _context = context;
        }

       
        /// <summary>
        /// Get all boards
        /// </summary>
        /// <returns>List of boards</returns>
        [HttpGet("GetBoard")]
        public async Task<ActionResult<IEnumerable<Board>>> GetBoard()
        {
            return await _context.Board.ToListAsync();
        }

        /// <summary>
        /// Get board by Id
        /// </summary>
        /// <param name="id">Id of the board</param>
        /// <returns>Board found</returns>
        [HttpGet("GetBoard/{id}")]
        public async Task<ActionResult<Board>> GetBoard(long id)
        {
            var board = await _context.Board.FindAsync(id);

            if (board == null)
            {
                return NotFound();
            }
            return board;
        }

        /// <summary>
        /// Update a board 
        /// </summary>
        /// <param name="id">Id of the board to update</param>
        /// <param name="board">New board's properties</param>
        /// <returns>Nothing</returns>
        [HttpPut("PutBoard/{id}")]
        public async Task<IActionResult> PutBoard(long id, Board board)
        {
            if (id != board.Id)
            {
                return BadRequest();
            }

            _context.Entry(board).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(id))
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
        /// Create a new board
        /// </summary>
        /// <param name="board">Board's properties</param>
        /// <returns>Board created</returns>
        [HttpPost("PostBoard")]
        public async Task<ActionResult<Board>> PostBoard(Board board)
        {
            board.Locked = false;
            _context.Board.Add(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoard", new { id = board.Id }, board);
        }

        /// <summary>
        /// Create a new board without parameter (proper)
        /// </summary>
        /// <returns>Board created</returns>
        [HttpPost("CreateBoard")]
        public async Task<ActionResult<Board>> CreateBoard()
        {
            Board board = new Board();
            board.Locked = false;
            _context.Board.Add(board);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBoard", new { id = board.Id }, board);
        }

        /// <summary>
        /// Check if board exist
        /// </summary>
        /// <param name="id">Id of the board to check</param>
        /// <returns>True if exist / false if not</returns>
        private bool BoardExists(long id)
        {
            return _context.Board.Any(e => e.Id == id);
        }

        /// <summary>
        /// Update the board's status to false if true / true if false
        /// </summary>
        /// <param name="id">Id of the board to update</param>
        /// <returns>Board updated</returns>
        [HttpPut("ChangeLockStatus/{id}")]
        public async Task<Board> ChangeLockStatus(long id)
        {
            Board board = await _context.Board.FindAsync(id);

            if (board == null)
            {
                return null;
            }
            else
            {
                if (board.Locked) board.Locked = false;
                else board.Locked = true;

                _context.Entry(board).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }

            return board;
        }
    }
}
