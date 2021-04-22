using System;
using Vehicles.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.Interfaces;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Vehicles.Helpers;
using vehicles.Helpers;
using System.IO;
using Vehicles.Contracts.V1;
using Vehicles.AuthorizationsManagers;
using vehicles.Authorization.AuthorizationsManagers;
using vehicles.Models;
using Microsoft.AspNetCore.Hosting;

namespace Vehicles.Data
{

    public class SeedData
    {
        static IVehicleImageRetriever _vehicleImageRetriever;
        string _imgDirectory;
        private readonly IWebHostEnvironment _appEnvironment;
        public SeedData(
             IVehicleImageRetriever vehicleImageRetriever,
             IWebHostEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
            _vehicleImageRetriever = vehicleImageRetriever;


            //   var directory = Directory.GetCurrentDirectory();
            var directory = _appEnvironment.WebRootPath;
            Console.WriteLine($"directory from seed data {directory}");

            var imgDirectory = $@"{directory}/{ApiRoutes.imgsPath}";

            _imgDirectory = imgDirectory;
            Console.WriteLine($"SeedData \n {_imgDirectory}");

        }


        private static StringBuilder GetChars(Random rnd)
        {
            char ch;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 2; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * rnd.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder;
        }

        public static string GenerateRandomRegistrationPlateNumber()
        {
            Random rnd = new Random();
            string middle = "";
            for (int i = 0; i < 4; i++)
            {
                middle += rnd.Next(0, 9).ToString();
            }
            StringBuilder builder = GetChars(rnd);

            string part1 = builder.ToString().ToUpper();

            StringBuilder builder1 = GetChars(rnd);

            string part2 = builder1.ToString().ToUpper();
            return string.Format($"{part1}-{middle}-{part2}");
        }
        public Penalty[] Penalties { get; set; } =
        {
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Operation by drivers of vehicles, the identification numbers of the components of which do not correspond to the entries in the registration documents, destroyed or forged;",
                Location = "Kiev",
                Price = 255,
                Date = DateTime.Parse("08/18/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "transportation by drivers of vehicles operating in the mode of minibuses, passengers in excess of the maximum number provided by the technical characteristics of the vehicle or specified in the registration documents for this vehicle",
                Price = 170,
                Location = "Kiev",
                Date = DateTime.Parse("08/3/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Violation by drivers of vehicles operating in the mode of minibuses, the rules of stopping during the boarding (disembarkation) of passengers;",
                Location = "Kiev",
                Date = DateTime.Parse("08/3/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Violation by drivers of vehicles operating in the mode of minibuses, the rules of stopping during the boarding (disembarkation) of passengers;",
                Location = "Lviv",
                Price = 255,
                Date = DateTime.Parse("02/3/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Violation of the rules of using seat belts or motorcycle helmets;",
                Location = "Kharkiv",
                Price =510,
                Date = DateTime.Parse("02/20/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Transportation of passengers on a bus route longer than five hundred kilometers by one driver;",
                Location = "Odessa",
                Price =340,
                Date = DateTime.Parse("02/20/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Driving a vehicle that has malfunctions of the brake or steering system, traction device, external lighting devices (dark time of day) or other technical malfunctions with which, in accordance with the established rules, its operation is prohibited, or re-equipped in violation of relevant rules, regulations and standards;",
                Location = "Lviv",
                Price =510,
                Date = DateTime.Parse("02/20/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Repeated within a year of violation of Art. 121 part 10 (violation of the rules of transportation of children);",
                Location = "Zhytomyr",
                Price =850,
                Date = DateTime.Parse("02/20/2020"),
            },
            new Penalty
            {
                PayedStatus = false,
                Description =
                "Driving a vehicle used to provide services for the carriage of passengers, which has malfunctions under Art. 121 Part 1, or whose technical condition and equipment do not meet the requirements of standards, traffic rules and technical operation;",
                Location = "Zhytomyr",
                Price = 760,
                Date = DateTime.Parse("02/20/2020"),
            },


        };

        public  Car[] Cars 
        { get; set; } = {
          new Car()
          {

               Brand = "Kia Mohave OFFICIAL FULL AWD",
               CarEngine = 3,
               Color = "Grey",
               Date = DateTime.Now,
               Description = "I will sell the powerful frame jeep. WITHOUT AIR SUSPENSION - everything new from Toyota Prado. Bought in December 2011 from an official dealer Started driving in 2012 Served at an official dealer One station, one owner, service book, all service history.",
               Drive = "Mixed",
               Price = 14999.9m,
               Transmision = "Auto",
               UniqueNumber = "HN-8080-OV"

          },
          new Car()
          {
               
               Brand = "Toyota Camry MAX 2004",
               CarEngine = 3,
               Color = "Black",
               Date = DateTime.Parse("08-06-2018"),
               Description = "His car, owner of those. passport. Reliable, economical, dynamic, serviced in a timely manner and only with original spare parts (according to the regulations) of the automatic transmission, the engine works perfectly (oil from replacement to replacement does not pick up a gram), the car is in technical and visual condition perfectly! European, at maximum configuration, very roomy and comfortable, everything works in the car.",
               Drive = "Front",
               Price = 7800,
               Transmision = "Auto",
               UniqueNumber = "OD-7551-TX"
          },
          new Car()
          {
               
               Brand = "Lincoln Continental 2016",
               CarEngine = 3.7F,
               Color = "Black",
               Date = DateTime.Parse("11.07.2020"),
               Description = "Lincoln Continental AWD 3.7 V6 -Panoramic roof with sunroof -Side and rear blinds -Adaptive LED light AFS -A premium acoustics -Light sensor -Rain sensor -Non-contact opening of the trunk -Easy -Automotive start -Blowing and ventilation Lane Departure Prevention System (LKA)",
               Drive = "Mixed",
               Price = 25000,
               Transmision = "Auto",
               UniqueNumber = "KA-3115-OY"
          },
          new Car()
          {
               
               Brand = "Renault Talisman 2017",
               CarEngine = 1.5F,
               Color = "Grey",
               Date = DateTime.Parse("10.28.2020"),
               Description = "Freshly driven ... 100% customs cleared .... Perna registration 26.04.2017 .... Car with a small mileage of 139 thousand ... Without any embellishments, with the original mileage .. Massage, six-color interior, climate control, adaptive cruise control, rear view camera, parktronics around the perimeter of the car, blind spot sensors, projection, navigation, multi-steering wheel, keyless access ... Ability to sell in terms of VAT ....",
               Drive = "Front",
               Price = 14300,
               Transmision = "Auto/Manual",
               UniqueNumber = "EU-0746-BG"
          },
          new Car()
          {
               
               Brand = "Kia Ceed 1.6 crdi 2007",
               CarEngine = 1.6F,
               Color = "Black",
               Date = DateTime.Parse("04.08.2020"),
               Description = "A week since I arrived from Germany. In perfect condition with minimum proven mileage. On such cars they say: \"As for yourself.\" Two sets of wheels on winter / summer disks. City consumption up to 6 liters. There is not a single worn-out button in the cabin. Two keys. The seats are in perfect condition, even without the slightest signs of wear. Rich equipment. The service book confirms the originality and reliability of a relatively small",
               Drive = "Front",
               Price = 7500,
               Transmision = "Auto/Manual",
               UniqueNumber = "DU-1848-UF"
          },
          new Car()
          {
               
               Brand = "Mazda CX-9 Signature 2018",
               CarEngine = 2.5F,
               Color = "Grey metalic",
               Date = DateTime.Parse("11.06.2020"),
               Description = "The most complete set of Signature, it includes everything that can be in this model: adaptive LED lighting system, color projection on the windshield, active and passive safety: safe braking system with pedestrian recognition, lane control, blind spot control . Multimedia touchscreen monitor and GPS navigation (maps of Ukraine and Europe). Keyless access, light, rain and parking sensor.",
               Drive = "Full",
               Price = 36000,
               Transmision = "Auto",
               UniqueNumber = "DH-5232-IF"
          },
          new Car()
          {
               
               Brand = "Porsche Panamera 2010",
               CarEngine = 4.8F,
               Color = "Grey",
               Date = DateTime.Parse("04.05.2020"),
               Description = "Condition Garage storage � Service book � First owner � Not bit � Not painted � Individual equipment Security Central locking � Airbag � ABS � Immobilizer � Alarm � ABD � Air suspension � ESP � Halogen headlights � Servo steering",
               Drive = "Full",
               Price = 27600,
               Transmision = "Auto",
               UniqueNumber = "XX-7636-WX"
          },
          new Car()
          {
               
               Brand = "Mercedes-Benz C 200 2010",
               CarEngine = 2.2F,
               Color = "Grey",
               Date = DateTime.Parse("02.06.2018"),
               Description = "Merc after a complete overhaul of the engine has a warranty on the engine 50 000 thousand km. from Mak-Auto (Bosch Service) Chernivtsi. and after a complete overhaul of the automatic transmission has a warranty from the forum-car Chernivtsi 50 thousand km. replaced all 4 brake discs and pads new radiator new battery replaced front-rear racks. soot filter sensor oil pressure sensor liners -root and connecting rod 4 pistons and oil seals, engine chain, chain dampers. heat exchanger, and all its gaskets. thermostat.",
               Drive = "Back",
               Price = 12500,
               Transmision = "Full",
               UniqueNumber = "QY-0324-PK"
          },
          new Car()
          {
               
               Brand = "Toyota Camry MAX 2004",
               CarEngine = 3,
               Color = "Black",
               Date = DateTime.Parse("08.06.2018"),
               Description = "His car, owner of those. passport. Reliable, economical, dynamic, serviced in a timely manner and only with original spare parts (according to the regulations) of the automatic transmission, the engine works perfectly (oil from replacement to replacement does not pick up a gram), the car is in technical and visual condition perfectly! European, at maximum configuration, very roomy and comfortable, everything works in the car.",
               Drive = "Front",
               Price = 7800,
               Transmision = "Auto",
               UniqueNumber = "BU-8032-UY"
          },
          new Car()
          {
               
               Brand = "Volkswagen Passat B6 IDEAL",
               CarEngine = 2,
               Color = "Black",
               Date = DateTime.Parse("11.08.2020"),
               Description = "I bring to your attention a freshly imported from Germany, customs cleared and registered car brand VOLKSWAGEN PASSAT B6. The car was bought abroad from the first owner! Full service! Equipped with the most reliable, trouble-free and most durable 2.0 Diesel engine. The car is in very good condition, not broken, not small, all windows are native, 197 thousand native mileage! Service book, two keys. The motor works like a clock, without unnecessary sounds! The interior is in perfect condition. Climate control works! The electricity is all working. Chassis in perfect condition. Everything is in order with the documents, any kind of re-registration. Who is really looking for a car for themselves will not regret it! Contact us by phone for more information.",
               Drive = "Front",
               Price = 8999,
               Transmision = "Auto",
               UniqueNumber = "NY-3363-MQ"
          },
          new Car()
          {
               
               Brand = "Opel Zafira COMFORT LINE IDEAL 2009",
               CarEngine = 1.7F,
               Color = "Black",
               Date = DateTime.Parse("01.08.2020"),
               Description = "I bring to your attention a freshly imported from Germany, customs cleared and registered car brand OPEL ZAFIRA. The car was bought abroad from the first owner! Full service! Equipped with the most reliable, trouble-free and most durable engine 1.7 Diesel. The car is in very good condition, not broken, not small, all windows are native, 191 thousand native mileage! Service book, two keys. The motor works like a clock, without unnecessary sounds!",
               Drive = "Front",
               Price = 7650,
               Transmision = "Manual",
               UniqueNumber = "EG-6118-FB"
          },
          new Car()
          {
               
               Brand = "Volkswagen Passat B7 Highline 2014",
               CarEngine = 2,
               Color = "Beige",
               Date = DateTime.Parse("12.08.2020"),
               Description = "WITHOUT ANY ADDITIONS OR ACCIDENTS !!! THE CAR IS NOT BROKEN OR PAINTED !!! TESTED BY THE DEVICE !!! The car was just driven from Germany on 08.08.2020, look at the transit numbers there is a date !!! DRIVEN PERSONALLY !!! Lane keeping sensor, distance sensor on the front of the car Dis tronic, GPS navigation, Shifting gears on the steering wheel, tire pressure sensor, dead zone sensor",
               Drive = "Front",
               Price = 15400,
               Transmision = "Manual",
               UniqueNumber = "IA-8214-WC"
          },
          new Car()
          {
               
               Brand = "Lexus RC 350 AWD 2015",
               CarEngine = 3.5F,
               Color = "Grey",
               Date = DateTime.Parse("03.06.2020"),
               Description = "The Lexus RC350AWD is a luxury sports coupe. The engine is a 3.5-liter V6, producing 350 horsepower. The layout of the cabin is built according to the 2 + 2 scheme, and with a fairly comfortable rear seat and access to it (the retractable front seats are equipped with an electromechanical device, one-touch operation and returning the seat to its previous position)",
               Drive = "Full",
               Price = 35000,
               Transmision = "Auto",
               UniqueNumber = "UT-1521-ZO"
          },
          new Car()
          {
               
               Brand = "Mercedes-Benz G 63 AMG 2020",
               CarEngine = 4F,
               Color = "Black",
               Date = DateTime.Parse("05.06.2020"),
               Description = "His car, owner of those. passport. Reliable, economical, dynamic, serviced in a timely manner and only with original spare parts (according to the regulations) of the automatic transmission, the engine works perfectly (oil from replacement to replacement does not pick up a gram), the car is in technical and visual condition perfectly! European, at maximum configuration, very roomy and comfortable, everything works in the car.",
               Drive = "Front",
               Price = 214999,
               Transmision = "Auto",
               UniqueNumber = "DG-8676-ZA"
          }
     };
        public class UserAndPassword
     {
          public CustomUser CustomUser{get;set;}
          public string Password{get;set;}
     }
        public  UserAndPassword[] UsersAndPasswords 
        { get; set; } = {

             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                       {
                    
                            BirthDate = DateTime.Parse("1988-02-06"),
                            PhoneNumber = "1-770-736-8031",
                            City = "Wisokyburgh",
                            FirstName= "Ervin",
                            LastName= "Howell",
                            EmailConfirmed = true,
                            Email = "name1Email@domain.com",
                            UserName = "ErvinUser"
                       },
                  Password = "!VeryStrPass1234_1"
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
                       BirthDate = DateTime.Parse("1978-07-26"),
                       PhoneNumber = "010-692-6593",
                       City = "Victor Plains",
                       FirstName= "Romaguera ",
                       LastName= "Deckow",
                       EmailConfirmed = true,
                       Email = "name2Email@domain.com",
                       UserName = "RomagueraUser"
                  },
                  Password = "!VeryStrPass1234_2"
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {                    
                       BirthDate = DateTime.Parse("1991-06-14"),
                       PhoneNumber = "010-692-6593",
                       City = "Wisokyburgh",
                       FirstName= "Deckow",
                       LastName= "Crist",
                       EmailConfirmed = true,
                       Email = "name3Email@domain.com",
                       UserName = "DeckowUser"
                  },
                  Password = "!VeryStrPass1234_3"
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {             
                       BirthDate = DateTime.Parse("1988-02-15"),
                       PhoneNumber = "1-463-123-4447",
                       City = "Hoeger Mall",
                       FirstName= "Robel",
                       LastName= "Corkery",
                       EmailConfirmed = true,
                       Email = "name4Email@domain.com",
                       UserName = "RobelUser"
     
                  },
                  Password = "!VeryStrPass1234_4"            
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {      
                       BirthDate = DateTime.Parse("1976-02-04"),
                       PhoneNumber = "493-170-9623",
                       City = "Douglas",
                       FirstName= "Dennis",
                       LastName= "Schulist",
                       EmailConfirmed = true,
                       Email = "name5Email@domain.com",
                       UserName = "DennisUser"
                  },
                  Password = "!VeryStrPass1234_5" 
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
               
                       BirthDate = DateTime.Parse("1989-02-01"),
                       PhoneNumber = "254-954-1289",
                       City = "Formerhoekweg",
                       FirstName= "Amelia",
                       LastName= "Van der Slik",
                       EmailConfirmed = true,
                       Email = "name6Email@domain.com",
                       UserName = "AmeliaUser"
          
                  },
                  Password = "!VeryStrPass1234_6"  
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
                       BirthDate = DateTime.Parse("1988-02-19"),
                       PhoneNumber = "477-935-8478",
                       City = "Wisokyburgh",
                       FirstName= "Sinne",
                       LastName= "Van Schooten",
                       EmailConfirmed = true,
                       Email = "name7Email@domain.com",
                       UserName = "SinneUser"
                  },
                  Password = "!VeryStrPass1234_7"  
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
               
                  BirthDate = DateTime.Parse("1988-02-04"),
                  PhoneNumber = "757-146-1278",
                  City = "Formerhoekweg",
                  FirstName= "Sinne",
                  LastName= "Van Schooten",
                  EmailConfirmed = true,
                  Email = "name8Email@domain.com",
                  UserName = "SinneUser"
          
                  },
                  Password = "!VeryStrPass1234_8"
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
               
                  BirthDate = DateTime.Parse("1988-02-25"),
                  PhoneNumber = "210-067-6132",
                  City = "Pune",
                  FirstName= "Mari",
                  LastName= "Guevara",
                  EmailConfirmed = true,
                  Email = "name9Email@domain.com",
                  UserName = "MariUser"
          
                  },
                  Password = "!VeryStrPass1234_9"  
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
               
                  BirthDate = DateTime.Parse("1988-02-14"),
                  PhoneNumber = "525-062-428",
                  City = "Santiago",
                  FirstName= "Rudi",
                  LastName= "Russo",
                  EmailConfirmed = true,
                  Email = "name10Email@domain.com",
                  UserName = "RudiUser"
               
                  },
                  Password = "!VeryStrPass1234_10"  
             },
             new UserAndPassword()
             {
                  CustomUser = new CustomUser()
                  {
               
                  BirthDate = DateTime.Parse("1988-02-17"),
                  PhoneNumber = "550-067-132",
                  City = "Cairo",
                  FirstName= "Juliet",
                  LastName= "Begum",
                  EmailConfirmed = true,
                  Email = "name11Email@domain.com",
                  UserName = "JulietUser"
               
                  },
                  Password = "!VeryStrPass1234_11"  
             }
     };

        public  List<ManyToManyCustomUserToVehicle> GetManyToManyCustomUserToVehicle
               (Car[] SomeCars, UserAndPassword[] SomeUserAndPasswords)
     {
          Random rnd = new Random();
          var manyToManyCustomUser = new List<ManyToManyCustomUserToVehicle>();
          var max = rnd.Next(30, 40);
          var addedPairs = new HashSet<CustomPair>();
          for (var i = 0; i < max; i++)
          {
          addedPairs.Add(
               new CustomPair(
                    SomeCars[rnd.Next(0, Cars.Length)].Id,
                    SomeUserAndPasswords[rnd.Next(0, SomeUserAndPasswords.Length)].CustomUser.Id
                    )
               );
          }
          foreach (var el in addedPairs)
          {
          manyToManyCustomUser.Add
               (
                    new ManyToManyCustomUserToVehicle()
                    {
                        TimeMark = DateTime.Now,
                         CarId = el.First,
                         CarOwnerId = el.Second
                    }
               );
          }
          return manyToManyCustomUser;
          
     }
        
        
        //public List<string> userIds = new List<string>();


        public async Task Initialize(
            ICustomUserManager userManager,
            ICustomRoleManager roleManager, 
            VehicleDbContext context)
        {
            
            context.Database.Migrate();


          if (!context.Cars.Any()) 
          {
                for (int i = 0; i < Cars.Length; i++)
                {
                    var ImgPath = _vehicleImageRetriever
                            .GetFilePathByVehicleBrandAndUniqueNumber
                                (Cars[i].Brand, Cars[i].UniqueNumber, _imgDirectory);
                    Cars[i].ImgPath = ImgPath;
                }
               context.Cars.AddRange(Cars);
               context.SaveChanges();
          }
            if (!context.Penalties.Any())
            {
                var rnd = new Random();

                var carsFromDb = await context.Cars.AsNoTracking().ToListAsync();
                var finalPenalties = new List<Penalty>();
                for (int i = 0; i < carsFromDb.Count; i++)
                {
                    var index = rnd.Next(0, carsFromDb.Count-1);
                    var car = carsFromDb[index];
                    var countOfPenaltiesForCar = rnd.Next(0, 5);
                    for (int j = 0; j < countOfPenaltiesForCar; j++)
                    {
                        var tempPenalties = new List<Penalty>(Penalties);
                        var penaltyIndex = rnd.Next(0, tempPenalties.Count - 1);
                        var penalty = tempPenalties[penaltyIndex];
                        penalty.CarId = car.Id;
                        finalPenalties.Add(penalty);

                        tempPenalties.Remove(penalty);
                    }
                    
                    carsFromDb.Remove(car);
                }
                context.Penalties.AddRange(finalPenalties);
                context.SaveChanges();
            }

          //using(userManager){

            if (!userManager.Users.AsNoTracking().Any())
            {
                //add password
                //new thread errors
                foreach (var user_password in UsersAndPasswords.ToList())
                {
                    var id = EnsureUser(userManager,
                                                user_password.CustomUser,
                                                user_password.Password).GetAwaiter().GetResult();
                }

            context.SaveChanges();

            }

            //TODO: get admins data from ignored file
            var adminFirstName = "admin_jHHHg3nnnwDn";
            var adminPassword = "77n3fGGGewfnnlkjddNh#2!_";
            var adminEmail = "adminUserName@custom.domain.com";
            var adminUserName = "adminUserName";

            var admin = await userManager.Users
                .FirstOrDefaultAsync(u => u.FirstName == adminFirstName);

            if (admin == null)
            {
                var adminId = await this.EnsureUser(
                    userManager,
                    new CustomUser() {
                        Email = adminEmail,
                        UserName = adminUserName,
                        FirstName = adminFirstName,
                        EmailConfirmed = true,
                        BirthDate = DateTime.Parse("1978-02-06"),
                    },
                    adminPassword
                    );

                await this.EnsureRole(
                    roleManager,
                    userManager,
                    adminId,
                    AuthorizationConstants.ContactAdministratorsRole);

                context.SaveChanges();

            }
          //}
            
            //exeption here works from second start because for the first time users are not initialized due to userManager
            //https://entityframework.net/knowledge-base/7819002/the-insert-statement-conflicted-with-the-foreign-key-constraint-in-entity-framework

          if (!context.ManyToManyCarOwners.Any())
          {
                try
                {
                    await TryAddManyToManyRelation(context);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(ex);
                    
                }
          }
          

        }

        private async Task TryAddManyToManyRelation(VehicleDbContext context)
        {
            var manyToManyCustomUserToVehicle = new List<ManyToManyCustomUserToVehicle>();

                var carsFromDb = await context.Cars.AsNoTracking().ToListAsync();
                var usersFromDb = await context.Users.AsNoTracking().ToListAsync();
                var userAndPasswordFromDb = new List<UserAndPassword>();
                for (int i = 0; i < usersFromDb.Count; i++)
                {
                    userAndPasswordFromDb.Add(new UserAndPassword()
                    {
                        CustomUser = usersFromDb[i],
                        Password = UsersAndPasswords[i].Password
                    });
                }

                manyToManyCustomUserToVehicle = GetManyToManyCustomUserToVehicle(
                     carsFromDb.ToArray(), userAndPasswordFromDb.ToArray());
            

            context.ManyToManyCarOwners.AddRange(manyToManyCustomUserToVehicle);
            context.SaveChanges();
        }


        private async Task<string> EnsureUser(
              ICustomUserManager userManager,
              CustomUser newUser,
              string Password)
        {
          
              var user = await userManager.FindByNameAsync(newUser.UserName);
              if (user == null)
              {
                   user = newUser;
                   userManager.CreateAsync(user, Password).Wait();
              }
              return user.Id;
        }

        private async Task<IdentityResult> EnsureRole(
            ICustomRoleManager roleManager,
            ICustomUserManager userManager,
            string uid, string role)
        {
            IdentityResult IR = null;

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))//ensure that current role doesn't exists and create it
            {
                IR = await roleManager.CreateAsync(new CustomRole(role));
            }

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("User does not exists!");
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                IR = await userManager.AddToRoleAsync(user, role);
            }


            return IR;
        }

    }
}

