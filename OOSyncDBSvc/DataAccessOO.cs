using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOSyncDBSvc.Model;
using Dapper;

namespace OOSyncDBSvc
{
    class DataAccessOO
    {
        public List<OO_ProductModel> GetProductByProdID(string siteCode, string prodId)
        {
            ////////////////////////////////////////////////////
            // To connect to SQL Server
            // Open the door to the database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from product where PosProdid = {prodId} and SiteCode='{siteCode}'").ToList();
                return output;
            }
        }
        public List<OO_ProductModel> Get_ALL_Products(string siteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from Product where SiteCode='{siteCode}'").ToList();
                return output;
            }
        }
        public List<OO_ProductModel> Get_Product_By_ID(string siteCode, int prodID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from Product where PosProdId = {prodID} and SiteCode='{siteCode}'").ToList();
                return output;
            }
        }
        public List<OO_ChildGroupModel> Get_ChildGroup_By_ID(string siteCode, int p_intChildGroupID, int p_intParentProdId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ChildGroupModel>($"select * from ChildGroup where SiteCode='{siteCode}' and Id = {p_intChildGroupID} and ParentProdId={p_intParentProdId}").ToList();
                return output;
            }
        }
        public List<OO_ChildProdModel> Get_ChildProd_By_ID(string siteCode, int p_intChildGroupId, int p_intProdID, int p_intParentProdId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ChildProdModel>($"select * from ChildProd where SiteCode='{siteCode}' and ChildGroupId={p_intChildGroupId} and ProdId = {p_intProdID}  and ParentProdId={p_intParentProdId}").ToList();
                return output;
            }
        }
        public List<OO_ProductTypeModel> Get_ALL_ProductTypes(string siteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductTypeModel>($"select * from ProductType where SiteCode = '{siteCode}'").ToList();
                return output;
            }
        }
        public List<OO_ProductTypeModel> Get_ProductType_By_ID(string siteCode,int typeID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductTypeModel>($"select * from ProductType where ProductTypeId = {typeID} and SiteCode = '{siteCode}'").ToList(); 
                return output;
            }
        }
        public List<OO_SiteModel> Get_Site_By_Code(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_SiteModel>($"select * from site where SiteCode='{strSiteCode}'").ToList();
                return output;
            }
        }
        public int Update_Site(OO_SiteModel oo_SiteModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE Site SET SiteName = @SiteName, SiteAddress=@SiteAddress, SitePhoneNumber=@SitePhoneNumber, " +
                                "SiteGSTNumber=@SiteGSTNumber, IsSiteLive=@IsSiteLive " +
                                //, Site_Biz_Hour_Start=@Site_Biz_Hour_Start, Site_Biz_Hour_Finish=@Site_Biz_Hour_Finish, " +
                                //"Site_Biz_Hour_LastCall_Hour=@Site_Biz_Hour_LastCall_Hour, Site_Biz_Break_Start=@Site_Biz_Break_Start, " +
                                //"Site_Biz_Break_Finish=@Site_Biz_Break_Finish " +
                                "WHERE SiteCode = @SiteCode";
                var count = connection.Execute(query, oo_SiteModel);
                return count;
            }
        }
        public int Insert_Site(OO_SiteModel oo_SiteModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO Site (SiteCode,SiteName, SiteAddress, SitePhoneNumber, " +
                            "SiteGSTNumber, SiteCreatedDTTM, IsSiteLive, MainSiteCode) " +
                            "VALUES(@SiteCode,@SiteName,@SiteAddress,@SitePhoneNumber,@SiteGSTNumber, @SiteCreatedDTTM, @IsSiteLive, @MainSiteCode) ";
                            //"@Site_Biz_Hour_Start,@Site_Biz_Hour_Finish,@Site_Biz_Hour_LastCall_Hour,@Site_Biz_Break_Start,@Site_Biz_Break_Finish) ";
                var count = connection.Execute(query, oo_SiteModel);
                return count;
            }
        }
        public int Update_Product(OO_ProductModel oo_ProductModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE Product SET ProductName = @ProductName, SecondName=@SecondName, ProductTypeId=@ProductTypeId, " +
                                "UnitPrice=CAST(@UnitPrice as decimal(10,2)), IsTax1=@IsTax1, IsTax2=@IsTax2, IsTax3=@IsTax3, IsTaxInverseCalculation=@IsTaxInverseCalculation, " +
                                "PromoStartDate=@PromoStartDate, PromoEndDate=@PromoEndDate, PromoDay1=@PromoDay1, PromoDay2=@PromoDay2, PromoDay3=@PromoDay3, " +
                                "PromoPrice1=CAST(@PromoPrice1 as decimal(10,2)), PromoPrice2=CAST(@PromoPrice2 as decimal(10,2)), PromoPrice3=CAST(@PromoPrice3 as decimal(10,2)), IsSoldOut=@IsSoldOut, " +
                                "SyncDate=@SyncDate, IsSubItem=@IsSubItem " +
                                "WHERE PosProdId = @PosProdId and SiteCode = @SiteCode";
                var count = connection.Execute(query, oo_ProductModel);
                return count;
            }
        }

        public int Insert_Product(OO_ProductModel oo_ProductModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO Product (SiteCode, PosProdid, ProductName, SecondName, ProductTypeId, " +
                            "UnitPrice, IsTax1, IsTax2, IsTax3, IsTaxInverseCalculation, " +
                            "PromoStartDate, PromoEndDate, PromoDay1, PromoDay2, PromoDay3, " +
                            "PromoPrice1, PromoPrice2, PromoPrice3, IsSoldOut, SyncDate, IsSubItem) " +
                            "VALUES(@SiteCode, @PosProdid,@ProductName,@SecondName,@ProductTypeId,CAST(@UnitPrice as decimal(10,2)),@IsTax1,@IsTax2,@IsTax3,@IsTaxInverseCalculation, " +
                            "@PromoStartDate,@PromoEndDate,@PromoDay1,@PromoDay2,@PromoDay3,CAST(@PromoPrice1 as decimal(10,2)),CAST(@PromoPrice2 as decimal(10,2)), " +
                            "CAST(@PromoPrice3 as decimal(10,2)),@IsSoldOut,@SyncDate,@IsSubItem) ";
                var count = connection.Execute(query, oo_ProductModel);
                return count;
            }
        }
        public int Update_ProductType(OO_ProductTypeModel oo_ProductType)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE ProductType SET TypeName=@TypeName WHERE ProductTypeId = @PosTypeId and SiteCode=@SiteCode";
                var count = connection.Execute(query, oo_ProductType);
                return count;
            }
        }

        public int Insert_ProductType(OO_ProductTypeModel oo_ProductType)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO ProductType (SiteCode,ProductTypeid,TypeName) VALUES(@SiteCode,@ProductTypeid,@TypeName)";
                var count = connection.Execute(query, oo_ProductType);
                return count;
            }
        }
        public int Delete_Product_By_ID(string strSiteCode, int prodID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE Product WHERE PosProdId = " + prodID.ToString() + " and SiteCode='"+ strSiteCode + "'";
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Delete_All_Product(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE Product WHERE SiteCode='" + strSiteCode + "'";
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Delete_All_ProductType(string siteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE ProductType WHERE SiteCode='" + siteCode + "'";
                var count = connection.Execute(query);
                return count;
            }
        }
        public List<OO_TaxModel> Get_All_Tax(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TaxModel>($"select * from Tax where SiteCode='{strSiteCode}'").ToList();
                return output;
            }
        }
        public List<OO_TaxModel> Get_Tax(string strSiteCode,POS_TaxModel pos_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TaxModel>($"select * from Tax where PosCode = '{pos_Tax.Code}' and SiteCode = '{strSiteCode}'").ToList();
                return output;
            }
        }
        public int Update_Tax(string strSiteCode, OO_TaxModel oo_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE Tax SET Tax1=@Tax1,Tax2=@Tax2,Tax3=@Tax3 WHERE PosCode = @Code and SiteCode ='" +strSiteCode+"'";
                var count = connection.Execute(query, oo_Tax);
                return count;
            }
        }

        public int Insert_Tax(OO_TaxModel oo_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO Tax (SiteCode,PosCode,Tax1,Tax2,Tax3) VALUES(@SiteCode,@PosCode,@Tax1,@Tax2,@Tax3)";
                var count = connection.Execute(query, oo_Tax);
                return count;
            }
        }
        /* ---------------------------------------------------------------------------------- */
        /* Sync transactions from OO to POS
        /* ---------------------------------------------------------------------------------- */
        public List<OO_TranModel> Get_All_Pending_Transactions(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TranModel>($"select * from OOTransaction where IsPOSUpdated <> 1 and SiteCode = '{strSiteCode}' order by OrderDate, OrderTime").ToList();
                return output;
            }
        }
        public List<OO_TranModel> Get_All_Error_Transactions(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TranModel>($"select * from OOTransaction where IsPOSUpdated = 9 and SiteCode = '{strSiteCode}' order by OrderDate, OrderTime").ToList();
                return output;
            }
        }
        public List<OO_CustomerModel> Get_Customer_by_PhoneName(string strPhoneNo, string strLastName)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_CustomerModel>($"select * from Customer where Phone = '{strPhoneNo}' and LastName = '{strLastName}' ").ToList();
                return output;
            }
        }
        public List<OO_CustomerModel> Get_Customer_by_CustomerId(int iCustomerId)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_CustomerModel>($"select * from Customer where Id = {iCustomerId}").ToList();
                return output;
            }
        }
        public List<OO_ItemModel> Get_TranItems_by_TranID(string strSiteCode, int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ItemModel>($"select * from OOTranItems where TransactionId = '{iTranID.ToString()}' and SiteCode='{strSiteCode}' ").ToList();
                return output;
            }
        }
        public int Transaction_Sync_Completed(string strSiteCode, int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE OOTransaction SET IsPOSUpdated=1, DateTimePOSUpdate=GETDATE() WHERE Id =" + iTranID.ToString()+" AND SiteCode='" + strSiteCode+"'";
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Transaction_Sync_Error(string strSiteCode, int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE OOTransaction SET IsPOSUpdated=9 WHERE Id =" + iTranID.ToString() + " AND SiteCode='" + strSiteCode + "'"; 
                var count = connection.Execute(query);
                return count;
            }
        }

        internal List<OO_ProductDetailModel> Get_All_ProductDetail(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductDetailModel>($"select * from ProductDetail where SiteCode = '{strSiteCode}'").ToList();
                return output;
            }
        }

        internal List<OO_ProductDetailModel> Get_ProductDetail(string strSiteCode, POS_ProductDetailModel pDetail)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductDetailModel>($"select * from ProductDetail where ItemNo = {pDetail.ItemNo} and PosProdId = '{pDetail.ProductId}' and SiteCode = '{strSiteCode}'").ToList();
                return output;
            }
        }

        internal int Insert_ProductDetail(OO_ProductDetailModel oo_ProductDetail)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO ProductDetail (SiteCode,PosProdid,ItemNo,Detail) VALUES(@SiteCode,@PosProdid,@ItemNo,@Detail)";
                var count = connection.Execute(query, oo_ProductDetail);
                return count;
            }
        }

        internal int Update_ProductDetail(string strSiteCode, OO_ProductDetailModel oo_ProductDetail)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE ProductDetail SET Detail=@Detail WHERE PosProdlId =" + oo_ProductDetail.PosProdId + " and ItemNo=" + oo_ProductDetail.ItemNo + " " + " and SiteCode='" + oo_ProductDetail.SiteCode +"'";
                var count = connection.Execute(query, oo_ProductDetail);
                return count;
            }
        }

        internal List<OO_SiteConfigModel> Get_All_SiteConfig(string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_SiteConfigModel>($"select * from SiteConfig where SiteCode = '{strSiteCode}'").ToList();
                return output;
            }
        }

        internal List<OO_SiteConfigModel> Get_SiteConfig_By_ConfigName(string config_Name, string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_SiteConfigModel>($"select * from SiteConfig where ConfigName = '{config_Name}' And SiteCode = '{strSiteCode}'").ToList();
                return output;
            }
        }

        internal int Insert_SiteConfig(OO_SiteConfigModel oo_SiteConfig)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO SiteConfig (SiteCode,ConfigName,ConfigValue,ConfigDesc) VALUES(@SiteCode,@ConfigName,@ConfigValue,@ConfigDesc)";
                var count = connection.Execute(query, oo_SiteConfig);
                return count;
            }
        }

        internal int Update_SiteConfig(OO_SiteConfigModel oo_SiteConfig, string strSiteCode)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE SiteConfig SET ConfigValue=@ConfigValue, ConfigDesc=@ConfigDesc WHERE ConfigName='" + oo_SiteConfig.ConfigName + "' and SiteCode='" + strSiteCode + "'";
                var count = connection.Execute(query, oo_SiteConfig);
                return count;
            }
        }

        internal object Get_ChildGroup_By_ID(string strSiteCode, int id)
        {
            throw new NotImplementedException();
        }

        internal int Update_ChildGroup(OO_ChildGroupModel oo_ChildGroupModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE ChildGroup SET ChildGroupName=@ChildGroupName, IsMultiChoice=@IsMultiChoice, IsMandatory=@IsMandatory  " +
                            "WHERE SiteCode ='" + oo_ChildGroupModel.SiteCode + "' and Id=" + oo_ChildGroupModel.Id + " " + " and ParentProdIde=" + oo_ChildGroupModel.ParentProdId;
                var count = connection.Execute(query, oo_ChildGroupModel);
                return count;
            }
        }

        internal int Insert_ChildGroup(OO_ChildGroupModel oo_ChildGroupModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO ChildGroup (SiteCode,Id,ParentProdId,ChildGroupName,IsMultiChoice,IsMandatory) VALUES(@SiteCode,@Id,@ParentProdId,@ChildGroupName,@IsMultiChoice,@IsMandatory)";
                var count = connection.Execute(query, oo_ChildGroupModel);
                return count;
            }
        }

        internal int Update_ChildProd(OO_ChildProdModel oo_ChildProdModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE ChildProd SET Seq=@Seq  " +
                            "WHERE SiteCode ='" + oo_ChildProdModel.SiteCode + "' and ProdId=" + oo_ChildProdModel.ProdId + " " + " and ParentProdIde=" + oo_ChildProdModel.ParentProdId + 
                            " ChildGroupId = " + oo_ChildProdModel.ChildGroupId;
                var count = connection.Execute(query, oo_ChildProdModel);
                return count;
            }
        }
        internal int Insert_ChildProd(OO_ChildProdModel oo_ChildProdModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO ChildProd (SiteCode,ProdId,ParentProdId,ChildGroupId,Seq) VALUES(@SiteCode,@ProdId,@ParentProdId,@ChildGroupId,@Seq)";
                var count = connection.Execute(query, oo_ChildProdModel);
                return count;
            }
        }
    }
}
