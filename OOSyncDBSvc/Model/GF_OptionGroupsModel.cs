using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_OptionGroupsModel
    {
        public int Id { get; set; }
        public int Menu_Id { get; set; }
        public string Name { get; set; }
        public int Sort { get; set; }
        public bool Required { get; set; }
        public int Force_Max { get; set; }
        public int Force_Min { get; set; }
        public bool Allow_Quantity { get; set; }
        public int Tax_Category_Id { get; set; }
        public List<GF_OptionsModel> Options { get; set; }
    }
}
