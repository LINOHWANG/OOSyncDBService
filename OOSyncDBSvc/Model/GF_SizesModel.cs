using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_SizesModel
    {
        public int Id { get; set; }
        public int Menu_Item_Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public bool Default { get; set; }
        public List<GF_OptionGroupsModel> Groups { get; set; }
    }
}
