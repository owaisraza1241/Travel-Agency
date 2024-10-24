
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.Infrastructure.Service
{
    public class AmadeusApiClient: IAmadeusApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _authTokenUrl;
        private readonly ILogger<AmadeusApiClient> _logger;
        private DateTime _tokenExpiryTime;

        public AmadeusApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<AmadeusApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _clientId = configuration["AmadeusApi:ClientId"] ?? throw new ArgumentNullException(nameof(configuration), "Client ID not found in configuration");
            _clientSecret = configuration["AmadeusApi:ClientSecret"] ?? throw new ArgumentNullException(nameof(configuration), "Client Secret not found in configuration");
            _authTokenUrl = configuration["AmadeusApi:AuthTokenUrl"] ?? throw new ArgumentNullException(nameof(configuration), "Auth Token URL not found in configuration");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetAccessToken()
        {
            using HttpClient client = new HttpClient();
            var requestBody = new StringContent(
                $"grant_type=client_credentials&client_id={_clientId}&client_secret={_clientSecret}",
                Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.PostAsync($"{_authTokenUrl}/v1/security/oauth2/token", requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenJson = JObject.Parse(responseContent);
            return tokenJson["access_token"]?.ToString();
        }


        public async Task<List<FlightSearchResponseResult>> SearchFlights(string origin, string destination, string departureDate)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessToken()}");

            var url = $"{_authTokenUrl}/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={departureDate:yyyy-MM-dd}&adults=1&max=5";
            var response = await client.GetStringAsync(url);
            var flights = JArray.Parse(JObject.Parse(response)["data"]?.ToString());
            var flightResponses = flights.Select(flight =>
            {
                var itinerary = flight["itineraries"][0]["segments"][0];
                return new FlightSearchResponseResult
                {
                    Airline = itinerary["carrierCode"]?.ToString(),
                    FlightNumber = itinerary["number"]?.ToString(),
                    DepartureAirport = itinerary["departure"]["iataCode"]?.ToString(),
                    DepartureTime = DateTime.Parse(itinerary["departure"]["at"]?.ToString()),
                    ArrivalAirport = itinerary["arrival"]["iataCode"]?.ToString(),
                    ArrivalTime = DateTime.Parse(itinerary["arrival"]["at"]?.ToString()),
                    Price = flight["price"]["total"]?.ToString(),
                    Currency = flight["price"]["currency"]?.ToString()
                };
            }).ToList();
            return flightResponses;
           
        }
        public async Task<IEnumerable<HotelSearchResponse>> SearchHotels(string city)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessToken()}");

            var url = $"{_authTokenUrl}/v2/shopping/hotel-offers?cityCode={city}&adults=1";
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: Failed to retrieve hotels. Status code: {response.StatusCode}");
                return Enumerable.Empty<HotelSearchResponse>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var hotels = JArray.Parse(JObject.Parse(content)["data"]?.ToString());

            // Map the hotels response to the DTO
            var hotelResponses = hotels.Select(hotel =>
            {
                return new HotelSearchResponse
                {
                    HotelName = hotel["hotel"]["name"]?.ToString(),
                    Address = hotel["hotel"]["address"]["lines"]?[0]?.ToString(),
                    City = hotel["hotel"]["address"]["cityName"]?.ToString(),
                    Rating = hotel["hotel"]["rating"]?.ToString(),
                    Price = hotel["offers"]?[0]?["price"]?["total"]?.ToString(),
                    Currency = hotel["offers"]?[0]?["price"]?["currency"]?.ToString(),
                    Availability = bool.Parse(hotel["offers"]?[0]?["available"]?.ToString() ?? "false")
                };
            }).ToList();

            return hotelResponses;
        }
    }
}
