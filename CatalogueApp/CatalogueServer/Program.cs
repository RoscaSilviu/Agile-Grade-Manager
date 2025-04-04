using CatalogueServer.Controllers;
using CatalogueServer.Repositories;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AssignmentRepository>();
builder.Services.AddScoped<GradeRepository>();
builder.Services.AddScoped<ClassRepository>();


builder.Services.AddSingleton<Database>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Initialize the database on startup
var db = app.Services.GetRequiredService<Database>();
Console.WriteLine("Database initialized!");

app.Run();
