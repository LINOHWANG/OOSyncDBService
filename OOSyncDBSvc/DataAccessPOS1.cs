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
    class DataAccessPOS1
    {
        Utilities util = new Utilities();
        public List<POS1_InvNoModel> Get_InvNo()
        {
            ////////////////////////////////////////////////////
            // To connect to SQL Server
            // Open the door to the database
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS1")))
            {
                var output = connection.Query<POS1_InvNoModel>($"select * from mfInvNo").ToList();
                return output;
            }
        }
        public int Issue_New_InvNo(int iNewInvNo)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS1")))
            {
                string query = "UPDATE mfInvNo SET InvNo=" + iNewInvNo.ToString() + ", InvDate=GETDATE()";
                var count = connection.Execute(query);
                return count;
            }
        }

        internal bool Check_Online_DuplicateOrder(int iOnlineOrderid)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Helper.CnnVal("POS1")))
            {
                string query = $"select tableid from PhoneTranComplete where CreatePasswordName = '{iOnlineOrderid}'";

                int result = connection.ExecuteScalar<int>(query);
                if (result > 0)
                {
                    util.Logger("Found Online_DuplicateOrder on PhoneTranComplete = " + query + " : tabldid = " + result.ToString());
                    return true;
                }
                else
                {
                    // normal as new order
                    return false;
                }
            }
        }
    }
}