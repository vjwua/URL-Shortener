using Core.DTOs.ShortUrl;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlController(IShortUrlService shortUrlService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var links = await shortUrlService.GetAllAsync();
        return Ok(links);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var link = await shortUrlService.GetByIdAsync(id);
            return Ok(link);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateShortUrlDTO dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var link = await shortUrlService.CreateAsync(dto, userId!);
            return CreatedAtAction(nameof(GetById), new { id = link.Id }, link);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            await shortUrlService.DeleteAsync(id, userId!, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAll()
    {
        await shortUrlService.DeleteAllAsync();
        return NoContent();
    }

    [HttpGet("reroute/{shortCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> Reroute(string shortCode)
    {
        try
        {
            var originalUrl = await shortUrlService.GetOriginalUrlByShortCodeAsync(shortCode);
            return Redirect(originalUrl!);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}