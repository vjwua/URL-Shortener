using Core.DTOs.ShortUrl;
using FluentValidation;

namespace Core.Validators;

public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlDTO>
{
    public CreateShortUrlValidator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
                .WithMessage("URL cannot be empty.")
            .Must(BeAValidUrl)
                .WithMessage("Only valid http and https URLs are allowed.");
    }

    private static bool BeAValidUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return false;

        return uri.Scheme == Uri.UriSchemeHttp ||
               uri.Scheme == Uri.UriSchemeHttps;
    }
}