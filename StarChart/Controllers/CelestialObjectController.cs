using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

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

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject obj)
        {
            var found = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (found == null)
                return NotFound();

            found.Name = obj.Name;
            found.OrbitalPeriod = obj.OrbitalPeriod;
            found.OrbitedObjectId = obj.OrbitedObjectId;
            _context.CelestialObjects.Update(found);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var found = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (found == null)
                return NotFound();

            found.Name = name;
            _context.CelestialObjects.Update(found);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var list = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();

            if (!list.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(list);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
