namespace TodoApi.Models;

public class TodoList
{
	public long Id { get; set; }
	public required string Name { get; set; }
	public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

	// Properties for external synchronization
	public bool IsDeleted { get; set; }
	public DateTime? DeletedAt { get; set; }
	public DateTime LocalUpdatedAt { get; set; }
}
