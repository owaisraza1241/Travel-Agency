using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TravelAgency.Application.Service
{
    public class FlightService: IFlightService
    {
        private readonly IAmadeusApiClient _amadeusApiClient;
        public FlightService(IAmadeusApiClient amadeusApiClient)
        {
            _amadeusApiClient = amadeusApiClient;
        }
        public async Task<List<FlightSearchResponseResult>> SearchFlightsAsync(string origin, string destination, DateTime departureDate)
        {

            // Handle one-way flight search
            var result = await _amadeusApiClient.SearchFlights(origin, destination, departureDate.ToString());
            return result;
        }
        //public async Task SearchFlights(string? amadeusApiUrl, string token, string origin, string destination, string departureDate)
        //{
        //    using HttpClient client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        //    var url = $"{amadeusApiUrl}/v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={departureDate}&adults=1&max=5";
        //    var response = await client.GetStringAsync(url);
        //    var flights = JArray.Parse(JObject.Parse(response)["data"]?.ToString());

        //    foreach (var flight in flights)
        //    {
        //        var itinerary = flight["itineraries"][0]["segments"][0];
        //        Console.WriteLine($"Airline: {itinerary["carrierCode"]}, Flight: {itinerary["number"]}");
        //        Console.WriteLine($"Departure: {itinerary["departure"]["iataCode"]} at {itinerary["departure"]["at"]}");
        //        Console.WriteLine($"Arrival: {itinerary["arrival"]["iataCode"]} at {itinerary["arrival"]["at"]}");
        //        Console.WriteLine($"Price: {flight["price"]["total"]} {flight["price"]["currency"]}");
        //        Console.WriteLine();
        //    }
        //}
    }
}
