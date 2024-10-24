using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TravelAgency.Application.Service;
using TravelAgency.Domain.Interfaces;
using TravelAgency.Domain.Model;
using TravelAgency.Domain.Validators;
using TravelAgency.Infrastructure.Service;

class Program
{
    static async Task Main(string[] args)
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        // Add configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Configure HttpClient for ISabreApiClient
        services.AddHttpClient<IAmadeusApiClient, AmadeusApiClient>();

        // Add other services
        services.AddTransient<IFlightService, FlightService>();

        // Add logging
        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(dispose: true));

        // Register validators manually
        services.AddTransient<IValidator<FlightSearchRequest>, FlightSearchRequestValidator>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var flightValidator = serviceProvider.GetRequiredService<IValidator<FlightSearchRequest>>();

        await SearchOneWayFlights(serviceProvider, flightValidator);

        await SearchHotels(serviceProvider);
    }

    private static async Task SearchOneWayFlights(IServiceProvider serviceProvider, IValidator<FlightSearchRequest> validator)
    {
        var flightRequest = new FlightSearchRequest();

        Console.WriteLine("Enter origin:");
        flightRequest.Origin = Console.ReadLine();

        Console.WriteLine("Enter destination:");
        flightRequest.Destination = Console.ReadLine();

        Console.WriteLine("Enter departure date (YYYY-MM-DD):");
        var departureDateString = Console.ReadLine();
        if (DateTime.TryParse(departureDateString, out DateTime departureDate))
        {
            flightRequest.DepartureDate = departureDate;
        }

        var flightValidationResult = validator.Validate(flightRequest);
        if (!flightValidationResult.IsValid)
        {
            foreach (var error in flightValidationResult.Errors)
            {
                Console.WriteLine($"Validation error: {error.ErrorMessage}");
            }
            return;
        }

        if (!string.IsNullOrEmpty(flightRequest.Origin) && !string.IsNullOrEmpty(flightRequest.Destination))
        {
            var flights = await serviceProvider.GetRequiredService<IFlightService>()
            .SearchFlightsAsync(flightRequest.Origin, flightRequest.Destination, flightRequest.DepartureDate);

            foreach (var flight in flights)
            {
                Console.WriteLine($"Airline: {flight.Airline}, Flight: {flight.FlightNumber}");
                Console.WriteLine($"Departure: {flight.DepartureAirport} at {flight.DepartureTime}");
                Console.WriteLine($"Arrival: {flight.ArrivalAirport}  at  {flight.ArrivalTime}");
                Console.WriteLine($"Price: {flight.Price}");
                Console.WriteLine();
            }
        }
    }

    private static async Task SearchHotels(IServiceProvider serviceProvider)
    {
        var flightRequest = new FlightSearchRequest();

        Console.WriteLine("Enter City code:");
        var city = Console.ReadLine();
        if (city is not null)
        {
            var hotels = await serviceProvider.GetRequiredService<IHotelService>()
           .SearchHotels(city);

            foreach (var hotel in hotels)
            {
                Console.WriteLine($"Hotel: {hotel.HotelName}");
                Console.WriteLine($"Address: {hotel.Address}, {hotel.City}");
                Console.WriteLine($"Rating: {hotel.Rating}");
                Console.WriteLine($"Price: {hotel.Price} {hotel.Currency}");
                Console.WriteLine($"Availability: {hotel.Availability}");
                Console.WriteLine();
            }
        }
        
    }
}
