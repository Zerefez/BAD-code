using ExperienceService.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExperienceService.Data
{
    public class DbSeeder
    {
        private readonly MongoDbContext _context;

        public DbSeeder(MongoDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            if (!_context.Providers.Find(_ => true).Any())
            {
                SeedProviders();
            }

            if (!_context.Services.Find(_ => true).Any())
            {
                SeedServices();
            }

            if (!_context.Guests.Find(_ => true).Any())
            {
                SeedGuests();
            }

            if (!_context.SharedExperiences.Find(_ => true).Any())
            {
                SeedSharedExperiences();
            }

            if (!_context.Discounts.Find(_ => true).Any())
            {
                SeedDiscounts();
            }

            if (!_context.Billings.Find(_ => true).Any())
            {
                SeedBillings();
            }
        }

        private void SeedProviders()
        {
            var providers = new List<Provider>
            {
                new Provider { Name = "Noah's Hotel", Address = "Finlandsgade 17, 8200 Aarhus N", Number = "+45 71555080", TouristicOperatorPermit = "/Noas Hotel/Permit" },
                new Provider { Name = "Grand Ocean Resort", Address = "Beach Road 42, 8000 Aarhus C", Number = "+45 71717171", TouristicOperatorPermit = "/Grand Ocean Resort/Permit" },
                new Provider { Name = "Skyline Adventures", Address = "Mountain View 99, 9000 Aalborg", Number = "+45 70707070", TouristicOperatorPermit = "/Skyline Adventures/Permit" },
                new Provider { Name = "Sunset Bistro", Address = "Harbor Street 12, 5000 Odense", Number = "+45 72727272", TouristicOperatorPermit = "/Sunset Bistro/Permit" },
                new Provider { Name = "City Tour Guides", Address = "Old Town Square 3, 1000 Copenhagen", Number = "+45 73737373", TouristicOperatorPermit = "/City Tour Guides/Permit" }
            };
            
            _context.Providers.InsertMany(providers);
        }

        private void SeedServices()
        {
            var noahsHotelId = _context.Providers.Find(p => p.Name == "Noah's Hotel").FirstOrDefault().Id;
            var skylineAdventuresId = _context.Providers.Find(p => p.Name == "Skyline Adventures").FirstOrDefault().Id;
            var cityTourGuidesId = _context.Providers.Find(p => p.Name == "City Tour Guides").FirstOrDefault().Id;

            var services = new List<Service>
            {
                new Service 
                { 
                    Name = "Night at Noah's Hotel Single Room", 
                    Description = "A cozy single room at Noah's Hotel.", 
                    Price = 730, 
                    Date = new DateTime(2024, 6, 15), 
                    ProviderId = noahsHotelId
                },
                new Service 
                { 
                    Name = "Night at Noah's Hotel Double Room", 
                    Description = "A spacious double room at Noah's Hotel.", 
                    Price = 910, 
                    Date = new DateTime(2024, 6, 15), 
                    ProviderId = noahsHotelId
                },
                new Service 
                { 
                    Name = "Flight AAR – VIE", 
                    Description = "One-way flight from Aarhus (AAR) to Vienna (VIE).", 
                    Price = 1000, 
                    Date = new DateTime(2024, 7, 1), 
                    ProviderId = skylineAdventuresId
                },
                new Service 
                { 
                    Name = "Vienna Historic Center Walking Tour", 
                    Description = "Guided walking tour of Vienna's historic center.", 
                    Price = 100, 
                    Date = new DateTime(2024, 7, 2), 
                    ProviderId = cityTourGuidesId
                }
            };
            
            _context.Services.InsertMany(services);
        }

        private void SeedGuests()
        {
            var guests = new List<Guest>
            {
                new Guest { Name = "Joan Eriksen", Number = "+45 11113333", Age = 27 },
                new Guest { Name = "Suzanne Mortensen", Number = "+45 22224444", Age = 29 },
                new Guest { Name = "Patrick Larsen", Number = "+45 33335555", Age = 32 },
                new Guest { Name = "Anne Christensen", Number = "+45 44446666", Age = 26 }
            };
            
            _context.Guests.InsertMany(guests);
        }

        private void SeedSharedExperiences()
        {
            var sharedExperiences = new List<SharedExperience>
            {
                new SharedExperience 
                { 
                    Name = "Trip to Austria", 
                    Date = DateTime.Now
                },
                new SharedExperience 
                { 
                    Name = "Dinner Downtown", 
                    Date = DateTime.Now
                },
                new SharedExperience 
                { 
                    Name = "Pottery Weekend", 
                    Date = DateTime.Now
                }
            };
            
            _context.SharedExperiences.InsertMany(sharedExperiences);

            // Associate services with shared experiences
            var tripToAustria = _context.SharedExperiences.Find(se => se.Name == "Trip to Austria").FirstOrDefault();
            
            var singleRoomId = _context.Services.Find(s => s.Name == "Night at Noah's Hotel Single Room").FirstOrDefault().Id;
            var flightId = _context.Services.Find(s => s.Name == "Flight AAR – VIE").FirstOrDefault().Id;
            var walkingTourId = _context.Services.Find(s => s.Name == "Vienna Historic Center Walking Tour").FirstOrDefault().Id;

            var serviceIds = new List<string> { singleRoomId, flightId, walkingTourId };
            
            // Associate guests with Trip to Austria
            var joanId = _context.Guests.Find(g => g.Name == "Joan Eriksen").FirstOrDefault().Id;
            var suzanneId = _context.Guests.Find(g => g.Name == "Suzanne Mortensen").FirstOrDefault().Id;
            var patrickId = _context.Guests.Find(g => g.Name == "Patrick Larsen").FirstOrDefault().Id;
            var anneId = _context.Guests.Find(g => g.Name == "Anne Christensen").FirstOrDefault().Id;

            var guestIds = new List<string> { joanId, suzanneId, patrickId, anneId };

            // Update the shared experience with service and guest IDs
            var filter = Builders<SharedExperience>.Filter.Eq(se => se.Id, tripToAustria.Id);
            var update = Builders<SharedExperience>.Update
                .Set(se => se.ServiceIds, serviceIds)
                .Set(se => se.GuestIds, guestIds);
            
            _context.SharedExperiences.UpdateOne(filter, update);
        }

        private void SeedDiscounts()
        {
            var singleRoomId = _context.Services.Find(s => s.Name == "Night at Noah's Hotel Single Room").FirstOrDefault().Id;
            var doubleRoomId = _context.Services.Find(s => s.Name == "Night at Noah's Hotel Double Room").FirstOrDefault().Id;

            var discounts = new List<Discount>
            {
                new Discount 
                { 
                    Name = "Single Room 10% Discount",
                    ServiceId = singleRoomId, 
                    GuestCount = 10, 
                    Percentage = 10 
                },
                new Discount 
                { 
                    Name = "Single Room 20% Discount",
                    ServiceId = singleRoomId, 
                    GuestCount = 50, 
                    Percentage = 20 
                },
                new Discount 
                { 
                    Name = "Double Room 10% Discount",
                    ServiceId = doubleRoomId, 
                    GuestCount = 10, 
                    Percentage = 10 
                },
                new Discount 
                { 
                    Name = "Double Room 20% Discount",
                    ServiceId = doubleRoomId, 
                    GuestCount = 50, 
                    Percentage = 20 
                }
            };
            
            _context.Discounts.InsertMany(discounts);
        }

        private void SeedBillings()
        {
            var walkingTourId = _context.Services.Find(s => s.Name == "Vienna Historic Center Walking Tour").FirstOrDefault().Id;
            var singleRoomId = _context.Services.Find(s => s.Name == "Night at Noah's Hotel Single Room").FirstOrDefault().Id;
            var flightId = _context.Services.Find(s => s.Name == "Flight AAR – VIE").FirstOrDefault().Id;

            var walkingTourService = _context.Services.Find(s => s.Id == walkingTourId).FirstOrDefault();
            var singleRoomService = _context.Services.Find(s => s.Id == singleRoomId).FirstOrDefault();
            var flightService = _context.Services.Find(s => s.Id == flightId).FirstOrDefault();

            var walkingTourPrice = walkingTourService.Price;
            var singleRoomPrice = singleRoomService.Price;
            var flightPrice = flightService.Price;

            var walkingTourProviderId = walkingTourService.ProviderId;
            var singleRoomProviderId = singleRoomService.ProviderId;
            var flightProviderId = flightService.ProviderId;

            var joanId = _context.Guests.Find(g => g.Name == "Joan Eriksen").FirstOrDefault().Id;
            var suzanneId = _context.Guests.Find(g => g.Name == "Suzanne Mortensen").FirstOrDefault().Id;
            var patrickId = _context.Guests.Find(g => g.Name == "Patrick Larsen").FirstOrDefault().Id;
            var anneId = _context.Guests.Find(g => g.Name == "Anne Christensen").FirstOrDefault().Id;

            // Create billings based on the registrations in the original seeder
            var billings = new List<Billing>
            {
                // Walking Tour - 2 guests
                new Billing { GuestId = joanId, ProviderId = walkingTourProviderId, Amount = walkingTourPrice, Date = DateTime.Now },
                new Billing { GuestId = suzanneId, ProviderId = walkingTourProviderId, Amount = walkingTourPrice, Date = DateTime.Now },
                
                // Single Room - 4 guests
                new Billing { GuestId = joanId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice, Date = DateTime.Now },
                new Billing { GuestId = suzanneId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice, Date = DateTime.Now },
                new Billing { GuestId = patrickId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice, Date = DateTime.Now },
                new Billing { GuestId = anneId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice, Date = DateTime.Now },
                
                // Flight - 4 guests
                new Billing { GuestId = joanId, ProviderId = flightProviderId, Amount = flightPrice, Date = DateTime.Now },
                new Billing { GuestId = suzanneId, ProviderId = flightProviderId, Amount = flightPrice, Date = DateTime.Now },
                new Billing { GuestId = patrickId, ProviderId = flightProviderId, Amount = flightPrice, Date = DateTime.Now },
                new Billing { GuestId = anneId, ProviderId = flightProviderId, Amount = flightPrice, Date = DateTime.Now }
            };
            
            _context.Billings.InsertMany(billings);
        }
    }
} 