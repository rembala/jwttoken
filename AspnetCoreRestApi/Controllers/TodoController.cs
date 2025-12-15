using AspnetCoreRestApi.Configurations;
using AspnetCoreRestApi.Configurations.Interfaces;
using AspnetCoreRestApi.Data;
using AspnetCoreRestApi.Helpers.Interfaces;
using AspnetCoreRestApi.Models;
using AspnetCoreRestApi.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AspnetCoreRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "appUser")]
    public class TodoController : ControllerBase
    {
        private readonly ApiDbContext _apidDbContext;

        private readonly ICorellationIdGenerator _corellationIdGenerator;
        private readonly ILogger<TodoController> _todoController;
        private readonly ITodoNotificationPublisherService _todoNotificationPublisherService;

        public TodoController(ApiDbContext apiDbContext,
            ICorellationIdGenerator corellationIdGenerator,
            ILogger<TodoController> todoController,
            ITodoNotificationPublisherService todoNotificationPublisherService)
        {
            _apidDbContext = apiDbContext;
            _corellationIdGenerator = corellationIdGenerator;
            _todoController = todoController;
            _todoNotificationPublisherService = todoNotificationPublisherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            _todoController.LogInformation("Getting all todo items. Correlation ID: {CorrelationId}", _corellationIdGenerator.Get());
            var items = await _apidDbContext.Items.ToListAsync();
            var jobId = BackgroundJob.Enqueue<IEmailService>(emailService => emailService.SendWelcomeEmail("maslovskij.artur@gmail.com", "Artur"));
            Console.WriteLine($"Hangfire Job ID: {jobId}");
            
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

        [HttpPost("SendTransitITem")]
        public async Task<IActionResult> SendTransitITem(ItemData data)
        {
            if (ModelState.IsValid)
            {
                await SendMassTransitItem(data);
            }

            return new JsonResult("something went wrong");
        }

        private async Task SendMassTransitItem(ItemData data)
        {
            await _todoNotificationPublisherService.SendNotification(data.Id, data.Title);
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
