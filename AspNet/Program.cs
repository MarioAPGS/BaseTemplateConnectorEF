using Infrastructure;
using Infrastructure.Providers;
using Infrastructure.Repositories.Base;
using Infrastructure.Repositories.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PlumberlabDbContext>(opt =>
        opt.UseNpgsql(builder.Configuration.GetConnectionString("PlumberlabContext")));

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      builder =>
                      {
                          builder
                          .AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddSingleton<IRepositoryBase, RepositoryBase>();
builder.Services.AddSingleton<IRepositorySecurity, RepositorySecurity>();
builder.Services.AddSingleton<IDockerProvider, DockerProvider>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.Run();
