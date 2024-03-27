using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class GF_MenusModel
    {
        public List<string> Languages { get; set; }
        public int Id { get; set; }
        public string Currency { get; set; }
        public string Default_Language { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }

        public int Account_Id { get; set; }
        public List<GF_CategoriesModel> Categories { get; set; }

        // List<currency_config> is not implemented 
    }
}
