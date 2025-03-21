using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entities;
using TelegramBot.TelegramEventArgs;

namespace TelegramBot.Services;

public class TelegramCallbackQueryHandler
{
    private readonly ForceReplyMarkup _forceReply;
    private TelegramFileDto? _file;
    private readonly TelegramFileProvider _telegramFileProvider;

    public TelegramCallbackQueryHandler(TelegramUpdateHandler updateHandler, TelegramFileProvider telegramFileProvider)
    {
        updateHandler.OnCallBackQuery += OnQuery;
        
        _forceReply = new ForceReplyMarkup {Selective = true};
        _file = null;
        _telegramFileProvider = telegramFileProvider;
    }

    private async Task SendFiles(ITelegramBotClient client, long chatId, CancellationToken token)
    {
        var keyboard = _telegramFileProvider.FormatFiles();

        if (keyboard == null)
        {
            await client.SendMessage(chatId, "Файлы не найдены...", cancellationToken:token);
                    
            return;
        }
                
        await client.SendMessage(chatId, "Выберите файл", replyMarkup: new InlineKeyboardMarkup{InlineKeyboard = keyboard}, cancellationToken: token);
    }

    private async Task SendFile(ITelegramBotClient client, long chatId, CancellationToken token, CallbackQuery query)
    {
        var data = query.Data;
        
        _file = _telegramFileProvider.GetFile(new TelegramFileSortRequest(null, null, data, null));

        if (_file == null)
        {
            await client.SendMessage(chatId, "Файл с таким именем не найден!", cancellationToken: token);
                    
            return;
        }

        var markup = new InlineKeyboardMarkup
        {
            InlineKeyboard =
            [
                [InlineKeyboardButton.WithCallbackData("Скачать файл", "download")],
                [InlineKeyboardButton.WithCallbackData("Отмена", "decline"),]
            ]
        };
                
        await client.SendMessage(chatId, $"{_file.FileName}\n\n{_file.FileDescription}", replyMarkup: markup, cancellationToken: token);
    }

    private async Task AddPage(ITelegramBotClient client, long chatId,int messageId, CancellationToken token)
    {
        _telegramFileProvider.AddPage();
                
        var keyboard = _telegramFileProvider.FormatFiles();

        if (keyboard == null) return;
                
        try
        {
            await client.EditMessageReplyMarkup(chatId, messageId,
                replyMarkup: new InlineKeyboardMarkup(keyboard), cancellationToken: token);
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task MinusPage(ITelegramBotClient client, long chatId, int messageId, CancellationToken token)
    {
        _telegramFileProvider.MinusPage();
                
        var keyboard = _telegramFileProvider.FormatFiles();

        if (keyboard == null) return;

        try
        {
            await client.EditMessageReplyMarkup(chatId, messageId,
                replyMarkup: new InlineKeyboardMarkup(keyboard), cancellationToken: token);
        }
        catch (Exception)
        {
            return;
        }
    }

    private async Task OnQuery(object source, CallbackQueryArgs e)
    {
        var client = e.Client;
        var query = e.CallbackQuery;
        var token = e.CancellationToken;
        var data = query.Data;
        var chatId = query.Message!.Chat.Id;
        var messageId = query.Message.MessageId;
        
        List<InlineKeyboardButton[]>? keyboard;
        
        if(data == null) return;

        switch (data)
        {
            case "get-file":
                await SendFiles(client, chatId, e.CancellationToken);
                break;
            
            case "add-file":
                await client.SendMessage(chatId, "Отправьте файл для добавления", replyMarkup: _forceReply, cancellationToken: token);
                break;
            
            case "download":
                await _telegramFileProvider.SendFile(client, chatId, token, _file!);
                break;
            
            case "decline":
                _file = null;
                break;
            
            case "add-page":
                
                await AddPage(client, chatId, messageId, token);
                
                return;
            
            case "minus-page":
                
                await MinusPage(client, chatId, messageId, token);
                
                return;
            
            default:
                await SendFile(client, chatId, token, e.CallbackQuery);
                break;
        }
        
        await client.DeleteMessage(chatId,messageId,token);
    }
}