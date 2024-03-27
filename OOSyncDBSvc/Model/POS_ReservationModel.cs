using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDBSvc.Model
{
    class POS_ReservationModel
    {
        public int Id { get; set; }
        public string ReservDate { get; set; }
        public string ReservTime { get; set; }
        public string CustName { get; set; }
        public string Phone { get; set; }
        public int NoOfPeople { get; set; }

        public string Memo { get; set; }
        public int CustID { get; set; }
    }
}
