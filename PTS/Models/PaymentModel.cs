using System;
using Core.Domains;

namespace PTS.Models {
    public class PaymentModel {
        public Payment Payment { get; set; }
        public Location Location { get; set; }
        public String Message { get; set; }
    }

}