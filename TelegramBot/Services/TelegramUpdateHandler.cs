using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.TelegramEventArgs;

namespace TelegramBot.Services;

public class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ILogger<TelegramUpdateHandler> _logger;
    
    public event TelegramEvents.OnTextMessage OnTextMessage;
    public event TelegramEvents.OnReplyMessage OnReplyMessage;
    public event TelegramEvents.OnCallbackQuery OnCallBackQuery;
    
    public TelegramUpdateHandler(ILogger<TelegramUpdateHandler> logger)
    {
        _logger = logger;
    }
    
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if(update.Message == null && update.CallbackQuery == null) return;
        
        var action = update switch
        {
            { Message.ReplyToMessage: not null } => OnReplyMessage(this, new ReplyMessageArgs
            {
                ChatId = update.Message.Chat.Id,
                Message = update.Message,
                Client = botClient,
                CancellationToken =  cancellationToken
            }),
            { Message: { Text: not null, ReplyToMessage: null } } => OnTextMessage
            (this, new TextMessageArgs
            {
                Client = botClient,
                ChatId = update.Message.Chat.Id,
                Message = update.Message,
                CancellationToken =  cancellationToken
            }),
            { CallbackQuery: not null } => OnCallBackQuery(this, new CallbackQueryArgs
            {
                Client = botClient,
                ChatId = update.CallbackQuery.Message!.Chat.Id,
                CallbackQuery = update.CallbackQuery,
                CancellationToken =  cancellationToken
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(update), update, null)
        };

        await action;
    }
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        if(exception?.Message == null) return Task.CompletedTask;
        
        _logger.LogError(exception, exception.Message);

        return Task.CompletedTask;
    }
}