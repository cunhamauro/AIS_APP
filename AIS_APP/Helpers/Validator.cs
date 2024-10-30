using System.Text.RegularExpressions;

namespace AIS_APP.Helpers
{
    public class Validator : IValidator
    {
        public string FirstNameError { get; set; } = "";
        public string LastNameError { get; set; } = "";
        public string EmailError { get; set; } = "";
        public string PhoneNumberError { get; set; } = "";
        public string PasswordError { get; set; } = "";

        private const string EmptyFirstNameErrorMsg = "Please enter your first name";
        private const string InvalidFirstNameErrorMsg = "Please enter a valid first name";
        private const string EmptyLastNameErrorMsg = "Please enter your last name";
        private const string InvalidLastNameErrorMsg = "Please enter a valid last name";
        private const string EmptyEmailErrorMsg = "Please enter your email";
        private const string InvalidEmailErrorMsg = "Please enter a valid email";
        private const string EmptyPhoneNumberErrorMsg = "Please enter your phone number";
        private const string InvalidPhoneNumberErrorMsg = "Please enter a valid phone number";
        private const string EmptyPasswordErrorMsg = "Please enter your password";
        private const string InvalidPasswordErrorMsg = "The password must be alphanumeric with a minimum length of 8 characters";

        public Task<bool> Validate(string firstName, string lastName, string email, string phonenumber, string password)
        {
            var isFirstNameValid = ValidateFirstName(firstName);
            var isLastNameValid = ValidateLastName(lastName);
            var isEmailValid = ValidateEmail(email);
            var isPhoneNumberValid = ValidatePhoneNumber(phonenumber);
            var isPasswordValid = ValidatePassword(password);

            return Task.FromResult(isFirstNameValid && isLastNameValid && isEmailValid && isPhoneNumberValid && isPasswordValid);
        }

        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                PasswordError = EmptyPasswordErrorMsg;
                return false;
            }

            if (password.Length < 8 || !Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
            {
                PasswordError = InvalidPasswordErrorMsg;
                return false;
            }

            PasswordError = "";
            return true;
        }

        private bool ValidatePhoneNumber(string phonenumber)
        {
            if (string.IsNullOrEmpty(phonenumber))
            {
                PhoneNumberError = EmptyPhoneNumberErrorMsg;
                return false;
            }

            if (phonenumber.Length < 9)
            {
                PhoneNumberError = InvalidPhoneNumberErrorMsg;
                return false;
            }

            PhoneNumberError = "";
            return true;
        }

        private bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                EmailError = EmptyEmailErrorMsg;
                return false;
            }

            if (!Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                EmailError = InvalidEmailErrorMsg;
                return false;
            }

            EmailError = "";
            return true;
        }

        private bool ValidateFirstName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                FirstNameError = EmptyFirstNameErrorMsg;
                return false;
            }

            if (lastName.Length < 3)
            {
                FirstNameError = InvalidFirstNameErrorMsg;
                return false;
            }

            FirstNameError = "";
            return true;
        }

        private bool ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
            {
                LastNameError = EmptyLastNameErrorMsg;
                return false;
            }

            if (lastName.Length < 3)
            {
                LastNameError = InvalidLastNameErrorMsg;
                return false;
            }

            LastNameError = "";
            return true;
        }
    }
}
