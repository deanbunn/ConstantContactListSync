using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantContactListSync
{
    public class Constant_Contact_Import_Data
    {
        public List<string> email_addresses { get; set; }

        public Constant_Contact_Import_Data()
        {
            email_addresses = new List<string>();
        }
    }
}
