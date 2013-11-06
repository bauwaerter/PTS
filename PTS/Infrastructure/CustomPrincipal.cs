using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using Core;

namespace PTS.Infrastructure
{
    [Serializable]
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
	    public bool IsInRole(string role) { return false; }
	    public int Id { get; set; }
	    public int CustomerId { get; set; }
	    public UserRoles Role { get; set; }
	    public bool IsFirstLogin { get; set; }
	    public string FirstName { get; set; }
	    public string LastName { get; set; }
	
        public CustomPrincipal(string userName)
	    {	
            Identity = new GenericIdentity(userName);
        }
	
	    public CustomPrincipal(string userName, CustomPrincipalSerializeModel model)	
        {	
            Identity = new GenericIdentity(userName);
	        Id = model.Id;
	        Role = model.Role;
	        IsFirstLogin = model.IsFirstLogin;
	        FirstName = model.FirstName;
	        LastName = model.LastName;
	        CustomerId = model.CustomerId;
        }
	
        public bool IsInRole(params UserRoles[] userRoles)
	    {
	          return userRoles.Contains(Role);
	     }

    }
}