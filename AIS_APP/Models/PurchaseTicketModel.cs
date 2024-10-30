using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class PurchaseTicketModel
    {
        public int FlightId { get; set; }

        public string? Seat { get; set; }

        public string? Title { get; set; }

        public string? FullName { get; set; }

        public string? IdNumber { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
