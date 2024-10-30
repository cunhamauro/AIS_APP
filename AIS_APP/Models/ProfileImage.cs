using AIS_APP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Models
{
    public class ProfileImage
    {
        public string? ImageUrl { get; set; }

        public string? ImagePath => AppConfig.Url + ImageUrl;
    }
}
