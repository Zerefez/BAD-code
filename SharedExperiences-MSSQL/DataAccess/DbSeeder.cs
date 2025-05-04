using ExperienceService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ExperienceService.Data
{
    public class DbSeeder
    {
        private readonly SharedExperiencesDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public DbSeeder(
            SharedExperiencesDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task SeedAsync()
        {
            // Use a transaction to ensure all seeding operations are atomic
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();
                Console.WriteLine("Database created or verified");
                
                // Check what data already exists
                var rolesExist = await _roleManager.Roles.AnyAsync();
                var usersExist = await _userManager.Users.AnyAsync();
                var providersExist = _context.Providers.Any();
                var servicesExist = _context.Services.Any();
                var guestsExist = _context.Guests.Any();
                var experiencesExist = _context.SharedExperiences.Any();
                var discountsExist = _context.Discounts.Any();
                var billingsExist = _context.Billings.Any();
                
                Console.WriteLine($"Data check - Roles: {rolesExist}, Users: {usersExist}, Providers: {providersExist}, " +
                                 $"Services: {servicesExist}, Guests: {guestsExist}, Experiences: {experiencesExist}, " +
                                 $"Discounts: {discountsExist}, Billings: {billingsExist}");
                
                // Seed everything that's missing, in the correct order
                if (!rolesExist)
                {
                    await SeedRolesAsync();
                    Console.WriteLine("Roles seeded successfully");
                }
                
                if (!usersExist)
                {
                    await SeedUserAccountsAsync();
                    Console.WriteLine("User accounts seeded successfully");
                }
                
                if (!providersExist || !servicesExist || !guestsExist || !experiencesExist || !discountsExist || !billingsExist)
                {
                    await SeedAllBusinessDataAsync();
                    Console.WriteLine("Business data seeded successfully");
                }
                
                // Commit all changes
                await transaction.CommitAsync();
                Console.WriteLine("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                // Roll back if any part fails
                await transaction.RollbackAsync();
                
                // Log the error and rethrow it
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task SeedRolesAsync()
        {
            // Create roles if they don't exist
            string[] roleNames = { "Admin", "Manager", "Provider", "Guest" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private async Task SeedUserAccountsAsync()
        {
            // Create admin user if it doesn't exist
            var adminUser = await _userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Create test users for each role
            var testUsers = new List<(string email, string firstName, string lastName, string password, string role)>
            {
                ("manager@example.com", "Test", "Manager", "Manager123!", "Manager"),
                ("provider@example.com", "Test", "Provider", "Provider123!", "Provider"),
                ("guest@example.com", "Test", "Guest", "Guest123!", "Guest")
            };

            foreach (var (email, firstName, lastName, password, role) in testUsers)
            {
                var userExists = await _userManager.FindByEmailAsync(email);
                if (userExists == null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        
                        // Reload the user to get the correct ID
                        user = await _userManager.FindByEmailAsync(email);

                        // Create corresponding entry in Provider or Guest table based on role
                        if (role == "Provider")
                        {
                            var provider = new Provider
                            {
                                ApplicationUserId = user.Id,
                                Name = $"{user.FirstName} {user.LastName}",
                                Number = "123456789",
                                Address = "Test Address",
                                TouristicOperatorPermit = "Test-123456"
                            };
                            _context.Providers.Add(provider);
                            await _context.SaveChangesAsync();
                        }
                        else if (role == "Guest")
                        {
                            var guest = new Guest
                            {
                                ApplicationUserId = user.Id,
                                Name = $"{user.FirstName} {user.LastName}",
                                Number = "123456789",
                                Age = 30
                            };
                            _context.Guests.Add(guest);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Seeds all business data in a single transaction with proper error handling
        /// </summary>
        private async Task SeedAllBusinessDataAsync()
        {
            try
            {
                // Clear any partially seeded data to ensure consistency
                await ClearBusinessDataAsync();
            
                // 1. Seed Providers (no dependencies)
                Console.WriteLine("Seeding Providers...");
                var providers = new List<Provider>
                {
                    new Provider { Name = "Noah's Hotel", Address = "Finlandsgade 17, 8200 Aarhus N", Number = "+45 71555080", TouristicOperatorPermit = "/Noas Hotel/Permit" },
                    new Provider { Name = "Grand Ocean Resort", Address = "Beach Road 42, 8000 Aarhus C", Number = "+45 71717171", TouristicOperatorPermit = "/Grand Ocean Resort/Permit" },
                    new Provider { Name = "Skyline Adventures", Address = "Mountain View 99, 9000 Aalborg", Number = "+45 70707070", TouristicOperatorPermit = "/Skyline Adventures/Permit" },
                    new Provider { Name = "Sunset Bistro", Address = "Harbor Street 12, 5000 Odense", Number = "+45 72727272", TouristicOperatorPermit = "/Sunset Bistro/Permit" },
                    new Provider { Name = "City Tour Guides", Address = "Old Town Square 3, 1000 Copenhagen", Number = "+45 73737373", TouristicOperatorPermit = "/City Tour Guides/Permit" }
                };
                _context.Providers.AddRange(providers);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {providers.Count} providers");

                // Store references to providers we need later
                var noahsHotel = providers.First(p => p.Name == "Noah's Hotel");
                var skylineAdventures = providers.First(p => p.Name == "Skyline Adventures");
                var cityTourGuides = providers.First(p => p.Name == "City Tour Guides");

                // 2. Seed Services (depends on Providers)
                Console.WriteLine("Seeding Services...");
                var services = new List<Service>
                {
                    new Service 
                    { 
                        Name = "Night at Noah's Hotel Single Room", 
                        Description = "A cozy single room at Noah's Hotel.", 
                        Price = 730, 
                        Date = new DateTime(2024, 6, 15), 
                        ProviderId = noahsHotel.ProviderId
                    },
                    new Service 
                    { 
                        Name = "Night at Noah's Hotel Double Room", 
                        Description = "A spacious double room at Noah's Hotel.", 
                        Price = 910, 
                        Date = new DateTime(2024, 6, 15), 
                        ProviderId = noahsHotel.ProviderId
                    },
                    new Service 
                    { 
                        Name = "Flight AAR – VIE", 
                        Description = "One-way flight from Aarhus (AAR) to Vienna (VIE).", 
                        Price = 1000, 
                        Date = new DateTime(2024, 7, 1), 
                        ProviderId = skylineAdventures.ProviderId
                    },
                    new Service 
                    { 
                        Name = "Vienna Historic Center Walking Tour", 
                        Description = "Guided walking tour of Vienna's historic center.", 
                        Price = 100, 
                        Date = new DateTime(2024, 7, 2), 
                        ProviderId = cityTourGuides.ProviderId
                    }
                };
                _context.Services.AddRange(services);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {services.Count} services");

                // Store references to services we need later
                var singleRoom = services.First(s => s.Name == "Night at Noah's Hotel Single Room");
                var doubleRoom = services.First(s => s.Name == "Night at Noah's Hotel Double Room");
                var flight = services.First(s => s.Name == "Flight AAR – VIE");
                var walkingTour = services.First(s => s.Name == "Vienna Historic Center Walking Tour");

                // 3. Seed Guests (no dependencies)
                Console.WriteLine("Seeding Guests...");
                var guests = new List<Guest>
                {
                    new Guest { Name = "Joan Eriksen", Number = "+45 11113333", Age = 27 },
                    new Guest { Name = "Suzanne Mortensen", Number = "+45 22224444", Age = 29 },
                    new Guest { Name = "Patrick Larsen", Number = "+45 33335555", Age = 32 },
                    new Guest { Name = "Anne Christensen", Number = "+45 44446666", Age = 26 }
                };
                _context.Guests.AddRange(guests);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {guests.Count} guests");

                // Store references to guests we need later
                var joan = guests.First(g => g.Name == "Joan Eriksen");
                var suzanne = guests.First(g => g.Name == "Suzanne Mortensen");
                var patrick = guests.First(g => g.Name == "Patrick Larsen");
                var anne = guests.First(g => g.Name == "Anne Christensen");

                // 4. Seed Shared Experiences (no immediate dependencies)
                Console.WriteLine("Seeding Shared Experiences...");
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
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {sharedExperiences.Count} shared experiences");

                // Store reference to shared experience we need later
                var tripToAustria = sharedExperiences.First(se => se.Name == "Trip to Austria");

                // 5. Set up relationships (depends on all previous seedings)
                Console.WriteLine("Setting up relationships...");
                
                // Associate services with shared experiences
                tripToAustria.Services = new List<Service> { singleRoom, flight, walkingTour };
                
                // Associate guests with Trip to Austria
                tripToAustria.Guests = new List<Guest> { joan, suzanne, patrick, anne };

                // Associate guests with services
                walkingTour.Guests = new List<Guest> { joan, suzanne };
                singleRoom.Guests = new List<Guest> { joan, suzanne, patrick, anne };

                _context.Update(tripToAustria);
                _context.UpdateRange(new[] { walkingTour, singleRoom });
                await _context.SaveChangesAsync();
                Console.WriteLine("Relationships set up successfully");

                // 6. Seed Discounts (depends on Services)
                Console.WriteLine("Seeding Discounts...");
                var discounts = new List<Discount>
                {
                    new Discount 
                    { 
                        ServiceId = singleRoom.ServiceId, 
                        GuestCount = 10, 
                        DiscountValue = 10.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = singleRoom.ServiceId, 
                        GuestCount = 50, 
                        DiscountValue = 20.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = doubleRoom.ServiceId, 
                        GuestCount = 10, 
                        DiscountValue = 10.00M 
                    },
                    new Discount 
                    { 
                        ServiceId = doubleRoom.ServiceId, 
                        GuestCount = 50, 
                        DiscountValue = 20.00M 
                    }
                };
                _context.Discounts.AddRange(discounts);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {discounts.Count} discounts");

                // 7. Set up Billings (depends on Guests, Services, Providers)
                Console.WriteLine("Seeding Billings...");
                var walkingTourPrice = walkingTour.Price;
                var singleRoomPrice = singleRoom.Price;
                var flightPrice = flight.Price;

                var walkingTourProviderId = walkingTour.ProviderId;
                var singleRoomProviderId = singleRoom.ProviderId;
                var flightProviderId = flight.ProviderId;

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
                await _context.SaveChangesAsync();
                Console.WriteLine($"Added {billings.Count} billings");
                
                Console.WriteLine("All business data seeding completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SeedAllBusinessDataAsync: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // rethrow to handle in the outer transaction
            }
        }

        /// <summary>
        /// Clears all business data to ensure clean seeding
        /// </summary>
        private async Task ClearBusinessDataAsync()
        {
            try
            {
                // Remove data in a specific order to respect foreign key constraints
                if (_context.Billings.Any())
                {
                    _context.Billings.RemoveRange(_context.Billings);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Billings");
                }
                
                if (_context.Discounts.Any())
                {
                    _context.Discounts.RemoveRange(_context.Discounts);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Discounts");
                }
                
                // Clear many-to-many relationships
                var experiences = _context.SharedExperiences.Include(se => se.Services).Include(se => se.Guests).ToList();
                foreach (var experience in experiences)
                {
                    experience.Services = null;
                    experience.Guests = null;
                }
                
                var services = _context.Services.Include(s => s.Guests).ToList();
                foreach (var service in services)
                {
                    service.Guests = null;
                }
                
                if (experiences.Any() || services.Any())
                {
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing relationships");
                }
                
                if (_context.SharedExperiences.Any())
                {
                    _context.SharedExperiences.RemoveRange(_context.SharedExperiences);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Shared Experiences");
                }
                
                if (_context.Services.Any())
                {
                    _context.Services.RemoveRange(_context.Services);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Services");
                }
                
                if (_context.Guests.Any())
                {
                    _context.Guests.RemoveRange(_context.Guests);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Guests");
                }
                
                if (_context.Providers.Any())
                {
                    _context.Providers.RemoveRange(_context.Providers);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Cleared existing Providers");
                }
                
                Console.WriteLine("Successfully cleared all business data");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing business data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // rethrow to handle in the outer transaction
            }
        }

        // Original synchronous method for backward compatibility
        public void Seed()
        {
            SeedAsync().Wait();
        }
    }
}