using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rca7.UpdateAgent.Service.Services;
using Rca7.UpdateAgent.Service.Ipc;
using Rca7.UpdateClient.Shared.Config;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<UpdateConfiguration>(builder.Configuration.GetSection("UpdateConfiguration"));
builder.Services.AddSingleton<AgentStateStore>();
builder.Services.AddSingleton<NamedPipeServerHost>();
builder.Services.AddHostedService<AgentWorker>();

var app = builder.Build();

app.Run();
