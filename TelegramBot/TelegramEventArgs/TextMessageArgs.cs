using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.TelegramEventArgs;

public class TextMessageArgs :  EventArgs
{
    public ITelegramBotClient Client { get; set; }
    
    public Message Message { get; set; }
    
    public long ChatId { get; set; }
    
    public CancellationToken CancellationToken { get; set; }
}