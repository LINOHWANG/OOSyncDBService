using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB.Model
{
    class OO_ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SecondName { get; set; }
        public int ProductTypeId { get; set; }
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
        public bool IsSoldOut { get; set; }
        public string SyncDate { get; set; }
    }
}
