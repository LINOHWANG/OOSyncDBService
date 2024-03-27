using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_OptionsModel
    {
        public int Id { get; set; }
        public int Option_Group_Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public bool Default { get; set; }
        public int Sort { get; set; }
    }
}
