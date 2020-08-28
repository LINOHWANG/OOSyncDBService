using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_SysConfigModel
    {
        public string config_id        { get; set; }
        public string config_name        { get; set; }
        public string config_value        { get; set; }
        public string config_desc        { get; set; }
        public bool config_oosync { get; set; }
    }
}
