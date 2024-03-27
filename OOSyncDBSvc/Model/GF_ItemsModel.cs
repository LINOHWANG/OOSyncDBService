using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_ItemsModel
    {
        public int Id { get; set; }
        public int Menu_category_id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public bool Active { get; set; }
        public int? Active_Begin { get; set; }
        public int? Active_End { get; set; }
        public int Active_Days { get; set; }
        public int Sort { get; set; }
        public List<string> Tags { get; set; }
        //public List<GF_TagsModel> Tags { get; set; }
        public DateTime? Hidden_Until { get; set; }
        public int Tax_Category_Id { get; set; }
        public bool Hide_Instructions { get; set; }
        public List<GF_SizesModel> Sizes { get; set; }
        public List<GF_OptionGroupsModel> Groups { get; set; }
    }
}
