using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOSyncDBSvc.Model;
using Dapper;
using System.Data.SqlClient;

namespace OOSyncDBSvc
{
    static class Constants
    {
        public const int i_OnlineOrder_TableId_Start = 165;    // CON_ONLINE_ORDER_START
        public const int i_OnlineOrder_TableId_End = 500;      // CON_ONLINE_ORDER_END
    }
    class DataAccessPOS
    {
        Utilities util = new Utilities();
        public List<POS_PassWordModel> UserLogin(string passWord)
        {
            ////////////////////////////////////////////////////
            // To connect to SQL Server
            // Open the door to the database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                try
                {
                    var output = connection.Query<POS_PassWordModel>($"select passCode,passGrade,passName from mfPassWord where passCode = '{passWord}'").ToList();
                    return output;
                }
                catch (System.Data.SqlTypes.SqlTypeException ex)
                {
                    util.Logger(ex.Message);
                }
                catch (SqlException ex)
                {
                    util.Logger(ex.Message);
                }
                catch (Exception ex)
                {
                    util.Logger(ex.Message);
                }
                return null;
            }
        }
        public List<POS_OO_Prod_SyncModel> Get_OO_Prod_Sync_By_Flag(int iSyncFlag)
        {


            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                try
                {
                    var output = connection.Query<POS_OO_Prod_SyncModel>($"select * from OO_Prod_Sync where SyncFlag = {iSyncFlag} order by datetimecreated").ToList();
                return output;
                }
                catch (System.Data.SqlTypes.SqlTypeException ex)
                {
                    util.Logger(ex.Message);
                }
                catch (SqlException ex)
                {
                    util.Logger(ex.Message);
                }
                catch (Exception ex)
                {
                    util.Logger(ex.Message);
                }
                return null;
            }

        }
        public int Delete_OO_Prod_Sync()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                DateTime twb_today = DateTime.Today;
                twb_today.AddDays(-14);
                string dtWeekago = twb_today.ToString("yyyy-MM-dd HH:mm.ss");
                string query = "DELETE OO_Prod_Sync WHERE prodid = 1 and datetimecreated < '" + dtWeekago + "'";
                var count = connection.Execute(query);
                return count;
            }
        }
        public List<POS_ProductModel> Get_ALL_Products()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductModel>($"select * from Product where IsOnline=1").ToList();
                return output;
            }
        }
        public List<POS_ProductModel> Get_Product_By_ID(int prodID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductModel>($"select * from Product where id = {prodID}").ToList();
                return output;
            }
        }

        public List<POS_ProductTypeModel> Get_ALL_ProductTypes()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductTypeModel>($"select * from ProductType").ToList();
                return output;
            }
        }
        public List<POS_TaxModel> Get_All_Tax()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_TaxModel>($"select * from Tax").ToList();
                return output;
            }
        }
        public int Update_Product_Sync_Table(int syncid, int syncstat)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE OO_Prod_Sync SET SyncFlag = " + syncstat.ToString() + " WHERE id =" + syncid.ToString();
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Update_Product_Synced(int syncid, int syncstat)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE Product SET IsOOUpdated = 1 WHERE id =" + syncid.ToString();
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Insert_Customer(POS_CustomerModel pos_Customer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO Customer (FirstName,LastName,Phone,Address1,Address2,Zip,DateMarried,DateOfBirth,Memo,WebId,Email) values (" +
                                "@FirstName,@LastName,@Phone,@Address1,@Address2,@Zip,@DateMarried,@DateOfBirth,@Memo,@WebId,@Email)";
                var count = connection.Execute(query, pos_Customer);
                return count;
            }
        }
        public int Update_Customer(POS_CustomerModel pos_Customer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE Customer Set FirstName=@FirstName,LastName=@LastName,Phone=@Phone,Address1=@Address1,Address2=@Address2, " +
                                    "Zip=@Zip,DateMarried=@DateMarried,DateOfBirth=@DateOfBirth,Memo=@Memo,WebId=@WebId, Email=@Email WHERE id = @id ";
                var count = connection.Execute(query, pos_Customer);
                return count;
            }
        }
        public int Update_Customer_by_WebId(POS_CustomerModel pos_Customer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE Customer Set FirstName=@FirstName,LastName=@LastName,Phone=@Phone,Address1=@Address1,Address2=@Address2, " +
                                    "Zip=@Zip,DateMarried=@DateMarried,DateOfBirth=@DateOfBirth,Memo=@Memo,Email=@Email WHERE WebId = @WebId ";
                var count = connection.Execute(query, pos_Customer);
                return count;
            }
        }
        
        public List<POS_CustomerModel> Get_Customer_by_PhoneName(string strPhoneNo, string strLastName)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_CustomerModel>($"select * from Customer where Phone = '{strPhoneNo}' and LastName = '{strLastName}' ").ToList();
                return output;
            }
        }
        public List<POS_CustomerModel> Get_Customer_by_WebId(int iWebId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_CustomerModel>($"select * from Customer where WebId = {iWebId}").ToList();
                return output;
            }
        }
        public List<POS_OnlineOrderModel> Get_OnlineOrder_by_ooTranID(int iooTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_OnlineOrderModel>($"select * from OnlineOrder where oo_tranId = {iooTranID.ToString()}").ToList();
                return output;
            }
        }

        internal List<POS_OnlineOrderModel> Get_OnlineOrder_by_InvNo_CustID(int iInvNo, int iCustomerId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_OnlineOrderModel>($"select * from OnlineOrder where invoiceNo = {iInvNo.ToString()} and customerId = {iCustomerId.ToString()}").ToList();
                return output;
            }
        }

        internal int Insert_OnlineOrder(POS_OnlineOrderModel oOOrder)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO OnlineOrder (invoiceNo,customerId,oo_tranId,oo_OrderDate,oo_OrderTime,oo_PickupDate,oo_PickupTime,oo_IsDelivered,oo_IsPaid, " +
                                "oo_Amount,oo_Tax1,oo_Tax2,oo_Tax3,oo_TotalDue,oo_TotalPaid,CreatedDttm,IsOOUpdated,TableId) values (" +
                                "@invoiceNo,@customerId,@oo_tranId,@oo_OrderDate,@oo_OrderTime,@oo_PickupDate,@oo_PickupTime,@oo_IsDelivered, @oo_IsPaid," +
                                "CAST(@oo_Amount as decimal(10,2)),CAST(@oo_Tax1 as decimal(10,2)),CAST(@oo_Tax2 as decimal(10,2)), " +
                                "CAST(@oo_Tax3 as decimal(10,2)),CAST(@oo_TotalDue as decimal(10,2)),CAST(@oo_TotalPaid as decimal(10,2)),GETDATE(),@IsOOUpdated,@TableId)";
                var count = connection.Execute(query, oOOrder);
                return count;
            }
        }

        internal List<POS_TableTranModel> Get_TableTran_by_InvNo_ProdId(int iInvNo, int iProdid)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_TableTranModel>($"select * from TableTran where invoiceNo = {iInvNo.ToString()} and ProductId = {iProdid.ToString()}").ToList();
                return output;
            }
        }

        internal int Insert_TableTran(POS_TableTranModel pos_Tran)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO TableTran (ParentTranId,TranType,ProductId,ProductName,SecondName,ProductTypeId, " +
                                "InUnitPrice,OutUnitPrice,IsTax1,IsTax2,IsTax3,IsTaxInverseCalculation,IsPrinter1,IsPrinter2,IsPrinter3,IsPrinter4," +
                                "IsPrinter5,Printer1Qty,Printer2Qty,Printer3Qty,Printer4Qty,Printer5Qty,PromoStartDate,PromoEndDate, " +
                                "PromoDay1,PromoDay2,PromoDay3,PromoPrice1,PromoPrice2,PromoPrice3,IsKitchenItem,IsSushiBarItem,ManualName, " +
                                "DCMethod,Price,Quantity,Amount,Tax1Rate,Tax2Rate,Tax3Rate,Tax1,Tax2,Tax3,TableId,TableName,SplitId, " +
                                "OldTableId,InvoiceNo,NumInvPrt,InvPrtDate,InvPrtTime,IsOrdered,IsAdditionalOrder,OrderSeqNo,OrderNumPeople," +
                                "OrderDate,OrderTime,OrderPasswordCode,OrderPasswordName,OrderStation,IsCancelled,CancelDate,CancelTime, " +
                                "IsCancelPending,CancelPrintDate,CancelPrintTime,IsOK,IsCooked,IsPicked,IsPaidStarted,PaidType, " +
                                "StartReceiptNo,PaidStartDate,PaidStartTime,IsPaidComplete,CompleteReceiptNo,CompleteDate,CompleteTime,CreateDate,CreateTime," +
                                "CreatePasswordCode,CreatePasswordName,CreateStation,LastModDate,LastModTime,LastModPasswordCode,LastModPasswordName," +
                                "LastModStation,IsRounding,SplitTranId,SplitTranItemId,SplitTranItemSplitId " +
                                ") values (" +
                                "@ParentTranId,@TranType,@ProductId,@ProductName,@SecondName,@ProductTypeId," +
                                "CAST(@InUnitPrice as decimal(10,2)),CAST(@OutUnitPrice as decimal(10,2)),@IsTax1,@IsTax2,@IsTax3,@IsTaxInverseCalculation,@IsPrinter1,@IsPrinter2,@IsPrinter3,@IsPrinter4," +
                                "@IsPrinter5,@Printer1Qty,@Printer2Qty,@Printer3Qty,@Printer4Qty,@Printer5Qty,@PromoStartDate,@PromoEndDate," +
                                "@PromoDay1,@PromoDay2,@PromoDay3,@PromoPrice1,@PromoPrice2,@PromoPrice3,@IsKitchenItem,@IsSushiBarItem,@ManualName, " +
                                "@DCMethod,CAST(@Price as decimal(10,2)),@Quantity,CAST(@Amount as decimal(10,2)),CAST(@Tax1Rate as decimal(10,2)), " + 
                                "CAST(@Tax2Rate as decimal(10,2)),CAST(@Tax3Rate as decimal(10,2)),CAST(@Tax1 as decimal(10,2)),CAST(@Tax2 as decimal(10,2)), " +
                                "CAST(@Tax3 as decimal(10,2)),@TableId,@TableName,@SplitId, " +
                                "@OldTableId,@InvoiceNo,@NumInvPrt,@InvPrtDate,@InvPrtTime,@IsOrdered,@IsAdditionalOrder,@OrderSeqNo,@OrderNumPeople," +
                                "@OrderDate,@OrderTime,@OrderPasswordCode,@OrderPasswordName,@OrderStation,@IsCancelled,@CancelDate,@CancelTime," +
                                "@IsCancelPending,@CancelPrintDate,@CancelPrintTime,@IsOK,@IsCooked,@IsPicked,@IsPaidStarted,@PaidType, " +
                                "@StartReceiptNo,@PaidStartDate,@PaidStartTime,@IsPaidComplete,@CompleteReceiptNo,@CompleteDate,@CompleteTime,@CreateDate,@CreateTime," +
                                "@CreatePasswordCode,@CreatePasswordName,@CreateStation,@LastModDate,@LastModTime,@LastModPasswordCode,@LastModPasswordName," +
                                "@LastModStation,@IsRounding,@SplitTranId,@SplitTranItemId,@SplitTranItemSplitId" +
                                ")";
                var count = connection.Execute(query, pos_Tran);
                return count;
            }
        }

        internal int Update_SysConfig_OnlineStatus(string strOOStatus)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE sysConfig SET config_value = '" + strOOStatus + "' , config_desc = convert(varchar, getdate(), 21) where config_name = 'OO_STATUS'";
                var count = connection.Execute(query);
                return count;
            }
        }

        internal List<POS_SysConfigModel> Get_All_SysConfig()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_SysConfigModel>($"select * from sysconfig where config_oosync = 1").ToList();
                return output;
            }
        }
        internal List<POS_SysConfigModel> Get_SysConfig_By_Name(string strConfigName)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_SysConfigModel>($"select * from sysconfig where config_oosync = 1 and config_name ='{strConfigName}'").ToList();
                return output;
            }
        }

        internal List<POS_ProductModel> Get_ALL_Products_To_Sync()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductModel>($"select * from Product where IsOnline=1 And IsOOUpdated=0").ToList();
                return output;
            }
        }

        internal List<POS_ChildGroupModel> Get_All_ChildGroups_To_Sync(int p_intParentProdId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = $"select * from ChildGroup where ParentProdId=" + p_intParentProdId.ToString();
                var output = connection.Query<POS_ChildGroupModel>(query).ToList();
                return output;
            }
        }

        internal int Get_Empty_OnlineOrder_TableId()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.ExecuteScalar<int>($"select max(tableid) from OnlineOrder");
                int iEmptyTableId = Convert.ToInt32(output);
                if (iEmptyTableId < Constants.i_OnlineOrder_TableId_Start)
                {
                    iEmptyTableId = Constants.i_OnlineOrder_TableId_Start;
                }
                else if (iEmptyTableId > Constants.i_OnlineOrder_TableId_End)
                {
                    return -1;
                }
                else
                { 
                    iEmptyTableId++;
                }
                return iEmptyTableId;
            }
        }

        internal List<POS_ProductDetailModel> Get_All_ProductDetail()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductDetailModel>($"select * from ProductDetail").ToList();
                return output;
            }
        }

        internal List<POS_ChildProdModel> Get_All_ChildProd_By_ID(int p_intChildGroupId, int p_intParentProdId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = $"select * from ChildProd where ParentProdId=" + p_intParentProdId.ToString() + " And ChildGroupId = " + p_intChildGroupId.ToString();
                var output = connection.Query<POS_ChildProdModel>(query).ToList();
                return output;
            }
        }
    }
}
