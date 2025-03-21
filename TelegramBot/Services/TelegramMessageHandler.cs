using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.TelegramEventArgs;

namespace TelegramBot.Services;

public class TelegramMessageHandler
{
    public TelegramMessageHandler(TelegramUpdateHandler updateHandler)
    {
        updateHandler.OnTextMessage += OnTextMessage;
    }

    private async Task OnTextMessage(object source, TextMessageArgs e)
    {
        var token = e.CancellationToken;
        var chatId = e.ChatId;
        var client = e.Client;

        var markup = new InlineKeyboardMarkup
        {
            InlineKeyboard = 
            [
                [InlineKeyboardButton.WithCallbackData("Выбрать файл", "get-file")],
                [InlineKeyboardButton.WithCallbackData("Добавить файл", "add-file")]
            ]
        };
        
        await client.SendMessage(chatId, "Выберите опцию:",replyMarkup: markup, cancellationToken: token);
    }
}