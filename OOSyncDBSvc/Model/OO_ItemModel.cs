using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_ItemModel
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string TransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SecondName { get; set; }
        public float UnitPrice { get; set; }
        public bool IsTax1 { get; set; }
        public bool IsTax2 { get; set; }
        public bool IsTax3 { get; set; }
        public bool IsTaxInverseCalculation { get; set; }
        public string PromoStartDate { get; set; }
        public string PromoEndDate { get; set; }
        public int PromoDay1 { get; set; }
        public int PromoDay2 { get; set; }
        public int PromoDay3 { get; set; }
        public float PromoPrice1 { get; set; }
        public float PromoPrice2 { get; set; }
        public float PromoPrice3 { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public float Amount { get; set; }
        public float Tax1Rate { get; set; }
        public float Tax2Rate { get; set; }
        public float Tax3Rate { get; set; }
        public float Tax1 { get; set; }
        public float Tax2 { get; set; }
        public float Tax3 { get; set; }
        public string Comment { get; set; }
        public int ParentId { get; set; }
    }
}
