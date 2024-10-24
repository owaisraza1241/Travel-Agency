using TravelAgency.Domain.Entities;

namespace TravelAgency.Domain.Interfaces
{
    public interface IAmadeusApiClient
    {
        Task<List<FlightSearchResponseResult>> SearchFlights(string origin, string destination, string departureDate);
        Task<IEnumerable<HotelSearchResponse>> SearchHotels(string city);
    }
}
