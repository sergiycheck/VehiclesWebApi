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
using Microsoft.AspNetCore.Http;
using vehicles.Authorization.AuthorizationsManagers;
using vehicles.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;
using vehicles.Contracts.V1.Requests;

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

        ICustomAuthorizationService _customAuthorizationService;

        public VehiclesController(
            ICarService carService,
            ILogger<VehiclesController> logger,
            IUriService uriService,
            ICustomMapper customMapper,
            IWebHostEnvironment appEnvironment,
            IVehicleImageRetriever vehicleImageRetriever,
            ICustomAuthorizationService authorizationService
            )
        {
            _carService = carService;
            _logger = logger;
            _uriService = uriService;
            _customMapper = customMapper;
            _appEnvironment = appEnvironment;
            _vehicleImageRetriever = vehicleImageRetriever;

            _customAuthorizationService = authorizationService;

            // var directory = Directory.GetCurrentDirectory();
            var directory = _appEnvironment.WebRootPath;

            _logger.LogInformation($"current directory {directory}");

            var imgDirectory = $@"{directory}/{ApiRoutes.imgsPath}";
            _imgDirectory = imgDirectory;

            _logger.LogInformation($"VehiclesController _imgDirectory \n {_imgDirectory}");
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

            _logger.LogInformation($"list Car length is {result.Count}");

            var carResponses = new List<CarResponse>();

            result.ForEach( c=>carResponses.Add(
                    _customMapper.CarToCarResponse(c)));

            var responsesArray = carResponses.ToArray();

            _logger.LogInformation($"responsesArray length is {responsesArray.Length}");

            return Ok(new Response<CarResponse[]>(responsesArray));
        }

        //[Authorize]
        [HttpGet(ApiRoutes.Vehicles.Get)]
        public async Task<ActionResult<Car>> GetCarById(int? id)
        {
            var car = await _carService.GetById(id);
            if(car==null)
                return NotFound();

            return Ok(new Response<CarResponse>(
                 _customMapper.CarToCarResponse(car)));
        }

        //use postman post method or visual studio code extensions to send post method with existing carOwner json data that can be retrieved from get method for CarOwners

        //TODO: uncomment
        //[Authorize]

        [HttpPost(ApiRoutes.Vehicles.GetCarsByOwner)]
        public async Task<ActionResult> GetCarsByCarOwner([FromBody] OwnerRequest value)
        {
            var res = await _carService.GetCars(
                _customMapper.OwnerRequestToCarOwner(value));

            _logger.LogInformation($"Getting cars by carOwner {value.Name}", res);
            var carsResponces = new List<CarResponse>();
            res.ForEach( c=>carsResponces.Add(
                 _customMapper.CarToCarResponse(c)));

            return Ok(new Response<CarResponse[]>(carsResponces.ToArray()));
        }

        [Authorize]
        [HttpPost(ApiRoutes.Vehicles.Create), DisableRequestSizeLimit]
        public async Task<ActionResult> PostCarItem([FromForm] CarRequest carRequest)
        {

            var car = _customMapper.CarRequestToCar(carRequest);

            car.ImgPath = await CreateImgFromRequest(Request, carRequest);

            await _carService.Create(car);//changes entity state to added and execute save changes that produces insert command
            var locationUri = _uriService.GetVehicleUri(car.Id.ToString());

            
            return Created(locationUri, new Response<CarResponse>(
                 _customMapper.CarToCarResponse(car)));
        }

        private async Task<string> CreateImgFromRequest(HttpRequest request, CarRequest carRequest)
        {
            if (request == null)
            {
                return string.Empty;
            }

            var formCollection = await request.ReadFormAsync();
            var ImgFile = formCollection.Files.First();

            if (ImgFile != null && ImgFile.Length > 0)
            {
                var createdImgPathResult = await _vehicleImageRetriever
                        .CreateImgByBrandAndUniqueNumber(
                        ImgFile,
                        carRequest.Brand,
                        carRequest.UniqueNumber,
                        _imgDirectory);

                if (createdImgPathResult != string.Empty)
                {
                    
                    _logger.LogInformation($"{ImgFile.FileName} created successfully");
                    return createdImgPathResult;
                }
                else
                {
                    _logger.LogInformation($"{ImgFile.FileName} not created");

                }
            }
            return string.Empty;
        }


        private async Task<bool> CheckIfUserAuthorizedForOperation(
                            ClaimsPrincipal principalUser,
                            OwnerResource resource,
                            OperationAuthorizationRequirement requirement)
        {
            var isAuthorized = await _customAuthorizationService
                                        .AuthorizeAsync(principalUser, resource, requirement);
            if (!isAuthorized.Succeeded)
            {
                return false;
            }
            return true;

        }




        [HttpPost(ApiRoutes.Vehicles.CanAccess)]
        public async Task<IActionResult> CanUserAccess([FromBody] CanAccessRequest canAccessRequest)
        {
            if(canAccessRequest==null || 
                canAccessRequest.Token==null || 
                canAccessRequest.Id== null ||  
                canAccessRequest.Token==string.Empty)
            {
                return Ok(false);
            }
            if (!await CheckIsUserAuthorizedForAction((int)canAccessRequest.Id, canAccessRequest.Token))
            {
                return Ok(false);
            }
            return Ok(true);
        }



        private async Task<bool> CheckIsUserAuthorizedForAction(int? carId, string token)
        {
            //todo:
            //refactor get owner by car id
            if (carId == null || token == null || token ==string.Empty)
            {
                return false;
            }

            var actualOwners = await _carService.GetOwnersByCar((int)carId);
            if (actualOwners == null)
            {
                return false;
            }
            var ownersId = actualOwners.Select(o => o.Id);

            //get token and then get claims principal from token and set is to user
            var claimsPrincipalCurrentUser = _carService.GetClaimsPrincipal(token);
            if (claimsPrincipalCurrentUser != null)
            {
                HttpContext.User = claimsPrincipalCurrentUser;
            }

            if (!await CheckIfUserAuthorizedForOperation(
                    HttpContext.User,
                    new OwnerResource() { OwnersId = ownersId },
                    Operations.Update))
            {
                return false;
            }
            return true;
        }

        [Authorize]
        [HttpPut(ApiRoutes.Vehicles.Update), DisableRequestSizeLimit]
        public async Task<IActionResult> Put(int? id, [FromForm] CarRequest carRequest)
        {
            if (id != carRequest.Id)
                return BadRequest();

            //TODO:
            //test

            if(!await CheckIsUserAuthorizedForAction(carRequest.Id, carRequest.Token))
            {
                return BadRequest("You have not enough permissions for this action");
            }
            
            var car = _customMapper.CarRequestToCar(carRequest);
            var oldCar = await _carService.GetById(id);
            _vehicleImageRetriever.DeleteFile(oldCar.ImgPath);

            car.ImgPath = await CreateImgFromRequest(Request, carRequest);


            var updatedNums = await _carService.Update(car);
            if (updatedNums > 0)
            {
                return Ok(new Response<string>
                    ($"Car with id {carRequest.Id} and unique number " +
                        $"{carRequest.UniqueNumber} successfully updated"));
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost(ApiRoutes.Vehicles.Delete)]
        public async Task<IActionResult> Delete(int? id, [FromBody] CanAccessRequest canAccessRequest)
        {
            if (id != null) 
            {
                //TODO:
                //test

                if (!await CheckIsUserAuthorizedForAction(canAccessRequest.Id, canAccessRequest.Token))
                {
                    return BadRequest("You have not enough permissions for this action");
                }
               

                if (_carService.EntityExists((int)id))
                {
                    var car = await _carService.GetById(id);
                    _vehicleImageRetriever.DeleteFile(car.ImgPath);

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