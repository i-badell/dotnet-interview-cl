namespace TodoApi.Dtos.Requests;

public class UpdateTodoItem
{
	public string? Title { get; set; }
	public string? Description { get; set; }
	public bool? Completed { get; set; }
}
