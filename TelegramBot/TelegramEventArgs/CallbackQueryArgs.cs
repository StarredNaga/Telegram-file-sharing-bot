using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.TelegramEventArgs;

public class CallbackQueryArgs : EventArgs
{
    public ITelegramBotClient Client { get; set; }
    
    public CallbackQuery CallbackQuery { get; set; }
    
    public long ChatId { get; set; }
    
    public CancellationToken CancellationToken { get; set; }
}