using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GFO_CollectionsModel
    {
        public int count { get; set; }
        public List<GFO_OrdersModel> orders { get; set; }
    }
}
