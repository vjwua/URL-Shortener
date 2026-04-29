namespace Core.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }
    public string? OriginalUrl { get; set; }
    public string? ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByUserId { get; set; }
}
