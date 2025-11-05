namespace TodoApi.Dtos.Responses;

public class TodoItemDto
{
	public long Id { get; set; }
	public required string Title { get; set; }
	public string Description { get; set; } = "";
	public bool Completed { get; set; } = false;
}
