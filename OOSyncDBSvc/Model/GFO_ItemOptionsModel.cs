using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GFO_ItemOptionsModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string group_name { get; set; }
        public string type { get; set; }
        public int type_id { get; set; }
        public int quantity { get; set; }
        public float price { get; set; }
    }
}
