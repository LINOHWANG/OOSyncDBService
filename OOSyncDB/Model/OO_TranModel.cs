using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB.Model
{
    class OO_TranModel
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string Phone { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Buzzer { get; set; }
        public string Memo { get; set; }
        public bool IsDelivery { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string PickUpDate { get; set; }
        public string PickUpTime { get; set; }
        public bool IsPaid { get; set; }
        public float Amount { get; set; }
        public float Tax1 { get; set; }
        public float Tax2 { get; set; }
        public float Tax3 { get; set; }
        public float TotalDue { get; set; }
        public float TotalPaid { get; set; }
        public int IsPOSUpdated { get; set; }
    }
}
