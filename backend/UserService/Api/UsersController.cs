using Microsoft.AspNetCore.Mvc;
using UserService.Abstractions;

namespace UserService.Api;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    public UsersController(IUserService users) => _users = users;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(
        [FromBody] RegisterRequestDTO request, CancellationToken ct)
    {

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email is required");
        if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 8)
            return BadRequest("Password too short");
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest("FullName is required");

        Console.WriteLine($"[register] attempt for {request.Email}");

        var dto = await _users.RegisterAsync(request, ct);
        return Ok(dto); // ответ: { id, email, fullName }
    }
}