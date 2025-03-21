using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entities;
using TelegramBot.TelegramEventArgs;

namespace TelegramBot.Services;

public class TelegramFileHandler
{
    private TelegramFileCreateRequest _createRequest;
    private readonly ForceReplyMarkup _forceReply;
    private readonly TelegramFileProvider _telegramFileProvider;
    public TelegramFileHandler(TelegramUpdateHandler updateHandler, TelegramFileProvider telegramFileProvider)
    {
        updateHandler.OnReplyMessage += HandleReplyMessage;
        
        _createRequest = new TelegramFileCreateRequest();
        _forceReply = new ForceReplyMarkup {Selective = true};
        _telegramFileProvider = telegramFileProvider;
    }

    private async Task GetFile(ITelegramBotClient client, CancellationToken token, long chatId, Message message)
    {
        var fileId = _telegramFileProvider.GetFileIdFromMessage(message);

        if (fileId == null)
        {
            await client.SendMessage(chatId, "Неверный формат файла!", cancellationToken: token);
                    
            return;
        }
                
        _createRequest.FileId = fileId;
                
        await client.SendMessage(chatId, "Введите имя файла (обязательно!)", replyMarkup: _forceReply, cancellationToken: token);
    }

    private async Task GetFileName(ITelegramBotClient client, CancellationToken token, long chatId, Message message)
    {
        if (message.Text is null)
        {
            await client.SendMessage(chatId, "Имя файла не может быть пустым!", cancellationToken: token);
                    
            _createRequest = new TelegramFileCreateRequest();
                    
            return;
        }
                
        _createRequest.FileName = message.Text;
                
        await client.SendMessage(chatId, "Введите описание файла (Введите '/' если описание пустое)", replyMarkup: _forceReply, cancellationToken: token);
    }

    private async Task GetFileDescription(ITelegramBotClient client, CancellationToken token, long chatId,
        Message message)
    {
        _createRequest.FileDescription = (message.Text == "/" ? "Описания нет" : message.Text)!;

        try
        {
            await _telegramFileProvider.AddFile(_createRequest);
        }
        catch (Exception ex)
        {
            await client.SendMessage(chatId, "Файл с таким именем уже создан!", cancellationToken: token);

            return;
        }
                
        await client.SendMessage(chatId, $"Файл '{_createRequest.FileName}' успешно добавлен!", cancellationToken: token);
    }

    private async Task HandleReplyMessage(object? sender, ReplyMessageArgs e)
    {
        var message = e.Message;
        var replyMessage = e.Message.ReplyToMessage;
        var client = e.Client;
        var chatId = e.ChatId;
        var token = e.CancellationToken;
        
        if (replyMessage!.Text == null) return;

        switch (replyMessage.Text)
        {
            case "Отправьте файл для добавления":
                await GetFile(client, token, chatId, message);
                break;
            
            case "Введите имя файла (обязательно!)":
                
                await GetFileName(client, token, chatId, message);
                break;
            
            case "Введите описание файла (Введите '/' если описание пустое)":
                await  GetFileDescription(client, token, chatId, message);
                break;
        }
    }
}