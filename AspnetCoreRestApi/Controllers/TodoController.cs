using AspnetCoreRestApi.Data;
using AspnetCoreRestApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _apidDbContext;

        public TodoController(ApiDbContext apiDbContext)
        {
            _apidDbContext = apiDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            var items = await _apidDbContext.Items.ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemData data)
        {
            if (ModelState.IsValid)
            {
                await _apidDbContext.Items.AddAsync(data);
                await _apidDbContext.SaveChangesAsync();

                return CreatedAtAction("GetItem", new { data.Id}, data);
            }

            return new JsonResult("something went wrong");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var item = await _apidDbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemData item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            var existItem = await _apidDbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (existItem == null)
            {
                return NotFound();
            }

            UpdateItemData(item, existItem);

            await _apidDbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> UpdateItem(int id)
        {
            var existItem = await _apidDbContext.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (existItem == null)
            {
                return NotFound();
            }

            _apidDbContext.Items.Remove(existItem);

            await _apidDbContext.SaveChangesAsync();

            return NoContent();
        }

        private static void UpdateItemData(ItemData item, ItemData existItem)
        {
            existItem.Title = item.Title;
            existItem.Description = item.Description;
            existItem.Done = item.Done;
        }
    }
}
