using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STB_backend.DTOs;
using STB_backend.Models;

namespace STB_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssetsController : ControllerBase
    {
        private readonly Supabase.Client _supabaseClient;
        public AssetsController(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }
        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var assets = await _supabaseClient.From<Asset>().Get();
            return Ok(assets.Models);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] AssetCreateDTO assetDto)
        {
            var assetToInsert = new Asset
            {
                Name = assetDto.Name,
                Description = assetDto.Description,
                RoomId = assetDto.roomId,
                BaseCreditPricePerHour = assetDto.baseCreditPricePerHour,
                MonthlyDividendCredits = assetDto.monthlyDividendCredits,
                OwnerId = assetDto.ownerId,
                IsActive = true,
                IsDeleted = false,
                DeletedAt = null
            };
            var response = await _supabaseClient.From<Asset>().Insert(assetToInsert);
            return Ok(response);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset([FromRoute] string id, [FromBody] AssetUpdateDTO assetDto)
        {
            var assetresponse = await _supabaseClient.From<Asset>().Where(r => r.Id == id).Get();
            var existingAsset = assetresponse.Models.FirstOrDefault();
            if (existingAsset == null || existingAsset.IsDeleted)
            {
                return NotFound(new { message = "A keresett eszköz nem található, vagy törölve lett." });
            }
            //eszköz frissítése az adatbázisban
            existingAsset.Name = assetDto.Name;
            existingAsset.Description = assetDto.Description;
            existingAsset.RoomId = assetDto.roomId;
            existingAsset.BaseCreditPricePerHour = assetDto.baseCreditPricePerHour;
            existingAsset.MonthlyDividendCredits = assetDto.monthlyDividendCredits;
            existingAsset.OwnerId = assetDto.ownerId;
            existingAsset.IsActive = assetDto.IsActive;
            var response = await _supabaseClient.From<Asset>().Update(existingAsset);
            return Ok(response);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset([FromRoute] string id)
        {
            var assetresponse = await _supabaseClient.From<Asset>().Where(r => r.Id == id).Get();
            var existingAsset = assetresponse.Models.FirstOrDefault();
            if (existingAsset == null || existingAsset.IsDeleted)
            {
                return NotFound(new { message = "A keresett eszköz nem található, vagy már törölve lett." });
            }
            //eszköz törlése az adatbázisban
            existingAsset.IsDeleted = true;
            existingAsset.DeletedAt = DateTime.UtcNow;
            var response = await _supabaseClient.From<Asset>().Update(existingAsset);
            return Ok(response);
        }
    }
}
