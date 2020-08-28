using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_ProductTypeModel
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public string SiteCode { get; set; }
        public string TypeName { get; set; }
    }
}
