using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOSyncDBSvc.Model;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;

namespace OOSyncDBSvc
{
    static class Constants
    {
        public const int i_OnlineOrder_TableId_Start = 165;    // CON_ONLINE_ORDER_START
        public const int i_OnlineOrder_TableId_End = 184;      // CON_ONLINE_ORDER_END
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
                                "@FirstName,@LastName,@Phone,@Address1,@Address2,@Zip,@DateMarried,@DateOfBirth,@Memo,@WebId,@Email); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Customer);
                int result = connection.QuerySingle<int>(query, pos_Customer);
                return result;
            }
        }
        public int Update_Customer(POS_CustomerModel pos_Customer)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE Customer Set FirstName=@FirstName,LastName=@LastName,Phone=@Phone,Address1=@Address1,Address2=@Address2, " +
                                    "Zip=@Zip,DateMarried=@DateMarried,DateOfBirth=@DateOfBirth,Memo=@Memo,WebId=@WebId, Email=@Email WHERE id = @id; ";
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
        public List<POS_CustomerModel> Get_Customer_by_PhoneNo(string strPhoneNo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_CustomerModel>($"select * from Customer where Phone = '{strPhoneNo}' ").ToList();
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
        public List<POS_PhoneOrderModel> Get_OnlineOrder_by_ooTranID(int iooTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_PhoneOrderModel>($"select * from OnlineOrder where oo_tranId = {iooTranID.ToString()}").ToList();
                return output;
            }
        }

        internal List<POS_PhoneOrderModel> Get_OnlineOrder_by_InvNo_CustID(int iInvNo, int iCustomerId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_PhoneOrderModel>($"select * from OnlineOrder where invoiceNo = {iInvNo.ToString()} and customerId = {iCustomerId.ToString()}").ToList();
                return output;
            }
        }

        internal int Insert_PhoneOrder(POS_PhoneOrderModel pPhoneOrder)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO PhoneOrder (TableId,OrderDate,Phone,CustomerName,PickDate,PickTime,CustomerId,Memo, " +
                                "CreateDate,CreateTime,CreatePasswordCode,CreatePasswordName,CreateStation, " +
                                "LastModDate,LastModTime,LastModPasswordCode,LastModPasswordName,LastModStation " +
                                ") values (" +
                                "@TableId, @OrderDate, @Phone, @CustomerName, @PickDate, @PickTime, @CustomerId, @Memo, " +
                                "@CreateDate,@CreateTime,@CreatePasswordCode,@CreatePasswordName,@CreateStation, " +
                                "@LastModDate,@LastModTime,@LastModPasswordCode,@LastModPasswordName,@LastModStation " +
                                "); ";
                                //"SELECT CAST(SCOPE_IDENTITY() as int)";
                var count = connection.Execute(query, pPhoneOrder);
                //int result = connection.QuerySingle<int>(query, pPhoneOrder);
                //return Task.FromResult(0);
                //return result;
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
                                "LastModStation,IsRounding,SplitTranId,SplitTranItemId,SplitTranItemSplitId, " +
                                "IsPrinter6, Printer6QTY " +
                                ") values (" +
                                "@ParentTranId,@TranType,@ProductId,SUBSTRING(@ProductName,1,30),SUBSTRING(@SecondName,1,30),@ProductTypeId," +
                                "CAST(@InUnitPrice as decimal(10,2)),CAST(@OutUnitPrice as decimal(10,2)),@IsTax1,@IsTax2,@IsTax3,@IsTaxInverseCalculation,@IsPrinter1,@IsPrinter2,@IsPrinter3,@IsPrinter4," +
                                "@IsPrinter5,@Printer1Qty,@Printer2Qty,@Printer3Qty,@Printer4Qty,@Printer5Qty,@PromoStartDate,@PromoEndDate," +
                                "@PromoDay1,@PromoDay2,@PromoDay3,@PromoPrice1,@PromoPrice2,@PromoPrice3,@IsKitchenItem,@IsSushiBarItem,@ManualName, " +
                                "CAST(@DCMethod as decimal(10,2)),CAST(@Price as decimal(10,2)),@Quantity,CAST(@Amount as decimal(10,2)),CAST(@Tax1Rate as decimal(10,2)), " + 
                                "CAST(@Tax2Rate as decimal(10,2)),CAST(@Tax3Rate as decimal(10,2)),CAST(@Tax1 as decimal(10,2)),CAST(@Tax2 as decimal(10,2)), " +
                                "CAST(@Tax3 as decimal(10,2)),@TableId,@TableName,@SplitId, " +
                                "@OldTableId,@InvoiceNo,@NumInvPrt,@InvPrtDate,@InvPrtTime,@IsOrdered,@IsAdditionalOrder,@OrderSeqNo,@OrderNumPeople," +
                                "@OrderDate,@OrderTime,@OrderPasswordCode,@OrderPasswordName,@OrderStation,@IsCancelled,@CancelDate,@CancelTime," +
                                "@IsCancelPending,@CancelPrintDate,@CancelPrintTime,@IsOK,@IsCooked,@IsPicked,@IsPaidStarted,@PaidType, " +
                                "@StartReceiptNo,@PaidStartDate,@PaidStartTime,@IsPaidComplete,@CompleteReceiptNo,@CompleteDate,@CompleteTime,@CreateDate,@CreateTime," +
                                "@CreatePasswordCode,@CreatePasswordName,@CreateStation,@LastModDate,@LastModTime,@LastModPasswordCode,@LastModPasswordName," +
                                "@LastModStation,@IsRounding,@SplitTranId,@SplitTranItemId,@SplitTranItemSplitId," +
                                "@IsPrinter6, @Printer6QTY " +
                                "); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Tran);
                //return count;
                int result = connection.QuerySingle<int>(query, pos_Tran);
                //return Task.FromResult(0);
                return result;
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
                string query = $"select * from sysconfig where config_oosync = 1 and config_name like '%{strConfigName}%'";
                var output = connection.Query<POS_SysConfigModel>(query).ToList();
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

        internal int Get_Empty_PhoneOrder_TableId()
        {
            int intTableIdStart = Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]);
            int intTableIdEnd = Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_END"]);

            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                //string query = $"SELECT max(tableid) from PhoneOrder WHERE tableid >=" + intTableIdStart.ToString() + " And tableid <= " + intTableIdEnd.ToString() +";";
                string query = $"exec get_free_online_tableid " + intTableIdStart.ToString() + "," + intTableIdEnd.ToString() + ";";
                var output = connection.ExecuteScalar<int>(query);
                int iEmptyTableId = Convert.ToInt32(output);
                
                util.Logger("Get_Empty_PhoneOrder_TableId = " + query + " : " + output.ToString());

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
        public List<POS_GF_MenuItemsModel> Get_GF_MenuItems_ById(int p_intId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_GF_MenuItemsModel>($"select * from GF_MenuItems where Id = {p_intId}").ToList();
                return output;
            }
        }
        public int Insert_GF_MenuItems(POS_GF_MenuItemsModel pos_GF_MenuItems)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                //                string query = "Insert INTO GF_MenuItems (Id,Menu_Category_Id,Name, Description,Price,Active,ParentId,SubType,SubName,Match_POS_Id,Match_DTTM) values (" +
                //                                "@Id,@Menu_Category_Id,@Name, @Description,@Price,@Active,@ParentId,@SubType,@SubName,@Match_POS_Id,@Match_DTTM, @CategoryName)";
                //Feature #3234
                string query = "Insert INTO GF_MenuItems (Id,Menu_Category_Id,Name, Description,Price,Active,ParentId,SubType,SubName," + 
                                "Match_POS_Id, Match_DTTM, CategoryName) values (" +
                                "@Id,@Menu_Category_Id,@Name, @Description,CAST(@Price as decimal(10,2)),@Active,@ParentId,@SubType,@SubName," + 
                                "@Match_POS_Id, @Match_DTTM, @CategoryName)";

                var count = connection.Execute(query, pos_GF_MenuItems);
                return count;
            }
        }
        public int Update_GF_MenuItems(POS_GF_MenuItemsModel pos_GF_MenuItems, bool p_blnMatched)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                //                string query = "UPDATE GF_MenuItems Set Menu_Category_Id=@Menu_Category_Id,Name=@Name, Description=@Description,Price=@Price,Active=@Active, " +
                //                                    "ParentId=@ParentId,SubType=@SubType,SubName=@SubName,Match_POS_Id=@Match_POS_Id,Match_DTTM=@Match_DTTM " +
                //                                    "WHERE id = @id ";
                //Feature #3234
                string query = "";
                if (p_blnMatched)
                {
                    query = "UPDATE GF_MenuItems Set Menu_Category_Id=@Menu_Category_Id,Name=@Name, Description=@Description,Price=CAST(@Price as decimal(10,2))," +
                                    "Active=@Active, ParentId=@ParentId,SubType=@SubType,SubName=@SubName, " +
                                    "CategoryName=@CategoryName, Match_POS_Id=@Match_POS_Id, Match_DTTM=@Match_DTTM " +
                                    "WHERE id = @id ";
                }
                else
                {
                    query = "UPDATE GF_MenuItems Set Menu_Category_Id=@Menu_Category_Id,Name=@Name, Description=@Description,Price=CAST(@Price as decimal(10,2))," +
                                    "Active=@Active, ParentId=@ParentId,SubType=@SubType,SubName=@SubName, " +
                                    "CategoryName=@CategoryName " +
                                    "WHERE id = @id ";
                }

                var count = connection.Execute(query, pos_GF_MenuItems);
                return count;
            }
        }

        internal List<POS_ProductModel> Get_Product_By_GFTypeID(int? type_id)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = $"select * from Product where id in (Select Match_POS_id From GF_MenuItems Where id=" + type_id.ToString() + ");";
                var output = connection.Query<POS_ProductModel>(query).ToList();
                return output;
            }
        }

        internal POS_TaxModel Get_Tax_By_TaxCode(string taxCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "";
                if (string.IsNullOrEmpty(taxCode))
                {
                    query = $"select * from Tax WHERE Code = 'TAX';";
                }
                else
                {
                    query = $"select * from Tax WHERE Code = '" + taxCode +"';";
                }
                var output = connection.Query<POS_TaxModel>(query).ToList();
                return output[0];
            }
        }

        internal POS_SeqNoModel Get_SeqNo()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                POS_SeqNoModel posSeqNo = new POS_SeqNoModel();
                string query = "";
                query = $"Select * From mfSeqNo WHERE Code = 'SEQNO' And " + 
                        "convert(varchar(10), seqDate, 102) = convert(varchar(10), getdate(), 102); ";
                var output = connection.Query<POS_SeqNoModel>(query).ToList();
                if (output.Count > 0)
                {
                    return output[0];
                }
                else
                {
                    return posSeqNo;
                }
            }
        }

        internal int Update_SeqNo(int iNewSeqNo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "UPDATE mfSeqNo Set seqNo=" + iNewSeqNo +
                                ", seqDate = convert(varchar(10), getdate(), 102) " +
                                ", seqTime = convert(varchar(8), getdate(), 114) " +
                                "WHERE Code = 'SEQNO' ";

                var count = connection.Execute(query);
                return count;
            }
        }

        public List<POS_ProductModel> Get_Product_By_Name(string strPName)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_ProductModel>($"select * from Product where UPPER(ProductName) = UPPER('{strPName}')").ToList();
                return output;
            }
        }
        internal List<POS_ProductModel> Get_Product_OnlineOpenFood()
        {
            POS_ProductModel pProduct = new POS_ProductModel();
            POS_ProductTypeModel pType = new POS_ProductTypeModel();

            List<POS_ProductModel> pProducts = Get_Product_By_Name("ONLINE");

            if (pProducts.Count == 0)
            {
                pType.TypeName = "ONLINE";
                pType.IsLiquor = false;
                pType.IsRestaurant = false;

                //pProduct = pProducts[0];
                pProduct.ProductTypeId = Insert_ProductType(pType);
                pProduct.ProductName = "ONLINE";
                pProduct.IsSubItem = false;
                pProduct.IsManualItem = true;
                pProduct.InUnitPrice = 0;
                pProduct.OutUnitPrice = 0;
                pProduct.Balance = 0;
                pProduct.IsTax1 = true;
                pProduct.IsTax2 = false;
                pProduct.IsTax3 = false;
                pProduct.IsTaxInverseCalculation = false;
                pProduct.IsPrinter1 = true;
                pProduct.IsPrinter2 = true;
                pProduct.IsPrinter3 = true;
                pProduct.IsPrinter4 = false;
                pProduct.IsPrinter5 = false;
                pProduct.IsPrinter6 = false;
                pProduct.PromoStartDate = "2000-01-01";
                pProduct.PromoEndDate = "2000-01-01";
                pProduct.PromoDay1 = 0;
                pProduct.PromoDay2 = 0;
                pProduct.PromoDay3 = 0;
                pProduct.PromoPrice1 = 0;
                pProduct.PromoPrice2 = 0;
                pProduct.PromoPrice3 = 0;
                pProduct.IsSoldOut = false;
                pProduct.IsKitchenItem = false;
                pProduct.IsSushiBarItem = false;
                pProduct.SpiceLevel = 0;
                pProduct.IsHappyHourItem = false;
                pProduct.IsOnline = false;
                pProduct.IsOOUpdated = false;
                int intProductId = Insert_Product(pProduct);
                pProducts = Get_Product_By_ID(intProductId);
            }
            
            return pProducts;
        }

        private int Insert_Product(POS_ProductModel pProduct)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO Product (ProductName, ProductTypeId, IsSubItem, IsManualItem, " +
                                "InUnitPrice,OutUnitPrice,Balance,IsTax1,IsTax2,IsTax3,IsTaxInverseCalculation, " +
                                "IsPrinter1,IsPrinter2,IsPrinter3,IsPrinter4,IsPrinter5,IsPrinter6," +
                                "PromoStartDate,PromoEndDate,PromoDay1,PromoDay2,PromoDay3,PromoPrice1,PromoPrice2,PromoPrice3, " +
                                "IsSoldOut,IsKitchenItem,IsSushiBarItem,SpiceLevel,IsHappyHourItem,IsOnline,IsOOUpdated " +
                                ") values (" +
                                "@ProductName, @ProductTypeId, @IsSubItem, @IsManualItem, " +
                                "@InUnitPrice,@OutUnitPrice,@Balance,@IsTax1,@IsTax2,@IsTax3,@IsTaxInverseCalculation, " +
                                "@IsPrinter1,@IsPrinter2,@IsPrinter3,@IsPrinter4,@IsPrinter5,@IsPrinter6," +
                                "@PromoStartDate,@PromoEndDate,@PromoDay1,@PromoDay2,@PromoDay3,@PromoPrice1,@PromoPrice2,@PromoPrice3, " +
                                "@IsSoldOut,@IsKitchenItem,@IsSushiBarItem,@SpiceLevel,@IsHappyHourItem,@IsOnline,@IsOOUpdated " +
                                "); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Tran);
                //return count;
                int result = connection.QuerySingle<int>(query, pProduct);
                //return Task.FromResult(0);
                return result;
            }
        }

        private int Insert_ProductType(POS_ProductTypeModel pType)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO ProductType (TypeName, IsLiquor, SortOrder, IsRestaurant " +
                                ") values (" +
                                "@TypeName, @IsLiquor, @SortOrder, @IsRestaurant  " +
                                "); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Tran);
                //return count;
                int result = connection.QuerySingle<int>(query, pType);
                //return Task.FromResult(0);
                return result;
            }
        }

        internal bool Check_Online_DuplicateOrder(int iOnlineOrderId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = $"select tableid from PhoneOrder where CreatePasswordName = '{iOnlineOrderId}'";

                int result = connection.ExecuteScalar<int>(query);
                if (result > 0)
                {
                    util.Logger("Found Online_DuplicateOrder = " + query + " : tabldid = " + result.ToString());
                    return true;
                }
                else
                {
                    // normal as new order
                    return false;
                }
            }
        }

        internal double Get_Tax_Amount_ByTable(int intTableId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                //string query = $"SELECT sum(Tax1 + Tax2 + Tax3) from TableTran WHERE tableid =" + intTableId.ToString() + " And tableid <= " + intTableIdEnd.ToString() +";";
                string query = $"SELECT sum(Tax1 + Tax2 + Tax3) from TableTran WHERE tableid =" + intTableId.ToString() + ";";
                var output = connection.ExecuteScalar<double>(query);
                double dblTaxSum = output;

                return dblTaxSum;
            }
        }
        //Get_Total_Amount_ByTable
        internal double Get_Total_Amount_ByTable(int intTableId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                //string query = $"SELECT sum(Tax1 + Tax2 + Tax3) from TableTran WHERE tableid =" + intTableId.ToString() + " And tableid <= " + intTableIdEnd.ToString() +";";
                string query = $"SELECT sum(Amount + Tax1 + Tax2 + Tax3) from TableTran WHERE tableid =" + intTableId.ToString() + ";";
                var output = connection.ExecuteScalar<double>(query);
                double dblTotAmt = output;

                return dblTotAmt;
            }
        }
        internal POS_TableLayoutModel Get_DiningTableId_ByTableName(string strTable_number)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                var output = connection.Query<POS_TableLayoutModel>($"SELECT id, TableType, TableName from TableLayout WHERE TableType in (1,2) and TableName ='{strTable_number.Trim()}'").ToList();
                return output[0];
            }
        }
        //Feature #2676
        internal int Insert_Reservation(POS_ReservationModel posRservation)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO Reservation (ReservDate, ReservTime, CustName, Phone, NoOfPeople, Memo, CustID " +
                                ") values (" +
                                "@ReservDate, @ReservTime, @CustName, @Phone, @NoOfPeople, @Memo, @CustID  " +
                                "); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Tran);
                //return count;
                int result = connection.QuerySingle<int>(query, posRservation);
                //return Task.FromResult(0);
                return result;
            }
        }
        //Feature #3269
        internal int Insert_TableTranMemo(int intTableTranId, string instructions)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS")))
            {
                string query = "Insert INTO TableTranMemo (TableTranId, Memo " +
                                ") values (" +
                                $"{intTableTranId}, '{instructions}' " +
                                "); " +
                                "SELECT CAST(SCOPE_IDENTITY() as int)";
                //var count = connection.Execute(query, pos_Tran);
                //return count;
                int result = connection.QuerySingle<int>(query);
                //return Task.FromResult(0);
                return result;
            }
        }
    }
}
