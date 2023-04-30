using Microsoft.EntityFrameworkCore;
using PriceMicroservice.Services;
using PriceRack.DataAccess;
using PriceRack.Services;
using System.Configuration;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContextFactory<PriceContext>(options =>
options.UseSqlite("Data Source=prices.db"));
builder.Services.AddScoped<IPriceService, PriceService>();
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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PriceContext>();
    dbContext.Database.Migrate();
}

app.Run();
