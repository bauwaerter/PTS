using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domains {
    public abstract partial class BaseEntity {
        public virtual int Id { get; set; }
    }
}
