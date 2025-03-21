namespace TelegramBot.Entities;

public record TelegramFileSortRequest(Guid? Id, string? FileId, string? FileName, string? FileDescription);