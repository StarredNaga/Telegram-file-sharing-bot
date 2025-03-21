using Microsoft.EntityFrameworkCore;
using TelegramBot.Database;
using TelegramBot.Entities;

namespace TelegramBot.Services;

public class TelegramFileRepository
{
    private readonly TelegramFileContext _context;

    public TelegramFileRepository(TelegramFileContext context)
    {
        _context = context;
    }

    private IQueryable<TelegramFile> CreateQuery(TelegramFileSortRequest request)
    {
        var files = _context.Files.AsNoTracking();
        
        if(request.FileId != null) files = files.Where(x => x.FileId == request.FileId);
        
        if(request.Id != null) files = files.Where(x => x.Id == request.Id);
        
        if(request.FileDescription != null) files = files.Where(x => x.FileDescription == request.FileDescription);
        
        if(request.FileName != null) files = files.Where(x => x.FileName == request.FileName);
        
        return files;
    }

    public List<TelegramFileDto> GetFiles(TelegramFileSortRequest request)
    {
        return CreateQuery(request)
            .Select(x => new TelegramFileDto(x.FileId, x.FileName, x.FileDescription))
            .ToList();
    }

    public TelegramFileDto? GetFile(TelegramFileSortRequest request)
    {
        return GetFiles(request).FirstOrDefault();
    }

    public async Task<TelegramFileDto?> AddFile(TelegramFileCreateRequest request)
    {
        var file = new TelegramFile(request.FileId, request.FileName, request.FileDescription);
        
        await _context.AddAsync(file);
        
        await _context.SaveChangesAsync();
        
        return new TelegramFileDto(file.FileId,file.FileName, file.FileDescription);
    }

    public async Task UpdateFiles(TelegramFileUpdateRequest updateRequest, TelegramFileSortRequest sortRequest)
    {
        var files = CreateQuery(sortRequest);
        
        if(updateRequest.FileName != null) 
            await files
            .ExecuteUpdateAsync(x => 
                x.SetProperty(y => y.FileName, updateRequest.FileName));
       
        if(updateRequest.FileDescription != null)
            await files
                .ExecuteUpdateAsync(x => 
                    x.SetProperty(y => y.FileDescription, updateRequest.FileDescription));

        await _context.SaveChangesAsync();
    }

    public async Task DeleteFiles(TelegramFileSortRequest sortRequest)
    {
        var files = CreateQuery(sortRequest);

        await files.ExecuteDeleteAsync();
        
        await _context.SaveChangesAsync();
    }
}