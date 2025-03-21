using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot;
using TelegramBot.Configurations;
using TelegramBot.Database;
using TelegramBot.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfigurations"));

var configuration = builder.Services.BuildServiceProvider().GetService<IOptions<BotConfiguration>>()!.Value;

if (configuration == null) throw new Exception("Bot configuration not found");

var connection = builder.Configuration.GetConnectionString("Connection");

if (connection == null) throw new Exception("Database connection not found");

builder.Services.AddSingleton(new TelegramBotClient(configuration.Token));
builder.Services.AddSingleton(new TelegramFileContext(connection));
builder.Services.AddSingleton<TelegramFileRepository>();
builder.Services.AddSingleton<TelegramFileProvider>();
builder.Services.AddSingleton<TelegramUpdateHandler>();
builder.Services.AddSingleton<TelegramFileHandler>();
builder.Services.AddSingleton<TelegramCallbackQueryHandler>();
builder.Services.AddSingleton<TelegramMessageHandler>();
builder.Services.AddHostedService<TelegramBotWorker>();

var host = builder.Build();

host.Services.GetRequiredService<TelegramFileContext>().Database.EnsureCreated();

host.Services.GetRequiredService<TelegramFileHandler>();
host.Services.GetRequiredService<TelegramCallbackQueryHandler>();
host.Services.GetRequiredService<TelegramMessageHandler>();

host.Run();