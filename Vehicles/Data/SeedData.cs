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

namespace Vehicles.Data
{

    public static class SeedData
    {
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

        public static async Task Initialize(VehicleDbContext context)
        {

            var Cars = new Car[]
            {
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
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
                    UniqueNumber = GenerateRandomRegistrationPlateNumber()
               }
            };
            if (!context.Cars.Any()) 
            {
                context.Cars.AddRange(Cars);
                await context.SaveChangesAsync();
            }

            var Owners = new CarOwner[]
            {
                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-06"),
                     CarOwnerPhone = "1-770-736-8031",
                     Location = "Wisokyburgh",
                     Name = "Ervin",
                     SurName = "Howell",
                     Patronymic = "Leanne"
                },
                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1978-07-26"),
                     CarOwnerPhone = "010-692-6593",
                     Location = "Victor Plains",
                     Name = "Romaguera ",
                     SurName = "Deckow",
                     Patronymic = "Clementine"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1991-06-14"),
                     CarOwnerPhone = "010-692-6593",
                     Location = "Wisokyburgh",
                     Name = "Deckow",
                     SurName = "Crist",
                     Patronymic = "Leanne"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-15"),
                     CarOwnerPhone = "1-463-123-4447",
                     Location = "Hoeger Mall",
                     Name = "Robel",
                     SurName = "Corkery",
                     Patronymic = "Keebler"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1976-02-04"),
                     CarOwnerPhone = "493-170-9623",
                     Location = "Douglas",
                     Name = "Dennis",
                     SurName = "Schulist",
                     Patronymic = "Hisham"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1989-02-01"),
                     CarOwnerPhone = "254-954-1289",
                     Location = "Formerhoekweg",
                     Name = "Amelia",
                     SurName = "Van der Slik",
                     Patronymic = "Robert"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-19"),
                     CarOwnerPhone = "477-935-8478",
                     Location = "Wisokyburgh",
                     Name = "Sinne",
                     SurName = "Van Schooten",
                     Patronymic = "Cairo"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-04"),
                     CarOwnerPhone = "757-146-1278",
                     Location = "Formerhoekweg",
                     Name = "Sinne",
                     SurName = "Van Schooten",
                     Patronymic = "Hester"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-25"),
                     CarOwnerPhone = "210-067-6132",
                     Location = "Pune",
                     Name = "Mari",
                     SurName = "Guevara",
                     Patronymic = "Leanne"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-14"),
                     CarOwnerPhone = "525-062-428",
                     Location = "Santiago",
                     Name = "Rudi",
                     SurName = "Russo",
                     Patronymic = "Theo"
                },

                new CarOwner()
                {
                     BirthDate = DateTime.Parse("1988-02-17"),
                     CarOwnerPhone = "550-067-132",
                     Location = "Cairo",
                     Name = "Juliet",
                     SurName = "Begum",
                     Patronymic = "Pablo"
                }
            };

            if (!context.CarOwners.Any())
            {
                context.CarOwners.AddRange(Owners);
                await context.SaveChangesAsync();
            }

            if (!context.ManyToManyCarOwners.Any())
            {
                Random rnd = new Random();
                var manyToManyCarOwner = new List<ManyToManyCarOwner>();
                var max = rnd.Next(30, 40);
                var addedPairs = new HashSet<CustomPair>();
                for (var i = 0; i < max; i++)
                {
                    addedPairs.Add(new CustomPair(Cars[rnd.Next(0, Cars.Length)].Id, Owners[rnd.Next(0, Owners.Length)].Id));
                }
                foreach(var el in addedPairs) 
                {
                    manyToManyCarOwner.Add
                        (
                            new ManyToManyCarOwner()
                            {
                                CarId = el.First,
                                CarOwnerId = el.Second
                            }
                        );
                }

                context.AddRange(manyToManyCarOwner);
                await context.SaveChangesAsync();
            }
        }

    }
}

