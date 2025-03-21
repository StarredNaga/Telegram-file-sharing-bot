using Telegram.Bot;
using Telegram.Bot.Polling;
using TelegramBot.Services;

namespace TelegramBot;

public class TelegramBotWorker : BackgroundService
{
    private readonly TelegramBotClient _bot;
    private readonly TelegramUpdateHandler _updateHandler;

    public TelegramBotWorker(TelegramBotClient bot,
        TelegramUpdateHandler handler, TelegramFileProvider provider)
    {
        _bot = bot;
        _updateHandler = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new ReceiverOptions
        {
            AllowedUpdates = [],
        };
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bot.ReceiveAsync(_updateHandler,cancellationToken: stoppingToken, receiverOptions: options);
            
            await Task.Delay(100, stoppingToken);
        }
    }
}