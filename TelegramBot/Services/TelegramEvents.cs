using TelegramBot.TelegramEventArgs;

namespace TelegramBot.Services;

public static class TelegramEvents
{
    public delegate Task OnTextMessage(object source, TextMessageArgs e);
    
    public delegate Task OnCallbackQuery(object source, CallbackQueryArgs e);
    
    public delegate Task OnReplyMessage(object source, ReplyMessageArgs e);
}