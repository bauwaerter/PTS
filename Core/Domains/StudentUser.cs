using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Domains
{
    public class StudentUser : BaseEntity
    {
        public virtual string Major { get; set; }
        public virtual string Education { get; set; }

        #region navigation properties

        //private ICollection<Class> _classes;
        //public virtual ICollection<Class> Classes
        //{
        //    get {return _classes ?? (_classes = new List<Class>());}
        //    protected set {_classes = value;}
        //}

        #endregion

    }
} 
