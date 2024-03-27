using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GFO_OrderItemsModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string instructions { get; set; }
        public string type { get; set; }
        public int? type_id { get; set; }
        public int? parent_id { get; set; }
        public float total_item_price { get; set; }
        public string tax_type { get; set; }
        public float tax_value { get; set; }
        public float tax_rate { get; set; }
        public float price { get; set; }
        public int quantity { get; set; }
        public float item_discount { get; set; }
        public float cart_discount { get; set; }
        public float cart_discount_rate { get; set; }
        public List<GFO_ItemOptionsModel> options { get; set; }
        public string coupon { get; set; }
    }
}
