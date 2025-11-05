using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder
	.Services.AddDbContext<TodoContext>(opt =>
		opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"))
	)
	.AddEndpointsApiExplorer()
	.AddControllers();

builder.Services.AddScoped<ITodoListsRepository, TodoListsRepository>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();
app.Run();
