using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Entities;

namespace TelegramBot.Services;

public class TelegramFileProvider
{
    private readonly TelegramFileRepository _repository;
    private const int PageSize = 5;
    private int _currentPage;
    private int _pageCount;

    public TelegramFileProvider(TelegramFileRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddFile(TelegramFileCreateRequest request)
    {
        if (_repository.GetFile(new TelegramFileSortRequest(null, null, request.FileName, null)) != null)
        {
            throw new Exception("File with same name already exists");
        }
        await _repository.AddFile(request);
    }
    
    public string? GetFileIdFromMessage(Message message)
    {
        return message switch
        {
            {Photo.Length: > 0} => message.Photo.Last().FileId,
            {Document: not null} => message.Document.FileId,
            {Audio:not null} => message.Audio.FileId,
            {Video: not null} => message.Video.FileId,
            _=> null
        };
    }

    public TelegramFileDto? GetFile(TelegramFileSortRequest sortRequest)
    {
        return _repository.GetFile(sortRequest);
    }

    public void AddPage()
    {
        _currentPage++;
        
        if(_currentPage > _pageCount) _currentPage--;
    }

    public void MinusPage()
    {
        _currentPage--;
        
        if(_currentPage < 1) _currentPage = 0;
    }
    
    public List<InlineKeyboardButton[]>? FormatFiles()
    {
        var files = _repository
            .GetFiles(new TelegramFileSortRequest(null, null, null, null));
        
        _pageCount = files.Count / PageSize;

        List<InlineKeyboardButton[]> markup = [];

        if (_pageCount >= 1)
        {
            markup = files.Select(x => InlineKeyboardButton.WithCallbackData(x.FileName, x.FileName))
                .Select(y => new[] { y })
                .Skip(PageSize * _currentPage)
                .Take(PageSize).ToList();
            
            markup.Add([InlineKeyboardButton.WithCallbackData("<--", "minus-page"), InlineKeyboardButton.WithCallbackData("-->", "add-page")]);
            
            return markup;
        }
        
        markup = files.Select(x => InlineKeyboardButton.WithCallbackData(x.FileName, x.FileName))
            .Select(y => new[] { y })
            .ToList();

        return markup.Count == 0 ?
            null : markup;
    }
    
    public async Task SendFile(ITelegramBotClient client,long chatId,CancellationToken token, TelegramFileDto file)
    {
        try
        {
            await client.SendDocument(chatId, file.FileId, cancellationToken:  token);
        }
        catch (Exception)
        {
            await client.SendPhoto(chatId, file.FileId, cancellationToken: token);
        }
        
    }
}