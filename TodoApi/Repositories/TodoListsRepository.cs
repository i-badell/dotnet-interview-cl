using Microsoft.EntityFrameworkCore;
using TodoApi.Data;

namespace TodoApi.Repositories;

public class TodoListsRepository : ITodoListsRepository
{
	private readonly TodoContext _context;

	public TodoListsRepository(TodoContext context)
	{
		_context = context;
	}

	public async Task<bool> ListExistsAsync(long id)
	{
		return await _context.TodoList.AnyAsync(tl => tl.Id == id);
	}
}
