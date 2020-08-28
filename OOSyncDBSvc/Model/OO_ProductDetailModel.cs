using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_ProductDetailModel
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public int PosProdId { get; set; }
        public int ItemNo { get; set; }
        public string Detail { get; set; }
    }
}
