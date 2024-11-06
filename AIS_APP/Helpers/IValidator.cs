using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIS_APP.Helpers
{
    public interface IValidator
    {
        string FirstNameError { get; set; }

        string LastNameError { get; set; }

        string EmailError { get; set; }

        string PhoneNumberError { get; set; }

        string PasswordError { get; set; }

        Task<bool> Validate(string firstName, string lastName, string email, string phonenumber, string password, string confirmPassword);
    }
}
