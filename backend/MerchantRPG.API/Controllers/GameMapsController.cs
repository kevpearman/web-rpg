// Controllers/PlayersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Data;
using MerchantRPG.API.Models;

namespace MerchantRPG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameMapsController : ControllerBase
    {
        private readonly GameDbContext _context;

        public GameMapsController(GameDbContext context)
        {
            _context = context;
        }

        // GET: api/maps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMaps()
        {
            var maps = await _context.GameMaps
                .Select(m => new
                {
                    m.Id,
                    m.Duration,
                    m.Name,
                    m.Difficulty,
                    m.RequiredLevel,
                    m.RewardRanges,
                    m.UnlocksMap,
                })
                .ToListAsync();

            return Ok(maps);
        }
    }
}