using Rca7.Update.Application.Repositories;
using Rca7.Update.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<CosOptions>(builder.Configuration.GetSection("Cos"));

builder.Services.AddSingleton<ICustomerTreeRepository, InMemoryCustomerTreeRepository>();
builder.Services.AddSingleton<IPackageRepository, InMemoryPackageRepository>();
builder.Services.AddSingleton<IReleaseOrderRepository, InMemoryReleaseOrderRepository>();
builder.Services.AddSingleton<IAuditLogRepository, InMemoryAuditLogRepository>();
builder.Services.AddSingleton<CustomerTreeService>();
builder.Services.AddSingleton<AuditLogService>();
builder.Services.AddSingleton<ICosStorageService, CosStorageService>();
builder.Services.AddSingleton<PackageUploadService>();
builder.Services.AddSingleton<ReleaseOrchestrator>();
builder.Services.AddSingleton<ReleaseOrderService>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<ReleaseOrchestrator>());

var app = builder.Build();

app.MapControllers();

app.Run();
