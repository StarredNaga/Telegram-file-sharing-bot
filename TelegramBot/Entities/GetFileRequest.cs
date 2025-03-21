namespace TelegramBot.Entities;

public record GetFileRequest(string? Name, string? Path,string? Extension, DateTime? From, DateTime? To);