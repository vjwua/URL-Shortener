using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenService tokenService,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await userManager.AddToRoleAsync(user, "User");
        return Ok("Registration successful.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);

        if (user is null)
            return Unauthorized("Invalid credentials.");

        var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
            return Unauthorized("Invalid credentials.");

        var accessToken = await tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(user);

        return Ok(new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}