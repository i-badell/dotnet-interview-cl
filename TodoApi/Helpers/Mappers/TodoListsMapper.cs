using TodoApi.Dtos.Responses;
using TodoApi.Models;

namespace TodoApi.Mappers;

public static class TodoListsMapper
{
	public static TodoListDto ToDto(this TodoList model) =>
		new() { Id = model.Id, Name = model.Name };

	public static TodoList ToModel(this Dtos.Requests.CreateTodoList dto) =>
		new() { Name = dto.Name };
}
