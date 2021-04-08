using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vehicles.Interfaces.ServiceInterfaces;
using Vehicles.Models;
using Microsoft.AspNetCore.Cors;
using Vehicles.Contracts.V1;
using Vehicles.Services;
using Vehicles.Contracts.V1.Responses;
using Vehicles.MyCustomMapper;
using Vehicles.Contracts.Responces;
using Vehicles.Contracts.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Hosting; // for IWebHostEnvironment
using System.IO;
using vehicles.Helpers;

namespace Vehicles.Controllers
{
    
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    // [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly ICarService _carService;
        private readonly ILogger<VehiclesController> _logger;
        private readonly IUriService _uriService;
        private readonly ICustomMapper _customMapper;
        private readonly IWebHostEnvironment _appEnvironment;

        private readonly IVehicleImageRetriever _vehicleImageRetriever;

        private readonly string _imgDirectory;

        public VehiclesController(
            ICarService carService,
            ILogger<VehiclesController> logger,
            IUriService uriService,
            ICustomMapper customMapper,
            IWebHostEnvironment appEnvironment,
            IVehicleImageRetriever vehicleImageRetriever
            )
        {
            _carService = carService;
            _logger = logger;
            _uriService = uriService;
            _customMapper = customMapper;
            _appEnvironment = appEnvironment;
            _vehicleImageRetriever = vehicleImageRetriever;

            var directory = Directory.GetCurrentDirectory();
            var imgDirectory = $@"{directory}\{ApiRoutes.imgsPath}";
            _imgDirectory = imgDirectory;
        }
        

        [HttpGet(ApiRoutes.Vehicles.Default)]
        public ActionResult<string> DefaultGet() 
        {
            return Content("An API about vehicles and owners");
        }
        
        [HttpGet(ApiRoutes.Vehicles.GetAll)]
        public async Task<ActionResult> GetCars()
        {
            var result = await _carService.GetAllCars();
            var carResponses = new List<CarResponse>();

            

            result.ForEach(async c=>carResponses.Add(
                await _customMapper.CarToCarResponse(c, _imgDirectory)));

            return Ok(new Response<CarResponse[]>(carResponses.ToArray()));
        }

        //[Authorize]
        [HttpGet(ApiRoutes.Vehicles.Get)]
        public async Task<ActionResult<Car>> GetCarById(int? id)
        {
            var car = await _carService.GetById(id);
            if(car==null)
                return NotFound();

            return Ok(new Response<CarResponse>(
                await _customMapper.CarToCarResponse(car, _imgDirectory)));
        }

        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners

        //TODO: uncomment
        //[Authorize]

        [HttpPost(ApiRoutes.Vehicles.GetCarsByOwner)]
        public async Task<ActionResult> GetCarsByCarOwner([FromBody] OwnerRequest value)
        {
            var res = await _carService.GetCars(_customMapper.OwnerRequestToCarOwner(value));
            _logger.LogInformation($"Getting cars by carOwner {value.Name}", res);
            var carsResponces = new List<CarResponse>();
            res.ForEach(async c=>carsResponces.Add(
                await _customMapper.CarToCarResponse(c, _imgDirectory)));

            return Ok(new Response<CarResponse[]>(carsResponces.ToArray()));
        }

        [Authorize]
        [HttpPost(ApiRoutes.Vehicles.Create)]
        public async Task<ActionResult> PostCarItem([FromBody] CarRequest carRequest)
        {
            var car = _customMapper.CarRequestToCar(carRequest);
            await _carService.Create(car);//changes entity state to added and execute save changes that produces insert command
            var locationUri = _uriService.GetVehicleUri(car.Id.ToString());
            return Created(locationUri, new Response<CarResponse>(
                await _customMapper.CarToCarResponse(car, _imgDirectory)));
        }


        [Authorize]
        [HttpPut(ApiRoutes.Vehicles.Update)]
        public async Task<IActionResult> Put(int? id, [FromBody] CarRequest carRequest)
        {
            if (id != carRequest.Id)
                return BadRequest();
            var car = _customMapper.CarRequestToCar(carRequest);
            var updatedNums = await _carService.Update(car);
            if(updatedNums>0)
                return Ok(new Response<string>($"Car with id {carRequest.Id} and unique number {carRequest.UniqueNumber} successfully updated"));
            return NoContent();
        }

        [Authorize]
        [HttpDelete(ApiRoutes.Vehicles.Delete)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null) 
            {
                if (_carService.EntityExists((int)id))
                {
                    await _carService.Delete(id);
                    return Ok(new Response<string>($"Car with id {id} was successfully deleted"));
                }
            }
            return BadRequest();
            
        }
        
        [HttpGet(ApiRoutes.Vehicles.GetImage)]
        public async Task<IActionResult> GetImage(string Brand, string UniqueNumber) 
        {
            if (string.IsNullOrEmpty(Brand) 
                && string.IsNullOrEmpty(UniqueNumber))
            {
                return BadRequest($"Can not get img with empty name {Brand}");
            }

            var directory = Directory.GetCurrentDirectory();
            var imgDirectory = $@"{directory}\{ApiRoutes.imgsPath}";

            try
            {
                var FileImgInfo = await _vehicleImageRetriever
                    .GetImageByBrandAndUniqueNumber(Brand, UniqueNumber, imgDirectory);
                if (FileImgInfo != null)
                {
                    return Ok(File(FileImgInfo.FileBytes, FileImgInfo.FileType));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return Ok(directory);

        }

    }
}
//command to generate controller
//dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers