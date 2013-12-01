using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Core.Helpers;

namespace Core
{
    #region Users
	
    public enum UserRoles
    {
        Admin = 3,
	    Tutor = 2,
        Student = 1,
        None = 0
    }

    public enum CardType {
        [Display(Name = "Visa")]
        Visa,

        [Display(Name = "Master Card")]
        MasterCard,

        [Display(Name = "American Express")]
        AmericanExpress,

        [Display(Name = "Discover")]
        Discover
    }

    #endregion
}
