using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedExperinces.WebApi.DataAccess;
using SharedExperinces.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = "Data Source=127.0.0.1,1433;Database=SharedExperincesDB;User Id=sa;Password=Password,1;TrustServerCertificate=True";
  

Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<SharedExperinceContext>(options =>
	options.UseSqlServer(connectionString));


builder.Services.AddTransient<ServiceService>();
builder.Services.AddTransient<SharedExperienceService>();
builder.Services.AddTransient<ProviderService>();






var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<SharedExperinceContext>();

	dbContext.Database.Migrate(); 

	SharedExperinceContext.Seed(dbContext); 
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();