using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_OO_Prod_SyncModel
    {
        public int id { get; set; }
        public int prodid { get; set; }
        public string Activity { get; set; }
        public string DoneBy { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public DateTime DateTimeSync { get; set; }
        public int SyncFlag { get; set; }
    }
}
