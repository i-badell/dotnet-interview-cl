using System.ComponentModel.DataAnnotations;

namespace TodoApi.Dtos.Requests;

public class CreateTodoItem
{
	[Required(
		AllowEmptyStrings = false,
		ErrorMessage = "Title is required when creating a todo item."
	)]
	public string Title { get; set; }
	public string? Description { get; set; }
	public bool? Completed { get; set; } = false;
}
