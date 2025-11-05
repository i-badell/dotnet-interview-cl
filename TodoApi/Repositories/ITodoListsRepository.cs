namespace TodoApi.Repositories;

public interface ITodoListsRepository
{
	Task<bool> ListExistsAsync(long id);
}