using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOSyncDB.Model
{
    class OO_CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Zip { get; set; }
        public string DateMarried { get; set; }
        public string DateOfBirth { get; set; }
        public string Memo { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
        public DateTime DateTimeVerified { get; set; }

    }
}
