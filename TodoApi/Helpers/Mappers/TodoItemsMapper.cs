using TodoApi.Dtos;
using TodoApi.Dtos.Requests;
using TodoApi.Models;

namespace TodoApi.Mappers;

public static class TodoItemsMapper
{
	public static TodoItem ToModel(this CreateTodoItem dto) =>
		new()
		{
			Title = dto.Title,
			Description = string.IsNullOrEmpty(dto.Description) ? "" : dto.Description,
			Completed = false,
		};

	public static Dtos.Responses.TodoItemDto ToDto(this TodoItem model) =>
		new()
		{
			Id = model.Id,
			Title = model.Title,
			Description = model.Description,
			Completed = model.Completed,
		};

	public static void UpdateModel(this TodoItem model, UpdateTodoItem dto)
	{
		model.Title = string.IsNullOrEmpty(dto.Title) ? model.Title : dto.Title;
		model.Description = string.IsNullOrEmpty(dto.Description)
			? model.Description
			: dto.Description;
		model.Completed = dto.Completed ?? model.Completed;
	}
}
