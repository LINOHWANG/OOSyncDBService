using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_ChildProdModel
    {
        public string SiteCode { get; set; }
        public int ProdId { get; set; }
        public int ParentProdId { get; set; }
        public int ChildGroupId { get; set; }
        public int Seq { get; set; }
    }
}
