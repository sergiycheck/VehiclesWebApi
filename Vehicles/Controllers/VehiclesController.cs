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
    public class VehiclesController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(ICarService carService,ILogger<VehiclesController> logger)
        {
            _carService = carService;
            _logger = logger;
        }


        // GET api/<VehiclesController>/
        [HttpGet]
        public ActionResult<string> DefaultGet() 
        {
            return Content("An API about vehicles and owners");
        }
        // GET api/<VehiclesController>/cars
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars()
        {
            var result = await _carService.GetAllCars();
            return Ok(result);
        }
        //Get api/vehicles/car/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Car>> GetCarById(int? id)
        {
            return Ok(await _carService.GetById(id));
        }

        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners
        // POST api/<VehiclesController>/get_cars_by_owner
        [HttpPost("get_cars_by_owner")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarsByCarOwner([FromBody] CarOwner value)
        {
            var res = await _carService.GetCars(value);
            _logger.LogInformation($"Getting cars by carOwner {value.Name}", res);
            return res;
        }

        [HttpPost("add")]
        public async Task<ActionResult<Car>> PostCarItem([FromBody] Car car)
        {
            await _carService.Create(car);//changes entity state to added and execute save changes that produces insert command
            return CreatedAtAction(nameof(PostCarItem), new { brand = car.Brand }, car);
        }


        // PUT api/<VehiclesController>/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Put(int? id, [FromBody] Car car)
        {
            if (id != car.Id)
                return BadRequest();
            await _carService.Update(car);
            return Content($"Car with id {car.Id} and unique number {car.UniqueNumber} successfully updated");
        }

        // DELETE api/<VehiclesController>/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null) 
            {
                if (_carService.EntityExists((int)id))
                {
                    await _carService.Delete(id);
                    return Content($"Car with id {id} was successfully deleted");
                }
            }
            return BadRequest();
            
        }
    }
}
//command to generate controller
//dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers