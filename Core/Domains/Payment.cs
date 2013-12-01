using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace Core.Domains
{
    public class Payment : BaseEntity
    {
        public virtual int StudentId { get; set; }
        public virtual int TeacherId { get; set; }
        public virtual int? ClassId { get; set; }
        public virtual double Amount { get; set; }
        public virtual string Description { get; set; }
        public virtual int CreditCardNumber { get; set; }
        public virtual DateTime Date { get; set; }
    }
}
