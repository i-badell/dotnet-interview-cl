using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dtos.Requests;
using TodoApi.Dtos.Responses;
using TodoApi.Mappers;
using TodoApi.Repositories;

namespace TodoApi.Controllers;

[Route("api/todolists/{listId}/todoitems")]
[ApiController]
public class TodoItemsController : ControllerBase
{
	private readonly TodoContext _context;
	private readonly ITodoListsRepository _todoListsRepository;

	public TodoItemsController(TodoContext context, ITodoListsRepository todoListsRepository)
	{
		_context = context;
		_todoListsRepository = todoListsRepository;
	}

	// GET: api/todoitems
	[HttpGet]
	public async Task<ActionResult<IList<TodoItemDto>>> GetTodoItems(long listId)
	{
		var listExists = await _todoListsRepository.ListExistsAsync(listId);
		if (!listExists)
		{
			return NotFound();
		}

		var todoItems = await _context
			.TodoItem.Where(ti => ti.TodoListId == listId)
			.Select(ti => ti.ToDto())
			.ToListAsync();

		return Ok(todoItems);
	}

	// GET: api/todolists/5/todoitems/10
	[HttpGet("{id}")]
	public async Task<ActionResult<TodoItemDto>> GetTodoItem(long listId, long id)
	{
		var listExists = await _todoListsRepository.ListExistsAsync(listId);
		if (!listExists)
		{
			return NotFound();
		}

		var todoItem = await _context.TodoItem.FirstOrDefaultAsync(ti =>
			ti.Id == id && ti.TodoListId == listId
		);
		if (todoItem == null)
		{
			return NotFound();
		}

		return Ok(todoItem.ToDto());
	}

	// PUT: api/todolists/5
	// To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPut("{id}")]
	public async Task<ActionResult> PutTodoItem(long listId, long id, UpdateTodoItem payload)
	{
		var listExists = await _todoListsRepository.ListExistsAsync(listId);
		if (!listExists)
		{
			return NotFound();
		}

		var todoItem = await _context.TodoItem.FirstOrDefaultAsync(ti =>
			ti.Id == id && ti.TodoListId == listId
		);
		if (todoItem == null)
		{
			return NotFound();
		}

		todoItem.UpdateModel(payload);
		await _context.SaveChangesAsync();

		return Ok(todoItem.ToDto());
	}

	// POST: api/todolists
	// To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
	[HttpPost]
	public async Task<ActionResult<TodoItemDto>> PostTodoItem(long listId, CreateTodoItem payload)
	{
		var list = await _context.TodoList.FindAsync(listId);
		if (list == null)
		{
			return NotFound();
		}

		var todoItem = payload.ToModel();
		list.TodoItems.Add(todoItem);
		await _context.SaveChangesAsync();

		return CreatedAtAction("GetTodoItem", new { listId, id = todoItem.Id }, todoItem.ToDto());
	}

	// DELETE: api/todolists/5
	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteTodoItem(long listId, long id)
	{
		var listExists = await _todoListsRepository.ListExistsAsync(listId);
		if (!listExists)
		{
			return NotFound();
		}

		var todoItem = await _context.TodoItem.FirstOrDefaultAsync(ti =>
			ti.Id == id && ti.TodoListId == listId
		);
		if (todoItem == null)
		{
			return NotFound();
		}

		_context.TodoItem.Remove(todoItem);
		await _context.SaveChangesAsync();

		return NoContent();
	}
}
