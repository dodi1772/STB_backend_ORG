using Microsoft.AspNetCore.Mvc;
using STB_backend.DTOs;

namespace STB_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        public AuthController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AuthDTO authDto)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignUp(authDto.Email, authDto.Password);
                if (response == null || response.User == null)
                {
                    return BadRequest(new { message = "A regisztráció sikertelen." });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }   
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTO authDto)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignIn(authDto.Email, authDto.Password);
                if (response == null || response.User == null)
                {
                    return Unauthorized(new { message = "Hibás email vagy jelszó." });
                }
                return Ok(new
                {
                    user = response.User.Id,
                    accessToken = response.AccessToken,
                    refreshToken = response.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Hibás email vagy jelszó." });
            }
        }
    }
}
