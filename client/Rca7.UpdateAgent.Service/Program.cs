using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rca7.UpdateAgent.Service.Services;
using Rca7.UpdateAgent.Service.Ipc;
using Rca7.UpdateClient.Shared.Config;

// 创建主机应用程序构建器
var builder = Host.CreateApplicationBuilder(args);

// 配置更新配置选项
builder.Services.Configure<UpdateConfiguration>(builder.Configuration.GetSection("UpdateConfiguration"));

// 注册代理状态存储服务
builder.Services.AddSingleton<AgentStateStore>();

// 注册命名管道服务端主机
builder.Services.AddSingleton<NamedPipeServerHost>();

// 注册代理工作服务
builder.Services.AddHostedService<AgentWorker>();

var app = builder.Build();

// 运行应用程序
app.Run();
