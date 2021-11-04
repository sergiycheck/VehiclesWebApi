using AutoMapper;  
using Vehicles.Models;
using Vehicles.Contracts.Responces;
using System.Linq;
using Vehicles.Contracts.Requests;
using vehicles.Helpers;
using System.IO;
using Vehicles.Contracts.V1;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using vehicles.Contracts.V1.Responses;
using vehicles.Models;

namespace Vehicles.MyCustomMapper
{  
    public interface ICustomMapper
    {
        public CarResponse CarToCarResponse(Car car);
        public Car CarRequestToCar(CarRequest carRequest);
        public OwnerResponce OwnerToOwnerResponse(CustomUser carOwnerUser);
        public CustomUser OwnerRequestToCarOwner(OwnerRequest OwnerRequest);
        public PenaltyResponse PenaltyToPenaltyResponse(Penalty penalty);
        public Penalty PenaltyRequestToPenalty(PenaltyRequest penalty,int carId);
        void UpdateUserFromDb(CustomUser userFromDb,CustomUser userFromRequest);


    }
    public class CustomMapper :ICustomMapper
    {
        private readonly IVehicleImageRetriever _vehicleImageRetriever;

        public CustomMapper(
            IVehicleImageRetriever vehicleImageRetriever
            )
        {
            _vehicleImageRetriever = vehicleImageRetriever;
        }
        public CarResponse CarToCarResponse(Car car)
        {
            FileContentResult ImgFile = null;
            try
            {
                Console.WriteLine($"{car.Brand} \n {car.ImgPath}");

                var FileImgInfo =  _vehicleImageRetriever
                    .GetFileImgInfoByImgPath(car.ImgPath);
                ImgFile = new FileContentResult(
                    FileImgInfo.FileBytes, FileImgInfo.FileType);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }

            return new CarResponse
            {
                Id = car.Id,
                UniqueNumber = car.UniqueNumber,
                Brand = car.Brand,
                Color = car.Color,
                Date = car.Date,
                ForSale = car.ForSale,
                Price = car.Price,
                CarEngine = car.CarEngine,
                Description = car.Description,
                Transmision = car.Transmision,
                Drive = car.Drive,
                ImgFile = ImgFile,
                OwnerResponces =car.ManyToManyCustomUserToVehicle?.Select(o => 
                            new OwnerResponce(){
                                Id = o.CarOwnerId,
                                Name = o.CarOwner?.FirstName,
                                SurName = o.CarOwner?.LastName,
                                CarOwnerPhone = o.CarOwner?.PhoneNumber,
                                Location = o.CarOwner?.Address,
                                BirthDate = o.CarOwner!=null?o.CarOwner.BirthDate:DateTime.Now,
                                CarResponces = null
                            }).ToList()
            };
        }

        public PenaltyResponse PenaltyToPenaltyResponse(Penalty penalty)
        {
            return new PenaltyResponse()
            {
                Id = penalty.Id,
                PayedStatus = penalty.PayedStatus,
                CarUniqueNumber = penalty.CarUniqueNumber,
                Date = penalty.Date,
                Description = penalty.Description,
                Location = penalty.Location,
                Price = penalty.Price
            };
        }
        public Penalty PenaltyRequestToPenalty(PenaltyRequest penalty,int carId)
        {
            return new Penalty()
            {
                Id = penalty.Id,
                PayedStatus = penalty.PayedStatus,
                CarId = carId,
                Date = penalty.Date,
                Description = penalty.Description,
                Location = penalty.Location,
                Price = penalty.Price,
                CarUniqueNumber = penalty.CarUniqueNumber
            };
        }


        public OwnerResponce OwnerToOwnerResponse(CustomUser owner)
        {
            if (owner != null)
            {
                return new OwnerResponce
                {
                    Id = owner.Id,
                    Email = owner.Email,
                    Name = owner.FirstName,
                    UserName = owner.UserName,
                    SurName = owner.LastName,
                    CarOwnerPhone = owner.PhoneNumber,
                    Location = owner.Address,
                    BirthDate = owner.BirthDate,

                    CarResponces = owner.ManyToManyCustomUserToVehicle?.Select(car =>
                        new CarResponse()
                        {
                            Id = car.Car != null ? car.Car.Id : 0,
                            UniqueNumber = car.Car?.UniqueNumber,
                            Brand = car.Car?.Brand,
                            Color = car.Car?.Color,
                            Date = car.Car != null ? car.Car.Date : DateTime.Now,
                            Price = car.Car != null ? car.Car.Price : 0,
                            CarEngine = car.Car != null ? car.Car.CarEngine : 0,
                            Description = car.Car?.Description,
                            Transmision = car.Car?.Transmision,
                            Drive = car.Car?.Drive,
                            OwnerResponces = null
                        }).ToList()
                };
            }
            return null;

        }
        public Car CarRequestToCar(CarRequest carRequest)
        {
            return new Car
            {
                Id = carRequest.Id,
                UniqueNumber = carRequest.UniqueNumber,
                Brand = carRequest.Brand,
                Color = carRequest.Color,
                Date = carRequest.Date,
                ForSale = carRequest.ForSale,
                Price = carRequest.Price,
                CarEngine = carRequest.CarEngine,
                Description = carRequest.Description,
                Transmision = carRequest.Transmision,
                Drive = carRequest.Drive,
                ManyToManyCustomUserToVehicle =carRequest.OwnerRequests?.Select(o => 
                            new ManyToManyCustomUserToVehicle(){
                                CarId = carRequest.Id,
                                CarOwnerId = o.Id
                            }).ToList()
            };
        }
        public CustomUser OwnerRequestToCarOwner(OwnerRequest OwnerRequest)
        {
            return new CustomUser
            {
                Id = OwnerRequest.Id,
                Email = OwnerRequest.Email,
                FirstName = OwnerRequest.Name,
                UserName = OwnerRequest.UserName,
                LastName = OwnerRequest.SurName,
                PhoneNumber = OwnerRequest.CarOwnerPhone,
                Address = OwnerRequest.Location,
                BirthDate = OwnerRequest.BirthDate,
                ManyToManyCustomUserToVehicle = OwnerRequest.CarRequests?.Select(car=>
                    new ManyToManyCustomUserToVehicle(){
                        CarId = car.Id,
                        CarOwnerId = OwnerRequest.Id
                    }).ToList()   
            };
        }
        public void UpdateUserFromDb(CustomUser userFromDb,CustomUser userFromRequest){
            userFromDb.FirstName = userFromRequest.FirstName;
            userFromDb.UserName = userFromRequest.UserName;
            userFromDb.Email = userFromRequest.Email;
            userFromDb.LastName = userFromRequest.LastName;
            userFromDb.Address = userFromRequest.Address;
            userFromDb.BirthDate = userFromRequest.BirthDate;
            userFromDb.PhoneNumber = userFromRequest.PhoneNumber;


        }

    }  
}