using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vehicles.Interfaces.ServiceInterfaces;
using Vehicles.Models;

namespace Vehicles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly ICarOwnerService _carOwnerService;
        private readonly ICarService _carService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(ICarOwnerService carOwnerService,ICarService carService,ILogger<VehiclesController> logger)
        {
            _carOwnerService = carOwnerService;
            _carService = carService;
            _logger = logger;
        }

        // GET api/<VehiclesController>/QC-3805-OM
        [HttpGet("{uniqueNumber}")]
        public async Task<ActionResult<IEnumerable<CarOwner>>> GetOwnersByCarUniqueNumber(string uniqueNumber)
        {
            var res = await _carOwnerService.GetCarOwners(uniqueNumber);
            _logger.LogInformation($"Getting car owners by uniqueNumber {uniqueNumber}",res);
            return res;
        }

        // GET api/<VehiclesController>/
        [HttpGet]
        public ActionResult<string> DefaultGet() 
        {
            return "An API about vehicles and owners";
        }
        // GET api/<VehiclesController>/cars
        [HttpGet("Cars")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            return await _carService.GetAllCars();
        }
        // GET api/<VehiclesController>/owners
        [HttpGet("Owners")]
        public async Task<ActionResult<IEnumerable<CarOwner>>> GetOwners()
        {
            return await _carOwnerService.GetAllCarOwners();
        }

        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners
        // POST api/<VehiclesController>
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarsByCarOwner([FromBody] CarOwner value)
        {
            var res = await _carService.GetCars(value);
            _logger.LogInformation($"Getting cars by carOwner {value.Name}", res);
            return res;
        }

        // PUT api/<VehiclesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<VehiclesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
//command to generate controller
//dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers