namespace Core.DTOs.ShortUrl;

public class ShortUrlResponseDTO
{
    public int Id { get; set; }
    public string? OriginalUrl { get; set; }
    public string? ShortCode { get; set; }
}
