using Microsoft.EntityFrameworkCore;
using PriceMicroservice.Services;
using PriceRack.Configs;
using PriceRack.DataAccess;
using PriceRack.DataAccess.DBContexts;
using PriceRack.DI;
using PriceRack.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterDependencies();
builder.Services.ConfigureDatabase();
builder.Services.ConfigureLogging(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MigrateToLatest();

app.Run();
