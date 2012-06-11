using System.Collections.Generic;

namespace LinqExpressionProjection.Test.Model
{
    public class Project
    {
        public int ID { get; set; }
        public virtual ICollection<Subproject> Subprojects { get; set; }
    }
}
