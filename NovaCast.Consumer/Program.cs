using NovaCast.Consumer;
using NovaCast.Consumer.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddHostedService<KafkaConsumerWorker>();

var host = builder.Build();
host.Run();