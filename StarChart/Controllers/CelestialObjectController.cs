using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var found = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);

            if(found == null)
                return NotFound();
            found.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == id).ToList();

            return Ok(found);
        }


        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var list = _context.CelestialObjects.Where(c => c.Name == name).ToList();

            if (!list.Any())
                return NotFound();

            list.ForEach(c => c.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == c.Id).ToList());

            return Ok(list);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.CelestialObjects.ToList();

            list.ForEach(c => c.Satellites = _context.CelestialObjects.Where(s => s.OrbitedObjectId == c.Id).ToList());

            return Ok(list);
        }
    }
}
