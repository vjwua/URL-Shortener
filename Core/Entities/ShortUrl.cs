using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities;

public class ShortUrl
{
    public int Id { get; private set; }
    public string? OriginalUrl { get; private set; }
    public string? ShortCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? CreatedByUserId { get; private set; }
}
