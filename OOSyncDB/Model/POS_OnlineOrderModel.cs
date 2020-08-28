using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB.Model
{
    class POS_OnlineOrderModel
    {
        public int id { get; set; }
        public int invoiceNo { get; set; }
        public int customerId { get; set; }
        public int oo_tranId { get; set; }
        public string oo_OrderDate { get; set; }
        public string oo_OrderTime { get; set; }
        public string oo_PickupDate { get; set; }
        public string oo_PickupTime { get; set; }
        public bool oo_IsDelivered { get; set; }
        public bool oo_IsPaid { get; set; }
        public float oo_Amount { get; set; }
        public float oo_Tax1 { get; set; }
        public float oo_Tax2 { get; set; }
        public float oo_Tax3 { get; set; }
        public float oo_TotalDue { get; set; }
        public float oo_TotalPaid { get; set; }
        public DateTime CreatedDttm { get; set; }
        public bool IsOOUpdated { get; set; }
        public DateTime OOUpdatedDttm { get; set; }

    }
}
