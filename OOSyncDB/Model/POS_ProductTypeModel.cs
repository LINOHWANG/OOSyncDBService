using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB.Model
{
    class POS_ProductTypeModel
    {
        public int id { get; set; }
        public string TypeName { get; set; }
        public bool IsLiquor { get; set; }
        public bool SortOrder { get; set; }
        public bool IsRestaurant { get; set; }

    }
}
