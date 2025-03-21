namespace TelegramBot.Entities;

public record TelegramFileCreateRequest
{
    public string FileName  { get; set; }
    
    public string FileDescription {get; set; }
    
    public string? FileId { get; set; }
}