using Core.Models.Configuration;
using Infraestructure.Services;
using Logic.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
AppConfiguration configuration = new(builder.Configuration);
builder.Services.AddSingleton(x => configuration);
builder.Services.AddTransient<ClientsFtpUploadAppService>();
builder.Services.AddTransient<FtpService>();
builder.Services.AddTransient<NotificationService>();
builder.Services.AddTransient<BQService>();
builder.Services.AddTransient<EPPlusService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
