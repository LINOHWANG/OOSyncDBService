using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_TableTranModel
    {
        public int Id { get; set; }
        public string ParentTranId { get; set; }
        public string TranType { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string SecondName { get; set; }
        public int ProductTypeId { get; set; }
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
        public int Printer1Qty { get; set; }
        public int Printer2Qty { get; set; }
        public int Printer3Qty { get; set; }
        public int Printer4Qty { get; set; }
        public int Printer5Qty { get; set; }
        public string PromoStartDate { get; set; }
        public string PromoEndDate { get; set; }
        public int PromoDay1 { get; set; }
        public int PromoDay2 { get; set; }
        public int PromoDay3 { get; set; }
        public float PromoPrice1 { get; set; }
        public float PromoPrice2 { get; set; }
        public float PromoPrice3 { get; set; }
        public bool IsKitchenItem { get; set; }
        public bool IsSushiBarItem { get; set; }
        public string ManualName { get; set; }
        public float DCMethod { get; set; }
        public float Price { get; set; }
        public float Quantity { get; set; }
        public float Amount { get; set; }
        public float Tax1Rate { get; set; }
        public float Tax2Rate { get; set; }
        public float Tax3Rate { get; set; }
        public float Tax1 { get; set; }
        public float Tax2 { get; set; }
        public float Tax3 { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int SplitId { get; set; }
        public int OldTableId { get; set; }
        public int InvoiceNo { get; set; }
        public int NumInvPrt { get; set; }
        public string InvPrtDate { get; set; }
        public string InvPrtTime { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsAdditionalOrder { get; set; }
        public int OrderSeqNo { get; set; }
        public int OrderNumPeople { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string OrderPasswordCode { get; set; }
        public string OrderPasswordName { get; set; }
        public string OrderStation { get; set; }
        public int IsCancelled { get; set; }
        public string CancelDate { get; set; }
        public string CancelTime { get; set; }
        public bool IsCancelPending { get; set; }
        public string CancelPrintDate { get; set; }
        public string CancelPrintTime { get; set; }
        public bool IsOK { get; set; }
        public bool IsCooked { get; set; }
        public bool IsPicked { get; set; }
        public bool IsPaidStarted { get; set; }
        public string PaidType { get; set; }
        public int StartReceiptNo { get; set; }
        public string PaidStartDate { get; set; }
        public string PaidStartTime { get; set; }
        public bool IsPaidComplete { get; set; }
        public int CompleteReceiptNo { get; set; }
        public string CompleteDate { get; set; }
        public string CompleteTime { get; set; }
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
        public bool IsRounding { get; set; }
        public int SplitTranId { get; set; }
        public int SplitTranItemId { get; set; }
        public int SplitTranItemSplitId { get; set; }

    }
}
