// Almost New 24/7 Outpost API: the composition root.
// Each Add... and Use... call is an extension method in the Extensions folder.
using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Extensions;
using Microsoft.AspNetCore.Identity;
using AlmostNewOutpost.Api.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOutpostOptions(builder.Configuration);
builder.Services.AddOutpostDatabase();
builder.Services.AddOutpostAuth();
builder.Services.AddOutpostWeather();
builder.Services.AddOutpostApi();
builder.Services.AddOutpostSwagger();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

app.UseOutpostPipeline();

if (app.Environment.IsDevelopment())
{
    await app.CheckDatabaseConnectionAsync();
}

app.Run();
