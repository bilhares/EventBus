using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commom;
using EventBus;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MainService.Controllers
{
    [Route("main")]
    public class MainController : Controller
    {
        private readonly IEventBus _eventBus;

        public MainController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MainIntegrationEvent message)
        {
            _eventBus.Publish(message, nameof(MainIntegrationEvent));
            return Ok();
        }   
    }
}
