﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class OO_ChildGroupModel
    {
        public string SiteCode { get; set; }
        public int Id { get; set; }
        public int ParentProdId { get; set; }
        public String ChildGroupName { get; set; }
        public bool IsMultiChoice { get; set; }
        public bool IsMandatory { get; set; }
    }
}
