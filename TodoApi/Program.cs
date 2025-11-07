using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder
	.Services.AddDbContext<TodoContext>(opt =>
		opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"))
	)
	.AddEndpointsApiExplorer()
	.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
	})
	.AddControllers();

builder.Services.AddScoped<ITodoListsRepository, TodoListsRepository>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
app.MapControllers();
app.Run();
