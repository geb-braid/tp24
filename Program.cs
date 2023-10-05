using Microsoft.EntityFrameworkCore;
using tp24_api.Models;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Services.AddControllers();
appBuilder.Services.AddDbContext<ReceivablesContext>(options => options.UseInMemoryDatabase("ReceivableList"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
appBuilder.Services.AddEndpointsApiExplorer();
appBuilder.Services.AddSwaggerGen();

var app = appBuilder.Build();

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
