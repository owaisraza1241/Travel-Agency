using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgency.Domain.Entities;

namespace TravelAgency.Domain.Interfaces
{
    public interface IFlightService
    {
        Task<List<FlightSearchResponseResult>> SearchFlightsAsync(string origin, string destination, DateTime departureDate);

    }
}
