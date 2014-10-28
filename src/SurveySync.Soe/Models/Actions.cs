using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SurveySync.Soe.Models {

    public class Actions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Actions()
        {
            Update = new Collection<Schema>();
            Create = new Collection<Schema>();
        }

        public IEnumerable<Schema> Update { get; set; }
        public IEnumerable<Schema> Create { get; set; }

        public int Total { get { return Update.Count() + Create.Count(); } }      
    }

}