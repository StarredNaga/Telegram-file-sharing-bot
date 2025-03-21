namespace TelegramBot.Entities;

public class TelegramFileDto
{
    public string? FileId { get; init; }
    
    public string FileName { get; init; }
    
    public string FileDescription { get; init; }

    public TelegramFileDto(string? fileId, string fileName, string fileDescription)
    {
        FileId = fileId;
        FileName = fileName;
        FileDescription = fileDescription;
    }
}