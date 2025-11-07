using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Dtos.Requests;
using TodoApi.Dtos.Responses;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Tests;

#nullable disable
public class TodoItemsControllerTests
{
	private ITodoListsRepository repository;

	private DbContextOptions<TodoContext> DatabaseContextOptions()
	{
		return new DbContextOptionsBuilder<TodoContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
	}

	private void PopulateDatabaseContext(TodoContext context)
	{
		// List 1 with two items
		var list1 = new TodoList { Id = 1, Name = "Task list 1" };
		list1.TodoItems.Add(
			new TodoItem
			{
				Id = 1,
				Title = "Task 1",
				Description = "Description 1",
				Completed = false,
				TodoListId = 1,
			}
		);
		list1.TodoItems.Add(
			new TodoItem
			{
				Id = 2,
				Title = "Task 2",
				Description = "Description 2",
				Completed = false,
				TodoListId = 1,
			}
		);

		// Another list to verify scoping by listId
		var list2 = new TodoList { Id = 2, Name = "Task list 2" };
		list2.TodoItems.Add(
			new TodoItem
			{
				Id = 3,
				Title = "Other list item",
				Description = "Desc",
				Completed = true,
				TodoListId = 2,
			}
		);

		context.TodoList.AddRange(list1, list2);
		context.SaveChanges();
	}

	// ---------- GET: /api/todolists/{listId}/todoitems ----------
	[Fact]
	public async Task GetTodoItems_WhenListExists_ReturnsOkAndItemsFilteredByList()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.GetTodoItems(1);

		Assert.IsType<OkObjectResult>(result.Result);
		var list = ((result.Result as OkObjectResult).Value as IList<TodoItemDto>);
		Assert.Equal(2, list.Count); // Only items from listId=1
		Assert.DoesNotContain(list, x => x.Id == 3); // Item from list 2 must not appear
	}

	[Fact]
	public async Task GetTodoItems_WhenListDoesNotExist_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.GetTodoItems(999);

		Assert.IsType<NotFoundResult>(result.Result);
	}

	// ---------- GET: /api/todolists/{listId}/todoitems/{id} ----------
	[Fact]
	public async Task GetTodoItem_WhenItemExistsAndBelongsToList_ReturnsOkWithDto()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.GetTodoItem(1, 1);

		Assert.IsType<OkObjectResult>(result.Result);
		var dto = (result.Result as OkObjectResult).Value as TodoItemDto;
		Assert.Equal(1, dto.Id);
		Assert.Equal("Task 1", dto.Title);
	}

	[Fact]
	public async Task GetTodoItem_WhenListDoesNotExist_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.GetTodoItem(999, 1);

		Assert.IsType<NotFoundResult>(result.Result);
	}

	[Fact]
	public async Task GetTodoItem_WhenItemNotInListOrMissing_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		// id exists in another list
		var resultWrongList = await controller.GetTodoItem(1, 3);
		Assert.IsType<NotFoundResult>(resultWrongList.Result);

		// id does not exist
		var resultMissing = await controller.GetTodoItem(1, 999);
		Assert.IsType<NotFoundResult>(resultMissing.Result);
	}

	// ---------- PUT: /api/todolists/{listId}/todoitems/{id} ----------
	[Fact]
	public async Task PutTodoItem_WhenOk_UpdatesAndReturnsOkWithDto()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var payload = new UpdateTodoItem
		{
			Title = "Task 1 - Updated",
			Description = "New description",
			Completed = true,
		};

		var result = await controller.PutTodoItem(1, 1, payload);

		Assert.IsType<OkObjectResult>(result);
		var dto = (result as OkObjectResult).Value as TodoItemDto;
		Assert.Equal(1, dto.Id);
		Assert.Equal("Task 1 - Updated", dto.Title);
		Assert.True(dto.Completed);

		// Verify persisted changes
		var updated = await context.TodoItem.SingleAsync(t => t.Id == 1);
		Assert.Equal("Task 1 - Updated", updated.Title);
		Assert.True(updated.Completed);
	}

	[Fact]
	public async Task PutTodoItem_WhenListDoesNotExist_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var payload = new UpdateTodoItem
		{
			Title = "X",
			Description = "Y",
			Completed = false,
		};
		var result = await controller.PutTodoItem(999, 1, payload);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task PutTodoItem_WhenItemDoesNotExistInList_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var payload = new UpdateTodoItem
		{
			Title = "X",
			Description = "Y",
			Completed = false,
		};

		// Item exists but in another list
		var resultWrongList = await controller.PutTodoItem(1, 3, payload);
		Assert.IsType<NotFoundResult>(resultWrongList);

		// Item missing
		var resultMissing = await controller.PutTodoItem(1, 999, payload);
		Assert.IsType<NotFoundResult>(resultMissing);
	}

	// ---------- POST: /api/todolists/{listId}/todoitems ----------
	[Fact]
	public async Task PostTodoItem_WhenListExists_CreatesAndReturnsCreatedAtWithDto()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var payload = new CreateTodoItem
		{
			Title = "New Task",
			Description = "Brand new",
			Completed = false,
		};

		var result = await controller.PostTodoItem(1, payload);

		var created = Assert.IsType<CreatedAtActionResult>(result.Result);
		Assert.Equal("GetTodoItem", created.ActionName);

		var dto = Assert.IsType<TodoItemDto>(created.Value);
		Assert.Equal("New Task", dto.Title);
		Assert.False(dto.Completed);

		// persisted and associated to list 1
		var saved = await context.TodoItem.SingleAsync(t => t.Id == dto.Id);
		Assert.Equal(1, saved.TodoListId);
	}

	[Fact]
	public async Task PostTodoItem_WhenListDoesNotExist_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var payload = new CreateTodoItem
		{
			Title = "X",
			Description = "Y",
			Completed = false,
		};

		var result = await controller.PostTodoItem(999, payload);

		Assert.IsType<NotFoundResult>(result.Result);
	}

	// ---------- DELETE: /api/todolists/{listId}/todoitems/{id} ----------
	[Fact]
	public async Task DeleteTodoItem_WhenOk_RemovesAndReturnsNoContent()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.DeleteTodoItem(1, 1);

		Assert.IsType<NoContentResult>(result);

		var exists = await context.TodoItem.AnyAsync(t => t.Id == 1);
		Assert.False(exists);
	}

	[Fact]
	public async Task DeleteTodoItem_WhenListDoesNotExist_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		var result = await controller.DeleteTodoItem(999, 1);

		Assert.IsType<NotFoundResult>(result);
	}

	[Fact]
	public async Task DeleteTodoItem_WhenItemNotInListOrMissing_ReturnsNotFound()
	{
		using var context = new TodoContext(DatabaseContextOptions());
		PopulateDatabaseContext(context);
		repository = new TodoListsRepository(context);
		var controller = new TodoItemsController(context, repository);

		// exists but belongs to another list
		var wrongList = await controller.DeleteTodoItem(1, 3);
		Assert.IsType<NotFoundResult>(wrongList);

		// missing
		var missing = await controller.DeleteTodoItem(1, 999);
		Assert.IsType<NotFoundResult>(missing);
	}
}
