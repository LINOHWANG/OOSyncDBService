using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_PhoneOrderModel
    {
        public int TableId { get; set; }
        public string OrderDate { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public string PickDate { get; set; }
        public string PickTime { get; set; }
        public int CustomerId { get; set; }
        public string Memo { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreatePasswordCode { get; set; }
        public string CreatePasswordName { get; set; }
        public string CreateStation { get; set; }
        public string LastModDate { get; set; }
        public string LastModTime { get; set; }
        public string LastModPasswordCode { get; set; }
        public string LastModPasswordName { get; set; }
        public string LastModStation { get; set; }

    }
}
