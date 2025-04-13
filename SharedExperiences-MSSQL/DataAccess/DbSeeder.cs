using ExperienceService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExperienceService.Data
{
    public class DbSeeder
    {
        private readonly SharedExperiencesDbContext _context;

        public DbSeeder(SharedExperiencesDbContext context)
        {
            _context = context;
        }

        public void Seed()
        {
            _context.Database.EnsureCreated();

            if (!_context.Providers.Any())
            {
                var providers = new List<Provider>
                {
                    new Provider { Name = "Noah's Hotel", Address = "Finlandsgade 17, 8200 Aarhus N", Number = "+45 71555080", TouristicOperatorPermit = "/Noas Hotel/Permit" },
                    new Provider { Name = "Grand Ocean Resort", Address = "Beach Road 42, 8000 Aarhus C", Number = "+45 71717171", TouristicOperatorPermit = "/Grand Ocean Resort/Permit" },
                    new Provider { Name = "Skyline Adventures", Address = "Mountain View 99, 9000 Aalborg", Number = "+45 70707070", TouristicOperatorPermit = "/Skyline Adventures/Permit" },
                    new Provider { Name = "Sunset Bistro", Address = "Harbor Street 12, 5000 Odense", Number = "+45 72727272", TouristicOperatorPermit = "/Sunset Bistro/Permit" },
                    new Provider { Name = "City Tour Guides", Address = "Old Town Square 3, 1000 Copenhagen", Number = "+45 73737373", TouristicOperatorPermit = "/City Tour Guides/Permit" }
                };
                _context.Providers.AddRange(providers);
                _context.SaveChanges();
            }

            if (!_context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service 
                    { 
                        Name = "Night at Noah's Hotel Single Room", 
                        Description = "A cozy single room at Noah's Hotel.", 
                        Price = 730, 
                        Date = new DateTime(2024, 6, 15), 
                        ProviderId = _context.Providers.First(p => p.Name == "Noah's Hotel").ProviderId
                    },
                    new Service 
                    { 
                        Name = "Night at Noah's Hotel Double Room", 
                        Description = "A spacious double room at Noah's Hotel.", 
                        Price = 910, 
                        Date = new DateTime(2024, 6, 15), 
                        ProviderId = _context.Providers.First(p => p.Name == "Noah's Hotel").ProviderId
                    },
                    new Service 
                    { 
                        Name = "Flight AAR – VIE", 
                        Description = "One-way flight from Aarhus (AAR) to Vienna (VIE).", 
                        Price = 1000, 
                        Date = new DateTime(2024, 7, 1), 
                        ProviderId = _context.Providers.First(p => p.Name == "Skyline Adventures").ProviderId
                    },
                    new Service 
                    { 
                        Name = "Vienna Historic Center Walking Tour", 
                        Description = "Guided walking tour of Vienna's historic center.", 
                        Price = 100, 
                        Date = new DateTime(2024, 7, 2), 
                        ProviderId = _context.Providers.First(p => p.Name == "City Tour Guides").ProviderId
                    }
                };
                _context.Services.AddRange(services);
                _context.SaveChanges();
            }

            if (!_context.Guests.Any())
            {
                var guests = new List<Guest>
                {
                    new Guest { Name = "Joan Eriksen", Number = "+45 11113333", Age = 27 },
                    new Guest { Name = "Suzanne Mortensen", Number = "+45 22224444", Age = 29 },
                    new Guest { Name = "Patrick Larsen", Number = "+45 33335555", Age = 32 },
                    new Guest { Name = "Anne Christensen", Number = "+45 44446666", Age = 26 }
                };
                _context.Guests.AddRange(guests);
                _context.SaveChanges();
            }

            if (!_context.SharedExperiences.Any())
            {
                var sharedExperiences = new List<SharedExperience>
                {
                    new SharedExperience 
                    { 
                        Name = "Trip to Austria", 
                        Description = "A group trip exploring Vienna, including flights, hotel stays, and guided tours.",
                        Date = DateTime.Now
                    },
                    new SharedExperience 
                    { 
                        Name = "Dinner Downtown", 
                        Description = "A fine dining experience at a highly-rated restaurant in the city center.",
                        Date = DateTime.Now
                    },
                    new SharedExperience 
                    { 
                        Name = "Pottery Weekend", 
                        Description = "Pottery weekend with good colleagues.",
                        Date = DateTime.Now
                    }
                };
                _context.SharedExperiences.AddRange(sharedExperiences);
                _context.SaveChanges();

                // Associate services with shared experiences
                var tripToAustria = sharedExperiences.First(se => se.Name == "Trip to Austria");
                var singleRoom = _context.Services.First(s => s.Name == "Night at Noah's Hotel Single Room");
                var flight = _context.Services.First(s => s.Name == "Flight AAR – VIE");
                var walkingTour = _context.Services.First(s => s.Name == "Vienna Historic Center Walking Tour");

                tripToAustria.Services = new List<Service> { singleRoom, flight, walkingTour };
                
                // Associate guests with Trip to Austria
                var joan = _context.Guests.First(g => g.Name == "Joan Eriksen");
                var suzanne = _context.Guests.First(g => g.Name == "Suzanne Mortensen");
                var patrick = _context.Guests.First(g => g.Name == "Patrick Larsen");
                var anne = _context.Guests.First(g => g.Name == "Anne Christensen");
                
                tripToAustria.Guests = new List<Guest> { joan, suzanne, patrick, anne };

                walkingTour.Guests = new List<Guest> { joan, suzanne };
                singleRoom.Guests = new List<Guest> { joan, suzanne, patrick, anne };

                _context.SharedExperiences.Update(tripToAustria);
                _context.SaveChanges();
            }

            

            if (!_context.Discounts.Any())
            {
                var discounts = new List<Discount>
                {
                    new Discount 
                    { 
                        ServiceId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Single Room").ServiceId, 
                        GuestCount = 10, 
                        DiscountValue = 10.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Single Room").ServiceId, 
                        GuestCount = 50, 
                        DiscountValue = 20.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Double Room").ServiceId, 
                        GuestCount = 10, 
                        DiscountValue = 10.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Double Room").ServiceId, 
                        GuestCount = 50, 
                        DiscountValue = 20.00M 
                    }
                };
                _context.Discounts.AddRange(discounts);
                _context.SaveChanges();
            }

            // Set up registrations and billings
            if (!_context.Billings.Any())
            {
                var walkingTourId = _context.Services.First(s => s.Name == "Vienna Historic Center Walking Tour").ServiceId;
                var singleRoomId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Single Room").ServiceId;
                var doubleRoomId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Double Room").ServiceId;
                var flightId = _context.Services.First(s => s.Name == "Flight AAR – VIE").ServiceId;

                var walkingTourPrice = _context.Services.First(s => s.ServiceId == walkingTourId).Price;
                var singleRoomPrice = _context.Services.First(s => s.ServiceId == singleRoomId).Price;
                var flightPrice = _context.Services.First(s => s.ServiceId == flightId).Price;

                var walkingTourProviderId = _context.Services.First(s => s.ServiceId == walkingTourId).ProviderId;
                var singleRoomProviderId = _context.Services.First(s => s.ServiceId == singleRoomId).ProviderId;
                var flightProviderId = _context.Services.First(s => s.ServiceId == flightId).ProviderId;

                var joan = _context.Guests.First(g => g.Name == "Joan Eriksen");
                var suzanne = _context.Guests.First(g => g.Name == "Suzanne Mortensen");
                var patrick = _context.Guests.First(g => g.Name == "Patrick Larsen");
                var anne = _context.Guests.First(g => g.Name == "Anne Christensen");

                // Create billings based on the registrations in the second file
                var billings = new List<Billing>
                {
                    // Walking Tour - 2 guests
                    new Billing { GuestId = joan.GuestId, ProviderId = walkingTourProviderId, Amount = walkingTourPrice },
                    new Billing { GuestId = suzanne.GuestId, ProviderId = walkingTourProviderId, Amount = walkingTourPrice },
                    
                    // Single Room - 4 guests
                    new Billing { GuestId = joan.GuestId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice },
                    new Billing { GuestId = suzanne.GuestId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice },
                    new Billing { GuestId = patrick.GuestId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice },
                    new Billing { GuestId = anne.GuestId, ProviderId = singleRoomProviderId, Amount = singleRoomPrice },
                    
                    // Flight - 4 guests
                    new Billing { GuestId = joan.GuestId, ProviderId = flightProviderId, Amount = flightPrice },
                    new Billing { GuestId = suzanne.GuestId, ProviderId = flightProviderId, Amount = flightPrice },
                    new Billing { GuestId = patrick.GuestId, ProviderId = flightProviderId, Amount = flightPrice },
                    new Billing { GuestId = anne.GuestId, ProviderId = flightProviderId, Amount = flightPrice }
                };
                
                _context.Billings.AddRange(billings);
                _context.SaveChanges();
            }
        }
    }
}