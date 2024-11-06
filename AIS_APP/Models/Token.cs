using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class Token
    {
        public string? AccessToken { get; set; }

        public string? TokenType { get; set; }

        public string? UserId { get; set; }

        public string? UserEmail { get; set; }
    }
}
