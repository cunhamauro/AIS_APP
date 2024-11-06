namespace AIS_APP.Models
{
    public class UpdatePasswordModel
    {
        public string? CurrentPassword { get; set; }

        public string? NewPassword { get; set; }

        public string? Confirm { get; set; }
    }
}
