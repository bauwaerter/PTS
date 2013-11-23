using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTS.Models
{
    public class LocationVM
    {
        public string Address {get;set;}
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; set; }
        public int id { get; set; }

        public LocationVM()
        {

        }

    } 
}