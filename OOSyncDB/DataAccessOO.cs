using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOSyncDB.Model;
using Dapper;

namespace OOSyncDB
{
    class DataAccessOO
    {
        public List<OO_ProductModel> GetProductByProdID(string prodId)
        {
            ////////////////////////////////////////////////////
            // To connect to SQL Server
            // Open the door to the database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from product where id = {prodId}").ToList();
                return output;
            }
        }
        public List<OO_ProductModel> Get_ALL_Products()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from Product").ToList();
                return output;
            }
        }
        public List<OO_ProductModel> Get_Product_By_ID(int prodID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductModel>($"select * from Product where id = {prodID}").ToList();
                return output;
            }
        }
        public List<OO_ProductTypeModel> Get_ALL_ProductTypes()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductTypeModel>($"select * from ProductType").ToList();
                return output;
            }
        }
        public List<OO_ProductTypeModel> Get_ProductType_By_ID(int typeID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ProductTypeModel>($"select * from ProductType where id = {typeID}").ToList();
                return output;
            }
        }
        public int Update_Product(POS_ProductModel pos_ProductModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE Product SET ProductName = @ProductName, SecondName=@SecondName, ProductTypeId=@ProductTypeId, " +
                                "UnitPrice=CAST(@OutUnitPrice as decimal(10,2)), IsTax1=@IsTax1, IsTax2=@IsTax2, IsTax3=@IsTax3, IsTaxInverseCalculation=@IsTaxInverseCalculation, " +
                                "PromoStartDate=@PromoStartDate, PromoEndDate=@PromoEndDate, PromoDay1=@PromoDay1, PromoDay2=@PromoDay2, PromoDay3=@PromoDay3, " +
                                "PromoPrice1=CAST(@PromoPrice1 as decimal(10,2)), PromoPrice2=CAST(@PromoPrice2 as decimal(10,2)), PromoPrice3=CAST(@PromoPrice3 as decimal(10,2)), IsSoldOut=@IsSoldOut " +
                                "SyncDate='' " +
                                "WHERE Id = @Id";
                var count = connection.Execute(query, pos_ProductModel);
                return count;
            }
        }

        public int Insert_Product(POS_ProductModel poS_ProductModel)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO Product (id,ProductName, SecondName, ProductTypeId, " +
                            "UnitPrice, IsTax1, IsTax2, IsTax3, IsTaxInverseCalculation, " +
                            "PromoStartDate, PromoEndDate, PromoDay1, PromoDay2, PromoDay3, " +
                            "PromoPrice1, PromoPrice2, PromoPrice3, IsSoldOut, SyncDate) " +
                            "VALUES(@id,@ProductName,@SecondName,@ProductTypeId,CAST(@OutUnitPrice as decimal(10,2)),@IsTax1,@IsTax2,@IsTax3,@IsTaxInverseCalculation, " +
                            "@PromoStartDate,@PromoEndDate,@PromoDay1,@PromoDay2,@PromoDay3,CAST(@PromoPrice1 as decimal(10,2)),CAST(@PromoPrice2 as decimal(10,2)),CAST(@PromoPrice3 as decimal(10,2)),@IsSoldOut,'') ";
                var count = connection.Execute(query, poS_ProductModel);
                return count;
            }
        }
        public int Update_ProductType(POS_ProductTypeModel pos_ProductType)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE ProductType SET TypeName=@TypeName WHERE Id = @Id";
                var count = connection.Execute(query, pos_ProductType);
                return count;
            }
        }

        public int Insert_ProductType(POS_ProductTypeModel pos_ProductType)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO ProductType (id,TypeName) VALUES(@id,@TypeName)";
                var count = connection.Execute(query, pos_ProductType);
                return count;
            }
        }
        public int Delete_Product_By_ID(int prodID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE Product WHERE id = " + prodID.ToString();
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Delete_All_Product()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE Product";
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Delete_All_ProductType()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "DELETE ProductType";
                var count = connection.Execute(query);
                return count;
            }
        }
        public List<OO_TaxModel> Get_All_Tax()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TaxModel>($"select * from Tax").ToList();
                return output;
            }
        }
        public List<OO_TaxModel> Get_Tax(POS_TaxModel pos_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TaxModel>($"select * from Tax where Code = '{pos_Tax.Code}'").ToList();
                return output;
            }
        }
        public int Update_Tax(POS_TaxModel pos_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE Tax SET Tax1=@Tax1,Tax2=@Tax2,Tax3=@Tax3 WHERE Code = @Code";
                var count = connection.Execute(query, pos_Tax);
                return count;
            }
        }

        public int Insert_Tax(POS_TaxModel pos_Tax)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "INSERT INTO Tax (Code,Tax1,Tax2,Tax3) VALUES(@Code,@Tax1,@Tax2,@Tax3)";
                var count = connection.Execute(query, pos_Tax);
                return count;
            }
        }
        /* ---------------------------------------------------------------------------------- */
        /* Sync transactions from OO to POS
        /* ---------------------------------------------------------------------------------- */
        public List<OO_TranModel> Get_All_Pending_Transactions()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TranModel>($"select * from OOTransaction where IsPOSUpdated < 1 order by OrderDate, OrderTime").ToList();
                return output;
            }
        }
        public List<OO_TranModel> Get_All_Error_Transactions()
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_TranModel>($"select * from OOTransaction where IsPOSUpdated = 9 order by OrderDate, OrderTime").ToList();
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
                var output = connection.Query<OO_CustomerModel>($"select * from Customer where WebId = {iCustomerId}").ToList();
                return output;
            }
        }
        public List<OO_ItemModel> Get_OrderItem_by_TranID(int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                var output = connection.Query<OO_ItemModel>($"select * from OOItem where TransactionId = '{iTranID.ToString()}' ").ToList();
                return output;
            }
        }
        public int Transaction_Sync_Completed(int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE OOTransaction SET IsPOSUpdated=1 WHERE Id =" + iTranID.ToString();
                var count = connection.Execute(query);
                return count;
            }
        }
        public int Transaction_Sync_Error(int iTranID)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("OODB")))
            {
                string query = "UPDATE OOTransaction SET IsPOSUpdated=9 WHERE Id =" + iTranID.ToString();
                var count = connection.Execute(query);
                return count;
            }
        }
    }
}
