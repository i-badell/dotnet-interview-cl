namespace TodoApi.Models;

public class TodoItem
{
	public long Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; } = "";
	public bool Completed { get; set; } = false;

	public long TodoListId { get; set; }
	public TodoList TodoList { get; set; } = null!;

	public bool IsDeleted { get; set; }
	public DateTime? DeletedAt { get; set; }
	public DateTime LocalUpdatedAt { get; set; }
}
