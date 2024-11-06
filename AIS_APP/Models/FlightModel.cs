using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class FlightModel
    {
        public int Id { get; set; }

        public string? FlightNumber { get; set; }

        public string? OriginCity { get; set; }

        public string? OriginCountry { get; set; }

        public string? Origin => $"{OriginCity}, {OriginCountry}";

        public string? OriginFlagImageUrl { get; set; }

        public string? DestinationCity { get; set; }

        public string? DestinationCountry { get; set; }

        public string? Destination => $"{DestinationCity}, {DestinationCountry}";

        public string? DestinationFlagImageUrl { get; set; }

        public DateTime Departure { get; set; }

        public DateTime Arrival { get; set; }

        public TimeSpan Duration => Arrival - Departure;

        public string DurationFormatted => $"{Duration.Hours:D2}:{Duration.Minutes:D2}:{Duration.Seconds:D2}";

        public bool Canceled { get; set; }

        public int FlightCapacity { get; set; }

        public List<string>? AvailableSeats { get; set; }

        public int SeatsTaken => FlightCapacity - AvailableSeats.Count;

        public int AvailableSeatsCount => AvailableSeats.Count;

        public decimal CurrentTicketPrice {  get; set; }

        public DateTime Boarding => Departure.AddMinutes(-30);
    }
}
