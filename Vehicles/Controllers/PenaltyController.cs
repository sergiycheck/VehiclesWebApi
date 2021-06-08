using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemTextJsonSamples;
using vehicles.Authorization.AuthorizationsManagers;
using vehicles.Contracts.V1.Responses;
using vehicles.Repositories;
using Vehicles;
using Vehicles.Contracts.V1;
using Vehicles.Contracts.V1.Responses;
using Vehicles.Models;
using Vehicles.MyCustomMapper;
using System.Text.Json.Serialization;
using Vehicles.Interfaces.ServiceInterfaces;
using System.Security.Claims;
using vehicles.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using vehicles.Contracts.V1.Requests;

namespace vehicles.Controllers
{
    [EnableCors(Startup.MyAllowSpecificOrigins)]
    [ApiController]
    public class PenaltyController : ControllerBase
    {
        private readonly IPenaltyRepository _penaltyRepository;
        private readonly ILogger<PenaltyController> _logger;
        private readonly ICustomAuthorizationService _customAuthorizationService;
        private readonly ICustomMapper _customMapper;
        private readonly ICarService _carService;

        public PenaltyController(
            IPenaltyRepository penaltyRepository,
            ILogger<PenaltyController> logger,
            ICustomAuthorizationService customAuthorizationService,
            ICustomMapper customMapper,
            ICarService carService
            )
        {
            _penaltyRepository = penaltyRepository;
            _logger = logger;
            _customAuthorizationService = customAuthorizationService;
            _customMapper = customMapper;
            _carService = carService;
        }


        //tested with postman
        [HttpGet(ApiRoutes.PenaltiesRoutes.GetAll)]
        public async Task<ActionResult> GetPenalties()
        {
            var penalties = await _penaltyRepository.GetAll().ToListAsync();
            _logger.LogInformation($"all penalties {penalties.Count}");
            var penaltyResponses = new List<PenaltyResponse>();

            penalties
                .ForEach(p => penaltyResponses
                    .Add(_customMapper
                        .PenaltyToPenaltyResponse(p)));

            var responsesArray = penaltyResponses.ToArray();
            _logger.LogInformation($"responsesArray length is {responsesArray.Length}");

            return Ok(new Response<PenaltyResponse[]>(responsesArray));
        }

        [Authorize]
        [HttpGet(ApiRoutes.PenaltiesRoutes.GetPenaltiesByUserId)]
        public async Task<ActionResult> GetPenaltiesByUser(string userId)
        {
            if(userId == null)
            {
                return BadRequest("empty user id");

            }

            var penalties = await _penaltyRepository.GetPenaltiesByUserId(userId);

            _logger
                .LogInformation($" penalties " +
                $"by user id  {userId}  {penalties.Count}");

            var penaltyResponses = new List<PenaltyResponse>();

            penalties
                .ForEach(p => penaltyResponses
                    .Add(_customMapper
                        .PenaltyToPenaltyResponse(p)));

            var responsesArray = penaltyResponses.ToArray();
            _logger.LogInformation($"responsesArray length is {responsesArray.Length}");

            return Ok(new Response<PenaltyResponse[]>(responsesArray));


        }


        //tested with postman
        [HttpGet(ApiRoutes.PenaltiesRoutes.GetPenaltiesByUniqueNumber)]
        public async Task<ActionResult> GetPenaltiesByUniqueNumber(string uniqueNumber)
        {

            if(uniqueNumber == string.Empty)
            {
                return BadRequest("no data for unique number");
            }

            var penalties = await _penaltyRepository
                .GetPenaltiesFromCarUniqueNumber(uniqueNumber);

            _logger
                .LogInformation($" penalties " +
                $"by unique number {uniqueNumber}  {penalties.Count}");

            var penaltyResponses = new List<PenaltyResponse>();

            penalties
                .ForEach(p => penaltyResponses
                    .Add(_customMapper
                        .PenaltyToPenaltyResponse(p)));

            var responsesArray = penaltyResponses.ToArray();
            _logger.LogInformation($"responsesArray length is {responsesArray.Length}");

            return Ok(new Response<PenaltyResponse[]>(responsesArray));
        }


        //tested with postman
        [HttpGet(ApiRoutes.PenaltiesRoutes.Get)]
        public async Task<ActionResult<PenaltyResponse>> GetPenaltyById(int? id)
        {
            if(id == null)
            {
                return BadRequest("empty id");
            }

            var penalty = await _penaltyRepository.GetById(id);
            if (penalty == null)
                return NotFound();

            return Ok(new Response<PenaltyResponse>(
                 _customMapper.PenaltyToPenaltyResponse(penalty)));
        }


        //tested with postman
        [HttpPost(ApiRoutes.PenaltiesRoutes.PayPenalty)]
        public async Task<IActionResult> 
            PayPenalty([FromBody] PenaltyPayRequest penaltyPayRequest)
        {
            if (penaltyPayRequest.Id == null)
            {
                return BadRequest("empty penaltyPayRequest id");
            }
            

            var penalty = await _penaltyRepository.GetById(penaltyPayRequest.Id);
            if (penalty == null)
                return NotFound();

            if (penalty.Price > penaltyPayRequest.Fee)
            {

                return BadRequest(
                    new Response<AuthFailedResponse>(
                        new AuthFailedResponse()
                        { Errors = new string[] { "not enough money to pay penalty" } }
                        )
                    );
            }

            var updatedResult = await _penaltyRepository.PayPenalty(penalty.Id);


            if (updatedResult > 0)
            {
                var updatedPenalty =await _penaltyRepository
                    .GetPenaltyWithCar((int)penaltyPayRequest.Id);

                var response = $"penalty with id {penalty.Id} payed ";

                if (updatedPenalty!=null && updatedPenalty.Car!=null)
                {
                    response = $"penalty with id {updatedPenalty.Id} " +
                        $"for car {updatedPenalty.Car.UniqueNumber} payed";
                }
                return Ok(new Response<string>
                    (response));
            }
            return NoContent();
        }

        //tested with postman
        [Authorize]
        [HttpPost(ApiRoutes.PenaltiesRoutes.Create), DisableRequestSizeLimit]
        public async Task<ActionResult> 
            PostPenalty([FromForm] PenaltyRequest penaltyRequest)
        {
            if (!await CheckIsUserAuthorizedForAction
                (penaltyRequest.Id, penaltyRequest.Token))
            {
                return BadRequest("You have not enough permissions for this action");
            }
            if(penaltyRequest.CarUniqueNumber == null)
            {
                return BadRequest("Cannot create penalty for empty CarId");
            }
            var car = await _carService.GetByUniqueNumber(penaltyRequest.CarUniqueNumber);
            if(car == null)
            {
                return BadRequest($"The car with id {penaltyRequest.CarUniqueNumber} not exist");
            }

            var penalty = _customMapper
                .PenaltyRequestToPenalty(penaltyRequest,car.Id);

            penalty.Id = 0;
            await _penaltyRepository.Create(penalty);
            await _penaltyRepository.SaveChangesAsync();

            return Created(
                ApiRoutes.PenaltiesRoutes.Create,
                new Response<PenaltyResponse>(
                    _customMapper.PenaltyToPenaltyResponse(penalty)));
        }

        //tested with postman
        [Authorize]
        [HttpPost
            (ApiRoutes.PenaltiesRoutes.Update), 
            DisableRequestSizeLimit]
        public async Task<IActionResult> 
            Put(int? id, [FromForm] PenaltyRequest penaltyRequest)
        {

            if (id != penaltyRequest.Id)
                return BadRequest();

            if (!await CheckIsUserAuthorizedForAction
                    (penaltyRequest.Id, penaltyRequest.Token))
            {
                return BadRequest("You have not enough permissions for this action");
            }


            if (penaltyRequest.CarUniqueNumber == null)
            {
                return BadRequest("Cannot create penalty for empty CarId");
            }
            var car = await _carService.GetByUniqueNumber(penaltyRequest.CarUniqueNumber);
            if (car == null)
            {
                return BadRequest($"The car with id {penaltyRequest.CarUniqueNumber} not exist");
            }


            var penalty = _customMapper
                .PenaltyRequestToPenalty(penaltyRequest,car.Id);

            _penaltyRepository.Update(penalty);
            var updatedNums = await _penaltyRepository.SaveChangesAsync();

            if (updatedNums > 0)
            {
                return Ok(new Response<string>
                    ($"Penalty with id {penalty.Id} successfully updated"));
            }

            return NoContent();
        }


        //tested with postman
        [Authorize]
        [HttpPost(ApiRoutes.PenaltiesRoutes.Delete)]
        public async Task<IActionResult> 
            Delete(int? id, [FromBody] CanAccessRequest canAccessRequest)
        {
            if (id != canAccessRequest.Id)
                return BadRequest();

            if (!await CheckIsUserAuthorizedForAction
                    (canAccessRequest.Id, canAccessRequest.Token))
            {
                return BadRequest("You have not enough permissions for this action");
            }


            await _penaltyRepository.Delete(id);
            var updatedNums = await _penaltyRepository.SaveChangesAsync();

            if (updatedNums > 0)
            {
                return Ok(new Response<string>
                    ($"Penalty with id {id} successfully deleted"));
            }

            return NoContent();
        }





        private async Task<bool> 
            CheckIsUserAuthorizedForAction(int? penaltyId, string token)
        {

            if (
                token == null || 
                token == string.Empty)
            {
                return false;
            }
            var claimsPrincipalCurrentUser = 
                _carService.GetClaimsPrincipal(token);
            if (claimsPrincipalCurrentUser != null)
            {
                HttpContext.User = claimsPrincipalCurrentUser;
            }
            if (!await CheckIfUserAuthorizedForOperation(
                HttpContext.User,
                new OwnerResource() { OwnersId = new string[] { } },
                Operations.Create))
            {
                return false;
            }
            return true;

        }



        private async Task<bool> CheckIfUserAuthorizedForOperation(
                    ClaimsPrincipal principalUser,
                    OwnerResource resource,
                    OperationAuthorizationRequirement requirement)
        {
            var isAuthorized = await 
                _customAuthorizationService
                    .AuthorizeAsync(principalUser, resource, requirement);

            if (!isAuthorized.Succeeded)
            {
                return false;
            }
            return true;

        }

    }
}
