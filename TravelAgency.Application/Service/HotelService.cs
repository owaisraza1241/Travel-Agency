using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgency.Domain.Entities;
using TravelAgency.Domain.Interfaces;

namespace TravelAgency.Application.Service
{
    public class HotelService: IHotelService
    {
        private readonly IAmadeusApiClient _amadeusApiClient;
        public HotelService(IAmadeusApiClient amadeusApiClient)
        {
            _amadeusApiClient = amadeusApiClient;
        }
        public async Task<IEnumerable<HotelSearchResponse>> SearchHotels(string city)
        {

            // Handle one-way flight search
            var result = await _amadeusApiClient.SearchHotels(city);
            return result;
        }
    }
}
