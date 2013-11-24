using Core.Domains;

namespace PTS.Models
{
    public class AccountUser
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal ClassRate { get; set; }
        public string Major {get;set;}
        public string Education {get;set;}
        public string PassWord { get; set; }
        public LocationVM Location { get; set; }
        public UserRole Role { get; set; }


        public AccountUser()
        {
            
        }

    }
}