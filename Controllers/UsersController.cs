using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STB_backend.DTOs;
using Supabase.Gotrue;


namespace STB_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        public UsersController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _supabaseClient.From<AppUser>().Where(u => !u.IsDeleted).Get();
            return Ok(users.Models);
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO userDto)
        {
            var userToInsert = new AppUser
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                CreditBalance = 0,
                UserType = Role.USER,
                PartnerTier = PartnerTier.NONE,
                DiscountRate = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                DeletedAt = null
            };
            var response = await _supabaseClient.From<AppUser>().Insert(userToInsert);
            return Ok(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserUpdateDTO userDto)
        {
            var userresponse = await _supabaseClient.From<AppUser>().Where(u => u.Id == id).Get();
            var existingUser = userresponse.Models.FirstOrDefault();
            if (existingUser==null || existingUser.IsDeleted)
            {
                return NotFound(new { message = "A keresett felhasználó nem található, vagy törölve lett." });
            }
            //felhasználó frissítése az adatbázisban
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.PhoneNumber = userDto.PhoneNumber;
            existingUser.UpdatedAt = DateTime.UtcNow;
            var response = await _supabaseClient.From<AppUser>().Update(existingUser);
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string id)
        {
            var userresponse = await _supabaseClient.From<AppUser>().Where(u => u.Id == id).Get();
            var existingUser = userresponse.Models.FirstOrDefault();
            if (existingUser == null || existingUser.IsDeleted)
            {
                return NotFound(new { message = "A keresett felhasználó nem található, vagy már törölve lett." });
            }
            //felhasználó törlése az adatbázisban
            existingUser.IsDeleted = true;
            existingUser.DeletedAt = DateTime.UtcNow;
            existingUser.UpdatedAt = DateTime.UtcNow;
            var response = await _supabaseClient.From<AppUser>().Update(existingUser);
            return Ok(response);
        }
    }
}