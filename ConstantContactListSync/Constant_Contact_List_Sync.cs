using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstantContactListSync
{
    public class Constant_Contact_List_Sync
    {
        public List<string> column_names { get; set; }
        public List<string> lists { get; set; }
        public List<Constant_Contact_Import_Data> import_data { get; set; }

        public Constant_Contact_List_Sync()
        {
            column_names = new List<string>();
            lists = new List<string>();
            import_data = new List<Constant_Contact_Import_Data>();
        }

    }
}
