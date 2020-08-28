using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_ProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string SecondName { get; set; }
        public int ProductTypeId { get; set; }
        public bool IsSubItem { get; set; }
        public float InUnitPrice { get; set; }
        public float OutUnitPrice { get; set; }
        public bool IsTax1 { get; set; }
        public bool IsTax2 { get; set; }
        public bool IsTax3 { get; set; }
        public bool IsTaxInverseCalculation { get; set; }
        public bool IsPrinter1 { get; set; }
        public bool IsPrinter2 { get; set; }
        public bool IsPrinter3 { get; set; }
        public bool IsPrinter4 { get; set; }
        public bool IsPrinter5 { get; set; }
        public string PromoStartDate { get; set; }
        public string PromoEndDate { get; set; }
        public int PromoDay1 { get; set; }
        public int PromoDay2 { get; set; }
        public int PromoDay3 { get; set; }
        public float PromoPrice1 { get; set; }
        public float PromoPrice2 { get; set; }
        public float PromoPrice3 { get; set; }
        public bool IsSoldOut { get; set; }

        // 20200731
        public bool IsKitchenItem { get; set; }
        public bool IsSushiBarItem { get; set; }
        public int SpiceLevel { get; set; }
        public bool IsHappyHourItem { get; set; }
        public string AvailStartTime { get; set; }
        public string AvailEndTime { get; set; }
        public bool IsOnline { get; set; }
        // 20200804
        public bool IsOOUpdated { get; set; }

    }
}
