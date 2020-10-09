using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vehicles.Interfaces.ServiceInterfaces;
using Vehicles.Models;
using Microsoft.AspNetCore.Cors;

namespace Vehicles.Controllers
{
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    [Route("api/[controller]")]
    [ApiController]
    public class CarOwnersController : ControllerBase
    {
        private readonly ICarOwnerService _carOwnerService;
        private readonly ILogger<VehiclesController> _logger;

        public CarOwnersController(ICarOwnerService carOwnerService,ILogger<VehiclesController> logger)
        {
            _carOwnerService = carOwnerService;
            _logger = logger;
        }

        // GET api/CarOwners/QC-3805-OM
        [HttpGet("{uniqueNumber}")]
        public async Task<ActionResult<IEnumerable<CarOwner>>> GetOwnersByCarUniqueNumber(string uniqueNumber)
        {
            var res = await _carOwnerService.GetCarOwners(uniqueNumber);
            _logger.LogInformation($"Getting car owners by uniqueNumber {uniqueNumber}",res);
            return res;
        }

        [HttpGet]
        public ActionResult<string> DefaultGet() 
        {
            return "An API about vehicles and owners";
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<CarOwner>>> GetOwners()
        {
            return await _carOwnerService.GetAllCarOwners();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CarOwner>> GetOwnerById(int? id)
        {
            return Ok(await _carOwnerService.GetById(id));
        }
        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners


        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

       
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
//command to generate controller
//dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers