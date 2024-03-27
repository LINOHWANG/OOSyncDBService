using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_GF_MenuItemsModel
    {
        public int Id { get; set; }
        public int Menu_Category_Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public bool Active { get; set; }
        public int ParentId { get; set; }
        public int SubType { get; set; }
        public string SubName { get; set; }
        public int? Match_POS_Id { get; set; }

        public DateTime? Match_DTTM { get; set; }

        public string CategoryName { get; set; }

        public bool? IsPrinter1 { get; set; }
        public bool? IsPrinter2 { get; set; }
        public bool? IsPrinter3 { get; set; }
        public bool? IsPrinter4 { get; set; }
        public bool? IsPrinter5 { get; set; }
        public bool? IsPrinter6 { get; set; }

    }
}
