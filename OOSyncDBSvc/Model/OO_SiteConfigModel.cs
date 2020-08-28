using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_SiteConfigModel
    {
        public int Id { get; set; }
        public string SiteCode { get; set; }
        public string ConfigName { get; set; }
        public string ConfigValue { get; set; }
        public string ConfigDesc { get; set; }
    }
}
