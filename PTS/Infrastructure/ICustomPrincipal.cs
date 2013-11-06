using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace PTS.Infrastructure
{
    internal interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        //UserRoles Role { get; set; }
        bool IsFirstLogin { get; set; }
        //bool IsInRole(params UserRoles[] userRoles);
    }
}