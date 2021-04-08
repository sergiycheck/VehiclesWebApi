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

namespace Vehicles.MyCustomMapper
{  
    public interface ICustomMapper
    {
        public Task<CarResponse> CarToCarResponse(Car car,string ImgDirectory);
        public OwnerResponce OwnerToOwnerResponse(CustomUser carOwnerUser);
        public Car CarRequestToCar(CarRequest carRequest);
        public CustomUser OwnerRequestToCarOwner(OwnerRequest OwnerRequest);
        
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
        public async Task<CarResponse> CarToCarResponse(Car car,string ImgDirectory)
        {
            FileContentResult ImgFile = null;
            try
            {
                var FileImgInfo = await _vehicleImageRetriever
                    .GetImageByBrandAndUniqueNumber(
                    car.Brand, car.UniqueNumber, ImgDirectory);
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
                Price = car.Price,
                CarEngine = car.CarEngine,
                Description = car.Description,
                Transmision = car.Transmision,
                Drive = car.Drive,
                ImgFile = ImgFile,
                OwnerResponces =car.ManyToManyCustomUserToVehicle!=null? car.ManyToManyCustomUserToVehicle.Select(o => 
                            new OwnerResponce(){
                                Id = o.CarOwnerId,
                                Name = o.CarOwner.FirstName,
                                SurName = o.CarOwner.LastName,
                                CarOwnerPhone = o.CarOwner.PhoneNumber,
                                Location = o.CarOwner.Address,
                                BirthDate = o.CarOwner.BirthDate,
                                CarResponces = null
                            }).ToList():null
            };
        }
        public OwnerResponce OwnerToOwnerResponse(CustomUser owner)
        {
            return new OwnerResponce
            {
                Id = owner.Id,
                Name = owner.FirstName,
                SurName = owner.LastName,
                CarOwnerPhone = owner.PhoneNumber,
                Location = $"{owner.Address} {owner.City}",
                BirthDate = owner.BirthDate,

                CarResponces = owner.ManyToManyCustomUserToVehicle!=null?owner.ManyToManyCustomUserToVehicle.Select(car=>
                    new CarResponse(){
                        Id = car.Car.Id,
                        UniqueNumber = car.Car.UniqueNumber,
                        Brand = car.Car.Brand,
                        Color = car.Car.Color,
                        Date = car.Car.Date,
                        Price = car.Car.Price,
                        CarEngine = car.Car.CarEngine,
                        Description = car.Car.Description,
                        Transmision = car.Car.Transmision,
                        Drive = car.Car.Drive,
                        OwnerResponces = null
                    }).ToList():null   
            };
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
                Price = carRequest.Price,
                CarEngine = carRequest.CarEngine,
                Description = carRequest.Description,
                Transmision = carRequest.Transmision,
                Drive = carRequest.Drive,
                ManyToManyCustomUserToVehicle =carRequest.OwnerRequests!=null? carRequest.OwnerRequests.Select(o => 
                            new ManyToManyCustomUserToVehicle(){
                                CarId = carRequest.Id,
                                CarOwnerId = o.Id
                            }).ToList():null
            };
        }
        public CustomUser OwnerRequestToCarOwner(OwnerRequest OwnerRequest)
        {
            return new CustomUser
            {
                Id = OwnerRequest.Id,
                FirstName = OwnerRequest.Name,
                LastName = OwnerRequest.SurName,
                PhoneNumber = OwnerRequest.CarOwnerPhone,
                Address = OwnerRequest.Location,
                BirthDate = OwnerRequest.BirthDate,
                ManyToManyCustomUserToVehicle = OwnerRequest.CarRequests!=null?OwnerRequest.CarRequests.Select(car=>
                    new ManyToManyCustomUserToVehicle(){
                        CarId = car.Id,
                        CarOwnerId = OwnerRequest.Id
                    }).ToList():null   
            };
        }
    }  
}