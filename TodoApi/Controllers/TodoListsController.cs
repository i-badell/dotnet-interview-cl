using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos.Requests;
using TodoApi.Dtos.Responses;
using TodoApi.Mappers;
using TodoApi.Models;

namespace TodoApi.Controllers
{
	[Route("api/todolists")]
	[ApiController]
	public class TodoListsController : ControllerBase
	{
		private readonly TodoContext _context;

		public TodoListsController(TodoContext context)
		{
			_context = context;
		}

		// GET: api/todolists
		[HttpGet]
		public async Task<ActionResult<IList<TodoListDto>>> GetTodoLists()
		{
			var todoLists = await _context.TodoList.Select(tl => tl.ToDto()).ToListAsync();
			return Ok(todoLists);
		}

		// GET: api/todolists/5
		[HttpGet("{id}")]
		public async Task<ActionResult<TodoListDto>> GetTodoList(long id)
		{
			var todoList = await _context.TodoList.FindAsync(id);
			if (todoList == null)
			{
				return NotFound();
			}

			return Ok(todoList.ToDto());
		}

		// PUT: api/todolists/5
		// To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPut("{id}")]
		public async Task<ActionResult> PutTodoList(long id, UpdateTodoList payload)
		{
			var todoList = await _context.TodoList.FindAsync(id);
			if (todoList == null)
			{
				return NotFound();
			}

			todoList.Name = payload.Name;
			await _context.SaveChangesAsync();

			return Ok(todoList.ToDto());
		}

		// POST: api/todolists
		// To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoList payload)
		{
			var todoList = payload.ToModel();

			_context.TodoList.Add(todoList);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetTodoList", new { id = todoList.Id }, todoList.ToDto());
		}

		// DELETE: api/todolists/5
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteTodoList(long id)
		{
			var todoList = await _context.TodoList.FindAsync(id);
			if (todoList == null)
			{
				return NotFound();
			}

			_context.TodoList.Remove(todoList);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
