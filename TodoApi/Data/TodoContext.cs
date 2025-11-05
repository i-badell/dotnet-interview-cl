using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class TodoContext : DbContext
{
	public TodoContext(DbContextOptions<TodoContext> options)
		: base(options) { }

	public DbSet<TodoList> TodoList { get; set; } = default!;
	public DbSet<TodoItem> TodoItem { get; set; } = default!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder
			.Entity<TodoItem>()
			.HasOne(e => e.TodoList)
			.WithMany(e => e.TodoItems)
			.HasForeignKey(e => e.TodoListId)
			.IsRequired();

		modelBuilder.Entity<TodoItem>().Property(e => e.Title).IsRequired();
	}
}
