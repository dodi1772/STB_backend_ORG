using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STB_backend.DTOs;
using STB_backend.Models;

namespace STB_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        public RoomsController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _supabaseClient.From<Room>().Where(r => !r.IsDeleted).Get();
            return Ok(rooms.Models);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDTO roomDto)
        {
            var roomToInsert = new Room
            {
                Name = roomDto.Name,
                Description = roomDto.Description,
                Capacity = roomDto.Capacity,
                baseCreditPricePerHour = roomDto.baseCreditPricePerHour,
                IsDeleted = false,
                DeletedAt = null
            };
            var response = await _supabaseClient.From<Room>().Insert(roomToInsert);
            return Ok(response);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom([FromRoute] string id, [FromBody] RoomUpdateDTO roomDto)
        {
            var roomresponse = await _supabaseClient.From<Room>().Where(r => r.Id == id).Get();
            var existingRoom = roomresponse.Models.FirstOrDefault();
            if (existingRoom == null || existingRoom.IsDeleted)
            {
                return NotFound(new { message = "A keresett szoba nem található, vagy törölve lett." });
            }
            //szoba frissítése az adatbázisban
            existingRoom.Name = roomDto.Name;
            existingRoom.Description = roomDto.Description;
            existingRoom.Capacity = roomDto.Capacity;
            existingRoom.baseCreditPricePerHour = roomDto.baseCreditPricePerHour;
            var response = await _supabaseClient.From<Room>().Update(existingRoom);
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] string id)
        {
            var roomresponse = await _supabaseClient.From<Room>().Where(r => r.Id == id).Get();
            var existingRoom = roomresponse.Models.FirstOrDefault();
            if (existingRoom == null || existingRoom.IsDeleted)
            {
                return NotFound(new { message = "A keresett szoba nem található, vagy már törölve lett." });
            }
            //szoba törlése az adatbázisban
            existingRoom.IsDeleted = true;
            existingRoom.DeletedAt = DateTime.UtcNow;
            var response = await _supabaseClient.From<Room>().Update(existingRoom);
            return Ok(response);
        }
    }
}