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
        public async Task<IActionResult> Register([FromBody] RegisterDTO authDto)
        {
            try
            {
                var response = await _supabaseClient.Auth.SignUp(authDto.Email, authDto.Password);
                if (response == null || response.User == null)
                {
                    return BadRequest(new { message = "A regisztráció sikertelen." });
                }
                var appUser = new AppUser
                {
                    Id = response.User.Id, // Az auth user UUID-ja
                    FirstName = authDto.FirstName,
                    LastName = authDto.LastName,
                    Email = authDto.Email,
                    PhoneNumber = authDto.PhoneNumber,
                    CreditBalance = 0,
                    UserType = Role.USER,
                    PartnerTier = PartnerTier.NONE,
                    DiscountRate = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                await _supabaseClient.From<AppUser>().Insert(appUser);
                return Ok(
                    new
                    {
                        user = appUser.Id,
                        message = "Sikeres regisztráció.",
                        accessToken = response.AccessToken,
                        expiresIn = response.ExpiresIn,
                        refreshToken = response.RefreshToken
                    }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message});
            }   
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO authDto)
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
                    expiresIn = response.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Hibás email vagy jelszó." });
            }
        }
    }
}
