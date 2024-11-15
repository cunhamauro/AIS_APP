﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class TicketRecord
    {
        public int Id { get; set; } // Ticket Id

        public string? UserId { get; set; }

        public string? HolderIdNumber { get; set; }

        public string? Seat { get; set; }

        public decimal TicketPrice { get; set; }

        public string? FlightNumber { get; set; }

        public string? OriginCity { get; set; }

        public string? OriginCountry { get; set; }

        public string? OriginFlagImageUrl { get; set; }

        public string? DestinationCity { get; set; }

        public string? DestinationCountry { get; set; }

        public string? DestinationFlagImageUrl { get; set; }

        public DateTime Departure { get; set; }

        public DateTime Arrival { get; set; }

        public bool Canceled { get; set; }

        public string Status => Canceled ? "Canceled" : "Completed";

        public string ScheduledStatus => Canceled ? "Canceled" : "Scheduled";

        public DateTime Boarding => Departure.AddMinutes(-30);
    }
}
