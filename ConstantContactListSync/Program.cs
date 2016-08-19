using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ConstantContactListSync
{
    class Program
    {
        //Var for Constant Contact API Key
        static string cc_api_key = "your_developer_api_key";

        //Var for Constant Contact Auth Token
        static string cc_auth_token = "oauth_token_from_clients_account";

        //Var for Constant Contact API Base URL
        static string cc_base_url = "https://api.constantcontact.com/v2";


        static void Main(string[] args)
        {

            //Get List of All Contact Lists
            Pull_All_Contact_Lists();

            //List of Emails Addresses To Add
            List<string> lEmailAddrsToAdd = new List<string>();
            lEmailAddrsToAdd.Add("user001@mycollege.edu");
            lEmailAddrsToAdd.Add("user002@mycollege.edu");
            lEmailAddrsToAdd.Add("user003@mycollege.edu");

            //Sync a List By ID
            Sync_List_By_ID("10101010101", lEmailAddrsToAdd);

            //End of the run. Pause to show results
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("-----End of the Run-----");
            Console.ReadLine();

        }


        public static void Pull_All_Contact_Lists()
        {

            //Http Web Request for Constant Contact Sync
            HttpWebRequest hwreqGetLists = (HttpWebRequest)WebRequest.Create(cc_base_url + "/lists?api_key=" + cc_api_key);
            hwreqGetLists.ContentType = "application/json; charset=utf-8";
            hwreqGetLists.Accept = "application/json";
            hwreqGetLists.Headers.Add("Authorization", "Bearer " + cc_auth_token);
            hwreqGetLists.Method = "Get";

            //Web Response for Get Lists
            HttpWebResponse hwrespGetLists = (HttpWebResponse)hwreqGetLists.GetResponse();

            //Stream Reader to Convert JSON Data
            StreamReader strmrdrGetLists = new StreamReader(hwrespGetLists.GetResponseStream());

            //Convert Returned JSON to Dynamic Object
            dynamic dynCCLists = JsonConvert.DeserializeObject<dynamic>(strmrdrGetLists.ReadToEnd());

            //Close Connections and Reader
            hwrespGetLists.Close();
            strmrdrGetLists.Close();

            //Null Check on Returned Lists
            if(dynCCLists != null && dynCCLists.Count > 0)
            {
                foreach(var ccList in dynCCLists)
                {
                    Console.WriteLine("ID: " + ccList.id);
                    Console.WriteLine("Name: " + ccList.name);
                    Console.WriteLine("Status: " + ccList.status);
                    Console.WriteLine("Created On: " + ccList.created_date);
                    Console.WriteLine("Modified On: " + ccList.modified_date);
                    Console.WriteLine("Contact Count: " + ccList.contact_count);
                    Console.WriteLine(" ");
                }

            }//End of dynCCLists Null\Empty Check


        }//End of Pull_Contact_Lists


        public static void Sync_List_By_ID(string cc_list_id, List<string> lEmailAddresses)
        {
            
            //Var for Clear List Membership Json
            string jsnClearListData = "{\"lists\":[\"" + cc_list_id + "\"]}";

            //#######################################
            // Clear Out List Before Adding Contacts
            //#######################################

            //Http Web Request for Constant Contact Sync
            HttpWebRequest hwreqCCListClear = (HttpWebRequest)WebRequest.Create(cc_base_url + "/activities/clearlists?api_key=" + cc_api_key);
            hwreqCCListClear.ContentType = "application/json; charset=utf-8";
            hwreqCCListClear.Accept = "application/json";
            hwreqCCListClear.Headers.Add("Authorization", "Bearer " + cc_auth_token);
            hwreqCCListClear.Method = "POST";

            //Use Stream Writer to Send Json Up to Server
            using (var strmwrtCCListClear = new StreamWriter(hwreqCCListClear.GetRequestStream()))
            {
                strmwrtCCListClear.Write(jsnClearListData);
                strmwrtCCListClear.Flush();
            }

            //Web Response for Constant Contact API List Clear Response Call
            HttpWebResponse hwrespCCListClear = (HttpWebResponse)hwreqCCListClear.GetResponse();

            //Stream Reader to Convert JSON Data
            StreamReader strmrdrCCListClear = new StreamReader(hwrespCCListClear.GetResponseStream());

            //Convert Returned JSON to Dynamic Object
            dynamic dynCCListClear = JsonConvert.DeserializeObject<dynamic>(strmrdrCCListClear.ReadToEnd());

            //Close Connections and Reader
            hwrespCCListClear.Close();
            strmrdrCCListClear.Close();

            //Check Returned Json
            if (dynCCListClear != null)
            {
                Console.WriteLine("Clear List API Response");
                Console.WriteLine("ID: " + dynCCListClear.id);
                Console.WriteLine("Type: " + dynCCListClear.type);
                Console.WriteLine("Error Count: " + dynCCListClear.error_count);
                Console.WriteLine("Contact Count: " + dynCCListClear.contact_count);
            }
            else
            {
                Console.WriteLine("Clear reply was null");
            }

            //########################################
            // Add Contacts to List
            //########################################

            //Initiate List Sync Object 
            Constant_Contact_List_Sync cc_list_sync = new Constant_Contact_List_Sync();

            //Set the List ID as String
            cc_list_sync.lists.Add(cc_list_id);

            //Set the Email Column Name for Sync
            cc_list_sync.column_names.Add("EMAIL");

            //Load Email List into Sync Object
            foreach (string emlAdr in lEmailAddresses)
            {
                //Initiate Import Data 
                Constant_Contact_Import_Data cc_imp_data = new Constant_Contact_Import_Data();
                cc_imp_data.email_addresses.Add(emlAdr);

                //Add to List Sync Import Data Array
                cc_list_sync.import_data.Add(cc_imp_data);
            }

            //Http Web Request for Constant Contact Sync
            HttpWebRequest hwreqCCListSync = (HttpWebRequest)WebRequest.Create(cc_base_url + "/activities/addcontacts?api_key=" + cc_api_key);
            hwreqCCListSync.ContentType = "application/json; charset=utf-8";
            hwreqCCListSync.Accept = "application/json";
            hwreqCCListSync.Headers.Add("Authorization", "Bearer " + cc_auth_token);
            hwreqCCListSync.Method = "POST";

            //Use Stream Writer to Send Json Up to Server
            using (var strmwrtCCListSync = new StreamWriter(hwreqCCListSync.GetRequestStream()))
            {
                strmwrtCCListSync.Write(JsonConvert.SerializeObject(cc_list_sync, Formatting.None));
                strmwrtCCListSync.Flush();
            }

            //Web Response for Constant Contact API Add Contact Response Call
            HttpWebResponse hwrespCCListSync = (HttpWebResponse)hwreqCCListSync.GetResponse();

            //Stream Reader to Convert JSON Data
            StreamReader strmrdrCCListSync = new StreamReader(hwrespCCListSync.GetResponseStream());

            //Convert Returned JSON to Dynamic Object
            dynamic dynCCListSync = JsonConvert.DeserializeObject<dynamic>(strmrdrCCListSync.ReadToEnd());

            //Close Connections and Reader
            hwrespCCListSync.Close();
            strmrdrCCListSync.Close();

            //Check Returned Json
            if (dynCCListSync != null)
            {
                Console.WriteLine(" ");
                Console.WriteLine("Add Contacts API Response");
                Console.WriteLine("ID: " + dynCCListSync.id);
                Console.WriteLine("Type: " + dynCCListSync.type);
                Console.WriteLine("Error Count: " + dynCCListSync.error_count);
                Console.WriteLine("Contact Count: " + dynCCListSync.contact_count);

            }
            else
            {
                Console.WriteLine("Add reply was null");
            }


        }//End of Sync_List_By_ID


    }
}
