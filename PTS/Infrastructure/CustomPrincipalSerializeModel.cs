using Core;
using Core.Domains;

namespace PTS.Infrastructure
{
    public class CustomPrincipalSerializeModel
    {
        public int Id { get; set; }
        public UserRoles Role { get; set; }
        public bool IsFirstLogin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CustomerId { get; set; }

        public CustomPrincipalSerializeModel()
        {
        }

        public CustomPrincipalSerializeModel(User user)
        {
            Id = user.Id;
            //Role = user.Role;
            //IsFirstLogin = !user.LastLogin.HasValue;
            //FirstName = user.FirstName;
            //LastName = user.LastName;
            //CustomerId = user.CustomerId.GetValueOrDefault(0);
        }
    }
}