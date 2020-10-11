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

namespace Vehicles.Controllers
{
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    //[Route("api/[controller]")]
    [ApiController]
    public class CarOwnersController : ControllerBase
    {
        private readonly ICarOwnerService _carOwnerService;
        private readonly ILogger<VehiclesController> _logger;
        private readonly IUriService _uriService;
        private readonly ICustomMapper _customMapper;               
        public CarOwnersController(
            ICarOwnerService carOwnerService,
            ILogger<VehiclesController> logger,
            IUriService uriService,
            ICustomMapper customMapper)
        {
            _carOwnerService = carOwnerService;
            _logger = logger;
            _uriService = uriService;
            _customMapper = customMapper;
        }

        [HttpGet(ApiRoutes.Owners.GetOwnersByCarUniqueNumber)]
        public async Task<ActionResult> GetOwnersByCarUniqueNumber(string uniqueNumber)
        {
            var res = await _carOwnerService.GetCarOwners(uniqueNumber);
            _logger.LogInformation($"Getting car owners by uniqueNumber {uniqueNumber}",res);
            var ownersResponces = new List<OwnerResponce>();
            res.ForEach(o=>ownersResponces.Add(_customMapper.OwnerToOwnerResponse(o)));
            return Ok(new Response<OwnerResponce[]>(ownersResponces.ToArray()));
        }

        [HttpGet(ApiRoutes.Owners.Default)]
        public ActionResult<string> DefaultGet() 
        {
            return Ok(new Response<string>("An API about vehicles and owners"));
        }

        [HttpGet(ApiRoutes.Owners.GetAll)]
        public async Task<ActionResult<IEnumerable<OwnerResponce>>> GetOwners()
        {
            var owners = await _carOwnerService.GetAllCarOwners();
            var ownersResponces = new List<OwnerResponce>();
            owners.ForEach(o=>ownersResponces.Add(_customMapper.OwnerToOwnerResponse(o)));
            return Ok(new Response<OwnerResponce[]>(ownersResponces.ToArray()));

        }

        [HttpGet(ApiRoutes.Owners.Get)]
        public async Task<ActionResult<OwnerResponce>> GetOwnerById(int? id)
        {
            return Ok(
                new Response<OwnerResponce>(
                    _customMapper.OwnerToOwnerResponse(
                        await _carOwnerService.GetById(id))));
        }
        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners

        [HttpPost(ApiRoutes.Owners.Create)]
        public IActionResult Create([FromBody] CarOwner owner)
        {
            return Ok();
        }

        [HttpPut(ApiRoutes.Owners.Update)]
        public void Put(int id, [FromBody] string value)
        {
        }

       
        [HttpDelete(ApiRoutes.Owners.Delete)]
        public void Delete(int id)
        {
        }
    }
}
//command to generate controller
//dotnet aspnet-codegenerator controller -name TodoItemsController -async -api -m TodoItem -dc TodoContext -outDir Controllers