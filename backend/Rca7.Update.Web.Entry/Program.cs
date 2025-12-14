using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<ICustomerTreeRepository, InMemoryCustomerTreeRepository>();
builder.Services.AddSingleton<CustomerTreeService>();

var app = builder.Build();

app.MapControllers();

app.Run();
