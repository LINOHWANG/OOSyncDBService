using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_CategoriesModel
    {
        public int Id { get; set; }
        public int Menu_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int? Active_Begin { get; set; }
        public int? Active_End { get; set; }
        public int Active_Days { get; set; }
        public int Sort { get; set; }
        public int? Picture_Id { get; set; }
        public DateTime? Hidden_Until { get; set; }
        public List<GF_ItemsModel> Items { get; set; }
        public List<GF_OptionGroupsModel> Groups { get; set; }
    }
}