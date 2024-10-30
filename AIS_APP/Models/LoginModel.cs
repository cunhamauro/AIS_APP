using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class LoginModel
    {
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
