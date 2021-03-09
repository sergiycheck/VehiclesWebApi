
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vehicles.Contracts.V1.Requests;
using vehicles.Hubs;
using Vehicles;
using Vehicles.Contracts.V1;

namespace vehicles.Controllers
{
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    [ApiController]
    public class ChatController: ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<ChatController> _logger;
        public ChatController(
            IHubContext<ChatHub> hubContext,
            ILogger<ChatController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost(ApiRoutes.ChatRoutes.Send)]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            _logger.LogInformation($"message received {messageDto.Data} {messageDto.User}");
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageDto.User, messageDto.Data);
            return Ok($"Data {messageDto.Data} from {messageDto.User} was send");
        }
    }
}
