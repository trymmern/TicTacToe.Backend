using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Data;
using TicTacToe.Api.Repository;

const string localhostCors = "localhost";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy(localhostCors, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEntityFrameworkNpgsql().AddDbContext<TicTacToeContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("TicTackToeConnection"));
});
builder.Services.AddControllers();
builder.Services.AddScoped<IGameRepository, GameRepository>();

var app = builder.Build();
app.UseCors(localhostCors);
app.MapControllers();

app.Run();