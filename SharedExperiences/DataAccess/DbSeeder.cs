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
                    new Provider { /*CVR = "11111114"*/ Name = "Noah's Hotel", Address = "Finlandsgade 17, 8200 Aarhus N", Number = "+45 71555080", TouristicOperatorPermit = "12345678" },
                    new Provider { /*CVR = "22222222"*/ Name = "Grand Ocean Resort", Address = "Beach Road 42, 8000 Aarhus C", Number = "+45 71717171", TouristicOperatorPermit = "87654321" },
                    new Provider { /*CVR = "33333333"*/ Name = "Skyline Adventures", Address = "Mountain View 99, 9000 Aalborg", Number = "+45 70707070", TouristicOperatorPermit = "12348765" },
                    new Provider { /*CVR = "44444444"*/ Name = "Sunset Bistro", Address = "Harbor Street 12, 5000 Odense", Number = "+45 72727272", TouristicOperatorPermit = "87651234" },
                    new Provider { /*CVR = "55555555"*/ Name = "City Tour Guides", Address = "Old Town Square 3, 1000 Copenhagen", Number = "+45 73737373", TouristicOperatorPermit = "12345679" }
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
                        ProviderId = 1
                    },
                    new Service 
                    { 
                        Name = "Night at Noah's Hotel Double Room", 
                        Description = "A spacious double room at Noah's Hotel.", 
                        Price = 910, 
                        Date = new DateTime(2024, 6, 15), 
                        ProviderId = 1
                    },
                    new Service 
                    { 
                        Name = "Flight AAR – VIE", 
                        Description = "One-way flight from Aarhus (AAR) to Vienna (VIE).", 
                        Price = 1000, 
                        Date = new DateTime(2024, 7, 1), 
                        ProviderId = 3
                    },
                    new Service 
                    { 
                        Name = "Vienna Historic Center Walking Tour", 
                        Description = "Guided walking tour of Vienna's historic center.", 
                        Price = 100, 
                        Date = new DateTime(2024, 7, 2), 
                        ProviderId = 5
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
                // Add sample shared experiences matching the second file
                var sharedExperiences = new List<SharedExperience>
                {
                    new SharedExperience 
                    { 
                        Name = "Trip to Austria", 
                        Description = "A group trip exploring Vienna, including flights, hotel stays, and guided tours.",
                        Date = DateTime.Now.AddDays(30)
                    },
                    new SharedExperience 
                    { 
                        Name = "Dinner Downtown", 
                        Description = "A fine dining experience at a highly-rated restaurant in the city center.",
                        Date = DateTime.Now.AddDays(15)
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
                _context.SaveChanges();
            }

            if (!_context.Discounts.Any())
            {
                // Add sample discounts matching the second file
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

            // Add registrations (equivalent to guest-service relationships)
            if (!_context.Billings.Any())
            {
                var walkingTourId = _context.Services.First(s => s.Name == "Vienna Historic Center Walking Tour").ServiceId;
                var singleRoomId = _context.Services.First(s => s.Name == "Night at Noah's Hotel Single Room").ServiceId;
                var joanId = _context.Guests.First(g => g.Name == "Joan Eriksen").GuestId;
                var suzanneId = _context.Guests.First(g => g.Name == "Suzanne Mortensen").GuestId;

                // Use billings to represent the registrations
                var billings = new List<Billing>
                {
                    new Billing 
                    { 
                        GuestId = joanId, 
                        ProviderId = _context.Services.First(s => s.ServiceId == walkingTourId).ProviderId,
                        Amount = _context.Services.First(s => s.ServiceId == walkingTourId).Price
                    },
                    new Billing 
                    { 
                        GuestId = suzanneId, 
                        ProviderId = _context.Services.First(s => s.ServiceId == walkingTourId).ProviderId,
                        Amount = _context.Services.First(s => s.ServiceId == walkingTourId).Price
                    },
                    new Billing 
                    { 
                        GuestId = joanId, 
                        ProviderId = _context.Services.First(s => s.ServiceId == singleRoomId).ProviderId,
                        Amount = _context.Services.First(s => s.ServiceId == singleRoomId).Price
                    },
                    new Billing 
                    { 
                        GuestId = suzanneId, 
                        ProviderId = _context.Services.First(s => s.ServiceId == singleRoomId).ProviderId,
                        Amount = _context.Services.First(s => s.ServiceId == singleRoomId).Price
                    }
                };
                _context.Billings.AddRange(billings);
                _context.SaveChanges();
            }
        }
    }
}