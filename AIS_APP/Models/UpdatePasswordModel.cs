using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class UpdatePasswordModel
    {
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? Confirm { get; set; }
    }
}
