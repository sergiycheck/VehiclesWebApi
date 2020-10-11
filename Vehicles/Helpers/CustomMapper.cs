using AutoMapper;  
using Vehicles.Models;
using Vehicles.Contracts.Responces;
using System.Linq;
using Vehicles.Contracts.Requests;
  
namespace Vehicles.MyCustomMapper
{  
    public interface ICustomMapper
    {
        public CarResponse CarToCarResponse(Car car);
        public OwnerResponce OwnerToOwnerResponse(CarOwner car);
        public Car CarRequestToCar(CarRequest carRequest);
        public CarOwner OwnerRequestToCarOwner(OwnerRequest OwnerRequest);
        
    }
    public class CustomMapper :ICustomMapper
    {  

        public CarResponse CarToCarResponse(Car car)
        {
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
                OwnerResponces =car.ManyToManyCarOwner!=null? car.ManyToManyCarOwner.Select(o => 
                            new OwnerResponce(){
                                Id = o.CarOwnerId,
                                Name = o.CarOwner.Name,
                                SurName = o.CarOwner.SurName,
                                Patronymic = o.CarOwner.Patronymic,
                                CarOwnerPhone = o.CarOwner.CarOwnerPhone,
                                Location = o.CarOwner.Location,
                                BirthDate = o.CarOwner.BirthDate,
                                CarResponces = null
                            }).ToList():null
            };
        }
        public OwnerResponce OwnerToOwnerResponse(CarOwner owner)
        {
            return new OwnerResponce
            {
                Id = owner.Id,
                Name = owner.Name,
                SurName = owner.SurName,
                Patronymic = owner.Patronymic,
                CarOwnerPhone = owner.CarOwnerPhone,
                Location = owner.Location,
                BirthDate = owner.BirthDate,
                CarResponces = owner.ManyToManyCarOwner!=null?owner.ManyToManyCarOwner.Select(car=>
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
                ManyToManyCarOwner =carRequest.OwnerRequests!=null? carRequest.OwnerRequests.Select(o => 
                            new ManyToManyCarOwner(){
                                CarId = carRequest.Id,
                                CarOwnerId = o.Id
                            }).ToList():null
            };
        }
        public CarOwner OwnerRequestToCarOwner(OwnerRequest OwnerRequest)
        {
            return new CarOwner
            {
                Id = OwnerRequest.Id,
                Name = OwnerRequest.Name,
                SurName = OwnerRequest.SurName,
                Patronymic = OwnerRequest.Patronymic,
                CarOwnerPhone = OwnerRequest.CarOwnerPhone,
                Location = OwnerRequest.Location,
                BirthDate = OwnerRequest.BirthDate,
                ManyToManyCarOwner = OwnerRequest.CarRequests!=null?OwnerRequest.CarRequests.Select(car=>
                    new ManyToManyCarOwner(){
                        CarId = car.Id,
                        CarOwnerId = OwnerRequest.Id
                    }).ToList():null   
            };
        }
    }  
}