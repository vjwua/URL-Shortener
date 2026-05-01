using Core.DTOs.AlgorithmInfo;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlgorithmInfoController(IAlgorithmInfoService algorithmInfoService) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        try
        {
            var info = await algorithmInfoService.GetAsync();
            return Ok(info);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] AlgorithmInfoDTO dto)
    {
        try
        {
            await algorithmInfoService.CreateAsync(dto);
            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] AlgorithmInfoDTO dto)
    {
        try
        {
            await algorithmInfoService.UpdateAsync(dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}