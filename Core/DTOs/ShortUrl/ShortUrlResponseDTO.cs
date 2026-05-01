namespace Core.DTOs.ShortUrl;

public class ShortUrlResponseDTO
{
    public Guid Id { get; set; }
    public string? OriginalUrl { get; set; }
    public string? ShortCode { get; set; }
    public DateTime createdAt { get; set; }
    public string? createdByUserId { get; set; }
}
