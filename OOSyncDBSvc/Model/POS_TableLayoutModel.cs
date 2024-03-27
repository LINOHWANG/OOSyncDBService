using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_TableLayoutModel
    {
            public int Id { get; set; }
            public int TableType { get; set; }  // 1=Dine-In, 2=Waiting, 3=Shape, 4=Text
            public string TableName { get; set; }
    }
}
