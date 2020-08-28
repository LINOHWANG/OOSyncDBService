using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_SiteModel
    {
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string SiteAddress { get; set; }
        public string SitePhoneNumber { get; set; }
        public string SiteGSTNumber { get; set; }
        public DateTime SiteCreatedDTTM { get; set; }
        public bool IsSiteLive { get; set; }
        public string MainSiteCode { get; set; }

        //public string Site_Biz_Hour_Start { get; set; }
        //public string Site_Biz_Hour_Finish { get; set; }
        //public string Site_Biz_Hour_LastCall_Hour { get; set; }
        //public string Site_Biz_Break_Start { get; set; }
        //public string Site_Biz_Break_Finish { get; set; }
    }

}
