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
    }
}