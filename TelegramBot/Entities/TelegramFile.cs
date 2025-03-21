namespace TelegramBot.Entities;

public class TelegramFile
{
    public Guid Id { get; init; }

    public string? FileId { get; init; }
    
    public string FileName { get; init; }
    
    public string FileDescription { get; init; }

    public TelegramFile(string? fileId, string fileName, string fileDescription)
    {
        FileId = fileId;
        FileName = fileName;
        FileDescription = fileDescription;
    }
}