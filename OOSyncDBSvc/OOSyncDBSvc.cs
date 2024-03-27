using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Timers;
using System.Runtime.InteropServices;
using OOSyncDBSvc.Model;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Net;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using System.Windows.Forms;

namespace OOSyncDBSvc
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class OOSyncDBSvc : ServiceBase
    {
        List<POS_PassWordModel> passWords = new List<POS_PassWordModel>();
        List<POS_OO_Prod_SyncModel> posProdSyncs = new List<POS_OO_Prod_SyncModel>();
        List<POS_ProductModel> posProducts = new List<POS_ProductModel>();
        POS_ProductModel posProduct = new POS_ProductModel();
        List<POS_ProductModel> posChildProducts = new List<POS_ProductModel>();
        List<POS_ChildGroupModel> posChildGroups = new List<POS_ChildGroupModel>();
        List<POS_ChildProdModel> posChildProds = new List<POS_ChildProdModel>();
        List<POS_TaxModel> posTaxs = new List<POS_TaxModel>();
        POS_TaxModel posTax = new POS_TaxModel();
        List<POS_ProductTypeModel> posProdTypes = new List<POS_ProductTypeModel>();
        List<POS_ProductDetailModel> posProdDetails = new List<POS_ProductDetailModel>();
        List<POS_CustomerModel> posCustomers = new List<POS_CustomerModel>();
        POS_CustomerModel posCustomer = new POS_CustomerModel();
        List<POS_PhoneOrderModel> posPhoneOrders = new List<POS_PhoneOrderModel>();
        POS_PhoneOrderModel posPhoneOrder = new POS_PhoneOrderModel();
        List<POS_TableTranModel> posTrans = new List<POS_TableTranModel>();
        List<POS_TableTranModel> posChildTrans = new List<POS_TableTranModel>();
        POS_TableTranModel posTran = new POS_TableTranModel();
        POS_TableTranModel posChildTran = new POS_TableTranModel();
        List<POS_SysConfigModel> posSysConfigs = new List<POS_SysConfigModel>();
        List<POS1_InvNoModel> pos1InvNos = new List<POS1_InvNoModel>();
        POS_SeqNoModel posSeqNo = new POS_SeqNoModel();
        /*        List<OO_ProductModel> ooProducts = new List<OO_ProductModel>();
                List<OO_ChildGroupModel> ooChildGroups = new List<OO_ChildGroupModel>();
                List<OO_ChildProdModel> ooChildProds = new List<OO_ChildProdModel>();
                List<OO_ProductTypeModel> ooProdTypes = new List<OO_ProductTypeModel>();
                List<OO_ProductDetailModel> ooProdDetails = new List<OO_ProductDetailModel>();
                List<OO_TaxModel> ooTaxs = new List<OO_TaxModel>();
                List<OO_TranModel> ooTrans = new List<OO_TranModel>();
                List<OO_CustomerModel> ooCustomers = new List<OO_CustomerModel>();
                List<OO_ItemModel> ooItems = new List<OO_ItemModel>();
                List<OO_ItemModel> ooChildItems = new List<OO_ItemModel>();
                List<OO_SiteModel> ooSites = new List<OO_SiteModel>();
                List<OO_SiteConfigModel> ooSiteConfigs = new List<OO_SiteConfigModel>();

                OO_SiteModel ooSite = new OO_SiteModel();
                OO_SiteConfigModel ooSiteConfig = new OO_SiteConfigModel(); */

        // --------------- Gloria Food API Menu Data ---------------------
        List<GF_MenusModel> gfMenus = new List<GF_MenusModel>();
        GF_MenusModel gfMenu = new GF_MenusModel();
        // --------------- POS Menu Product Matching Table ---------------------
        List<POS_GF_MenuItemsModel> posGF_MenuItems = new List<POS_GF_MenuItemsModel>();
        POS_GF_MenuItemsModel posGF_MenuItem = new POS_GF_MenuItemsModel();

        // --------------- Gloria Food API Order Data ---------------------
        List<GFO_CollectionsModel> gfoCollections = new List<GFO_CollectionsModel>();
        GFO_CollectionsModel gfoCollection = new GFO_CollectionsModel();
        List<GFO_OrdersModel> gfoOrders = new List<GFO_OrdersModel>();
        List<GFO_OrderItemsModel> gfoItems = new List<GFO_OrderItemsModel>();
        List<GFO_ItemOptionsModel> gfoOptions = new List<GFO_ItemOptionsModel>();
        POS_TableLayoutModel posTableLayout = new POS_TableLayoutModel();

        POS_ReservationModel posReservation = new POS_ReservationModel();       //Feature #2676

        Utilities util = new Utilities();
        private bool isPOSOK;
        private bool isOOOK;
        private string strActivity;
        private string strLog;
        private string strSiteCode;

        private int eventId = 1;
        private string strSiteName;
        private string strSiteBizHourStart;
        private string strSiteBizHourFinish;
        private string strSiteBizLastCallHour;
        private string strSiteBizBreakStart;
        private string strSiteBizBreakFinish;
        private bool isSiteDetailExists;
        private string strSiteAddress;
        private string strSitePhone;
        private string strSiteGSTNumber;
        private DateTime dtmStart;
        private string strServiceFeeName;   //Feature #3031

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);
        public OOSyncDBSvc()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            try
            {
                if (!System.Diagnostics.EventLog.SourceExists("OOSyncDBSvc"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("OOSyncDBSvc", "");
                }
            }
            catch(Exception e)
            {
                util.Logger(e.Message);
            }
            eventLog1.Source = "OOSyncDBSvc";
            eventLog1.Log = "";
        }
        public void onDebug()
        {
            util.Logger("++++++++++++++++++++++++ DEBUGGING OOSyncDBSvc Start ++++++++++++++++++++++++");
            //Sync_Process();
            Sync_GF_MenuItems_Process();
            if (POS_Get_Empty_PhoneOrder_TableId() > 0)
            {
                Sync_GF_Order_Process();
                Sync_GF_Order_Process_FromFile();
            }
            else
            {
                util.Logger("###### No Empty Online Tables #######");
            }
            util.Logger("++++++++++++++++++++++++ DEBUGGING OOSyncDBSvc Finish ++++++++++++++++++++++++");
        }
        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("OOSyncDB In OnStart.");

            var strTimerInterval = ConfigurationManager.AppSettings["TimerInterval"];
            int iTimerInterval = int.Parse(strTimerInterval);
            // Set up a timer that triggers every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            if (iTimerInterval > 0)
            {
                timer.Interval = iTimerInterval;
            }
            else
            {
                // default interval
                timer.Interval = 60000; // 60 seconds
            }
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            dtmStart = DateTime.Now;
            util.Logger("OOSyncDB Service is starting... : Interval = " + iTimerInterval + " Started at : " + dtmStart.ToString("yyyy-MM-dd HH:mm:ss"));
            // Get System Configuration : Feature #3031
            // Try until Sync_GF_MenuItems_Process have no exception 
            // Support #3105
            var tries = 100;
            while (true)
            {
                try
                {
                    Get_SystemConfiguration();
                    // Sync menu once when service is started
                    Sync_GF_MenuItems_Process();
                    timer.Start();
                    util.Logger("OOSyncDB Started Successfully");
                    break; // success!
                }
                catch (Exception e)
                {
                    util.Logger("OOSyncDB failed " + tries.ToString() + " tried : "+ e.Message);
                    if (--tries == 0)
                        throw;
                    Thread.Sleep(10000); // wait 10 second
                }
            }

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

        }

        private void Get_SystemConfiguration()
        {
            // Get System Configuration : Feature #3031
            // Get sysconfig table
            DataAccessPOS dbPos = new DataAccessPOS();
            posSysConfigs = dbPos.Get_SysConfig_By_Name("ONLINEORDER_SERVICE_FEE_NAME");
            util.Logger("Get_SystemConfiguration : " + posSysConfigs.Count.ToString());
            if (posSysConfigs.Count > 0)
            {
                strServiceFeeName = posSysConfigs[0].config_value;
            }
            else
            {
                strServiceFeeName = "NO_SERVICE_FEE";
            }

        }

        protected override void OnStop()
        {

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("OOSyncDB In OnStop.");
            util.Logger("OOSyncDB Service is stopping...");
        }
        public void OnTimer(object sender, ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            // eventLog1.WriteEntry("Monitoring OOSyncDB System : " + ConfigurationManager.AppSettings["ApplicationPath"], EventLogEntryType.Information, eventId++);
            util.Logger("OOSyncDBSvc Monitoring ....");
            bool isSameDay = (dtmStart.Date == DateTime.Now.Date);
            // Sync menu only when date is changed
            if (isSameDay != true)
            {
                util.Logger("DateChanged ....");
                Sync_GF_MenuItems_Process();
                dtmStart = DateTime.Now;
            }
            if (POS_Get_Empty_PhoneOrder_TableId() > 0)
            {
                Sync_GF_Order_Process();
                Sync_GF_Order_Process_FromFile();
            }
            else
            {
                util.Logger("###### No Empty Online Tables #######");
            }
        }
        private async Task<string> PostURI(Uri u, HttpContent c)
        {
            var response = string.Empty;
            
            string strAuthKey = ConfigurationManager.AppSettings["GFFetChOrderAPIRestaurantKey"];
            util.Logger("Order AuthKey = " + strAuthKey);
            if (string.IsNullOrEmpty(strAuthKey))
            {
                return response;
            }

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri("https://pos.globalfoodsoft.com");

                //specify to use TLS 1.2 as default connection : to connect to our server must support TLS 1.2 or 1.3.
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                // Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strAuthKey);
                client.DefaultRequestHeaders.Add("Authorization", strAuthKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Glf-Api-Version", "2");

                HttpResponseMessage result = await client.PostAsync(u, c);
                //if (result.IsSuccessStatusCode)
                //{
                    //response = result.StatusCode.ToString();
                //    response = result.Content.ToString();
                //}
                if (result.Content!= null)
                {
                    response = await result.Content.ReadAsStringAsync();
                }
            }
            return response;
        }
        private void Sync_GF_Order_Process()
        {
            util.Logger("----------------------< ORDER POLLING START >------------------------");
            //try
            //{
                Uri baseUri = new Uri("https://pos.globalfoodsoft.com/pos/order/pop");
                //var payload = "{\"Authorization\":" + strAuthKey + ",\"Accept\": \"application/json\",\"Glf-Api-Version\": 2}";
                var payload = "{}";


                HttpContent content = new StringContent(payload, Encoding.UTF8, "application/json");
                var t = Task.Run(() => PostURI(baseUri, content));
                t.Wait();
                
                util.Logger("Order API POST Status = " + t.Status);
                if (t.IsCompleted)
                {
                    //gfItems = response.Content.ReadAsAsync<List<GF_ItemsModel>>().Result;
                    string apiResponse = t.Result;
                    util.Logger("Polling Order POST API Playload Response = " + apiResponse);
                    // Entire menu data into gfMenu
                    gfoCollection = JsonConvert.DeserializeObject<GFO_CollectionsModel>(apiResponse);

                    util.Logger("Collection [# of orders fetched] count = " + gfoCollection.count);

                    if (gfoCollection.count > 0)
                    {
                        util.SaveOrderAsFile(apiResponse);
                        // Populate Order Data into POS Database
                        //if (Populate_POS_Orders() == false)
                        //    {
                        //        util.SaveOrderAsFile(apiResponse);
                        //    }
                    }
                }
                else
                {
                    util.Logger("Order API Error Status = " + t.Status + " " + t.Result);
                }
            //}
            //catch (Exception e)
            //{
            //    util.Logger("Order API Error Exception = " + e.Message);
            //}
            util.Logger("----------------------< ORDER POLLING FINISH >------------------------");
        }
        private void Sync_GF_Order_Process_FromFile()
        {
            util.Logger("----------------------< ORDER LOADING START >------------------------");
            //try
            //{
            //---------------------------------------------
            // Load order files
            try
            {
                string orderPath = ConfigurationManager.AppSettings["ApplicationPath"] + "\\" + ConfigurationManager.AppSettings["OrderPath"];
                util.Logger("Load order files path = " + orderPath);
                string orderfileExt = "*.ord";
                string[] ordfiles = Directory.GetFiles(orderPath, orderfileExt);
                string apiResponse = "";

                foreach (string onefile in ordfiles)
                {
                    FileInfo fi = new FileInfo(onefile);
                    util.Logger("Order file to proceed = " + fi.FullName);
                    apiResponse = util.ReadOrderFromFile(fi.FullName);
                    util.Logger("Order POST API Playload = " + apiResponse);
                    // Entire menu data into gfMenu

                    gfoCollection = JsonConvert.DeserializeObject<GFO_CollectionsModel>(apiResponse);

                    util.Logger("Collection [# of orders fetched] count = " + gfoCollection.count);

                    if (gfoCollection.count > 0)
                    {
                        // Populate Order Data into POS Database

                        if (Populate_POS_Orders() == false)
                        {
                            util.Logger("Order file not completed and Stay as it was = " + fi.FullName);
                        }
                        else
                        {
                            util.Logger("Order file completed and Move = " + fi.FullName);
                            fi.CopyTo(ConfigurationManager.AppSettings["ApplicationPath"] + "\\" + ConfigurationManager.AppSettings["CompletedPath"] + "\\" + fi.Name);
                            fi.Delete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                util.Logger("OOSyncDB failed " + e.Message);
                return;
            }
            //---------------------------------------------
            /*string apiResponse = util.ReadOrderFromFile();
                util.Logger("TEST Order POST API Playload Response = " + apiResponse);
                // Entire menu data into gfMenu
                gfoCollection = JsonConvert.DeserializeObject<GFO_CollectionsModel>(apiResponse);

                util.Logger("Collection [# of orders fetched] count = " + gfoCollection.count);

                if (gfoCollection.count > 0)
                {
                // Populate Order Data into POS Database

                    if (Populate_POS_Orders() == false)
                    {
                        util.SaveOrderAsFile(apiResponse);
                    }
                }*/
            //}
            //catch (Exception e)
            //{
            //    util.Logger("Order API Error Exception = " + e.Message);
            //}
            util.Logger("----------------------< ORDER POLLING FINISH >------------------------");
        }
        private bool Populate_POS_Orders()
        {
            string strPhoneNo = "";
            int intCustomerId = 0;
            int intTableId = 0;
            int intNewInvNo = 0;
            int intNewSeqNo = 0;
            int intTableTranId = 0;
            int intChildTableTranId = 0;
            bool blnIsOrderExists = false;
            bool blnIsParentOpenFood = false;
            double dblTaxAmount = 0;
            double dblTaxAmountOnline = 0;
            double dblTaxDiff = 0;
            double dblTip = 0;
            double dblTipTax = 0;
            DateTime dtmPickup;
            DateTime dtmReservation; //Feature #2676
            DateTime localTime = DateTime.Now;
            DateTime utcTime = DateTime.UtcNow;
            DateTimeOffset localTimeAndOffset = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            string strTableName = "";
            bool blnIsPaidOnline = false;   //Feature #2560
            //int iCCount = 0;
            //foreach (var pClientOrder in gfoCollection.orders)
            //{
            //util.Logger(" ============================================ " + iCCount++);
            //util.Logger(" ==> Client Order Id :" + pClientOrder.id);
            //util.Logger(" ==> Client Order Last Name :" + pClientOrder.client_last_name);
            //util.Logger(" ==> Client Order First Name :" + pClientOrder.client_first_name);
            //util.Logger(" ==> Client Order Phone :" + pClientOrder.client_phone);
            //util.Logger(" ==> Client Order Email :" + pClientOrder.client_email);
            //util.Logger(" ==> Client Order fulfillment_option :" + pClientOrder.fulfillment_option);
            if (gfoCollection.orders.Count > 0)
                {
                    gfoOrders.Clear();
                    gfoOrders = gfoCollection.orders;
                    int iCount = 0;
                    foreach (var pOrder in gfoOrders)
                    {
                        util.Logger("     ============================================ " + iCount++);
                        util.Logger("     > Order Id :" + pOrder.id);
                        util.Logger("     > Order Type :" + pOrder.type);   // pickup or dine_in//Feature #2369
                        util.Logger("     > Order Table# :" + pOrder.table_number);//Feature #2369
                        util.Logger("     > Order client_id :" + pOrder.client_id);
                        util.Logger("     > Order client_first_name :" + pOrder.client_first_name);
                        util.Logger("     > Order client_last_name :" + pOrder.client_last_name);
                        util.Logger("     > Order client_phone :" + pOrder.client_phone);
                        util.Logger("     > Order client_email :" + pOrder.client_email);
                        util.Logger("     > Order Net(SubTotal) Price :" + pOrder.sub_total_price);
                        util.Logger("     > Order Tax Value :" + pOrder.tax_value);
                        util.Logger("     > Order Total Price :" + pOrder.total_price);
                        util.Logger("     > Order Order Status :" + pOrder.status);
                        util.Logger("     > Order Accepted At :" + pOrder.accepted_at);
                        util.Logger("     > Order Fulfill At :" + pOrder.fulfill_at);
                        util.Logger("     > Order instructions :" + pOrder.instructions);
                        util.Logger("     > Order # of Items :" + pOrder.items.Count);
                        //Feature #2560
                        blnIsPaidOnline = false;
                        if (pOrder.used_payment_methods[0] == "ONLINE")
                        {
                            util.Logger("     > Order used_payment_methods :" + pOrder.used_payment_methods[0]);
                            util.Logger("     > Order gateway_transaction_id :" + pOrder.gateway_transaction_id);
                            util.Logger("     > Order gateway_type :" + pOrder.gateway_type);
                            util.Logger("     > Order payment :" + pOrder.payment);
                            util.Logger("     > Order card_type :" + pOrder.card_type);
                            blnIsPaidOnline = true;
                        }
                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
                        // Check Status = "accepted" pickup
                        // Customer Phone # (client_phone) : Remove "+1" if length is 12
                        // Create a Customer if not exist on POS
                        // Get a Empty Table Id from POS
                        // Create a PhoneOrder Record    
                        if (pOrder.status == "accepted")
                        {
                            //-----------------------------------------------------------------------
                            // CHECKING / CREATE or UPDATE CUSTOMER INFO
                            strPhoneNo = pOrder.client_phone;
                            if ((strPhoneNo.Length == 12) && (strPhoneNo.Substring(0, 2) == "+1"))
                            {
                                strPhoneNo = strPhoneNo.Substring(2);
                            }
                            posCustomer.FirstName = pOrder.client_first_name;
                            posCustomer.LastName = pOrder.client_last_name;
                            if (pOrder.client_first_name.Length > 20) posCustomer.FirstName = pOrder.client_first_name.Substring(0, 20);
                            if (pOrder.client_last_name.Length > 20) posCustomer.LastName = pOrder.client_last_name.Substring(0, 20);
                            posCustomer.Phone = strPhoneNo;             // Without "+1" Country Code
                            posCustomer.Email = pOrder.client_email;
                            posCustomer.Address1 = "";
                            posCustomer.Address2 = "";
                            posCustomer.Zip = "";
                            posCustomer.DateMarried = "";
                            posCustomer.DateOfBirth = "";
                            if (pOrder.instructions.Length > 300)
                            {
                                posCustomer.Memo = pOrder.instructions.Substring(0, 300);
                            }
                            else
                            {
                                posCustomer.Memo = pOrder.instructions;
                            }
                            posCustomer.WebId = 0;
                            intCustomerId = Add_Or_Update_Customer_on_POS(posCustomer);
                            intTableId = 0;
                            strTableName = "";
                            //-----------------------------------------------------------------------
                            // ONLINE PICKUP ORDER
                            // GETTING EMPTY ONLINE TABLE ID or CHECKING DUPLICATE ONLINE ORDER
                            // Feature #2369
                            if ((intCustomerId > 0) && (pOrder.type == "pickup"))
                            {
                                intTableId = POS_Get_Empty_PhoneOrder_TableId();
                                // Check POS database also required : PhoneOrder Table
                                blnIsOrderExists = POS_Check_DuplicateOrder(pOrder.id);
                                if (!blnIsOrderExists)
                                {
                                    // Check POS1 database also required : PhoneTranComplete Table
                                    blnIsOrderExists = POS1_Check_DuplicateOrder(pOrder.id);
                                }
                                if ((intTableId > 0) && (!blnIsOrderExists))
                                {
                                    posPhoneOrder.TableId = intTableId;
                                    posPhoneOrder.CustomerId = intCustomerId;
                                    posPhoneOrder.CustomerName = pOrder.client_first_name + " " + pOrder.client_last_name;
                                    if (posPhoneOrder.CustomerName.Length > 20)
                                    {
                                        posPhoneOrder.CustomerName = posPhoneOrder.CustomerName.Substring(0, 20);
                                    }
                                    posPhoneOrder.Phone = strPhoneNo;
                                    posPhoneOrder.OrderDate = pOrder.accepted_at.Substring(0, 10);
                                    dtmPickup = DateTime.SpecifyKind(DateTime.Parse(pOrder.fulfill_at), DateTimeKind.Utc);
                                    util.Logger("     > Order fulfill_at Online :" + pOrder.fulfill_at);
                                    util.Logger("     > Order fulfill_at UTC :" + dtmPickup.ToString("yyyy-MM-dd HH:mm:ss"));
                                    //util.Logger("     > Order fulfill_at Local :" + dtmPickup.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                                    posPhoneOrder.PickDate = dtmPickup.ToString("yyyy-MM-dd"); //pOrder.fulfill_at.Substring(0, 10);
                                    posPhoneOrder.PickTime = dtmPickup.ToString("HH:mm:ss"); //pOrder.fulfill_at.Substring(11,8);
                                                                                             //posPhoneOrder.Memo = pOrder.instructions;
                                    if (pOrder.instructions.Length > 300)
                                    {
                                        posPhoneOrder.Memo = pOrder.instructions.Substring(0, 300);
                                    }
                                    else
                                    {
                                        posPhoneOrder.Memo = pOrder.instructions;
                                    }
                                    posPhoneOrder.CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    posPhoneOrder.CreateTime = DateTime.Now.ToString("HH:mm:ss");
                                    posPhoneOrder.CreatePasswordCode = "9999";
                                    posPhoneOrder.CreatePasswordName = pOrder.id.ToString();   // @@@@@@ to save order-id on Phoneorder;
                                    posPhoneOrder.CreateStation = "MAIN";
                                    posPhoneOrder.LastModDate = DateTime.Now.ToString("yyyy-MM-dd");
                                    posPhoneOrder.LastModTime = DateTime.Now.ToString("HH:mm:ss");
                                    posPhoneOrder.LastModPasswordCode = "9999";       // @@@@@@ to save order-id on Phoneorder
                                    posPhoneOrder.LastModPasswordName = "SVC";
                                    posPhoneOrder.LastModStation = "MAIN";
                                    if (POS_Add_PhoneOrder(posPhoneOrder) < 0)
                                    {
                                        intTableId = -1;
                                        util.Logger("     > ### Unable To add Phone Order ### posPhoneOrder.TableId " + posPhoneOrder.TableId);
                                    }
                                }
                                else
                                {
                                    if (intTableId == 0)
                                    {
                                        util.Logger("     > ### Unable To Assign a ONLINE Table ### No empty table :" + pOrder.id);
                                        return false;
                                    }
                                    util.Logger("     > ### Unable To Assign a ONLINE Table ### Duplicate > Order Id :" + pOrder.id);
                                    util.Logger("     > ### Skipping ### Order.Id" + pOrder.id);
                                    continue;
                                }
                            }
                            //-----------------------------------------------------------------------
                            // DINE-IN ORDER PROCESSING
                            // CHECKING POS.TableLayout with TableType=1 and TableName for DINE-IN
                            // Feature #2369
                            posTableLayout.Id = 0;
                            posTableLayout.TableName = "";

                            //pOrder.type == "pickup"
                            //pOrder.type == "dine_in"
                            if ((intCustomerId > 0) && (pOrder.type == "dine_in") && (pOrder.table_number.Length > 0))
                            {
                                posTableLayout = POS_Get_DiningTable_TableId(pOrder.table_number);
                                intTableId = posTableLayout.Id;
                                strTableName = posTableLayout.TableName;
                                /*blnIsOrderExists = POS_Check_DuplicateDineInOrder(pOrder.id);
                                if (!blnIsOrderExists)
                                {
                                    // Check POS1 database also required : PhoneTranComplete Table
                                    blnIsOrderExists = POS1_Check_DuplicateOrder(pOrder.id);
                                }*/
                                //if ((intTableId > 0) && (!blnIsOrderExists))
                                if (intTableId > 0)
                                {
                                }
                                else
                                {
                                    if (intTableId == 0)
                                    {
                                        util.Logger("     > ### Unable To Assign a DINE-IN Table ### No table matched :" + pOrder.id);
                                        return false;
                                    }
                                    util.Logger("     > ### Unable To Assign a DINE-IN Table ### Duplicate > Order Id :" + pOrder.id);
                                    util.Logger("     > ### Skipping ### Order.Id" + pOrder.id);
                                    continue;
                                }
                            }

                        //-----------------------------------------------------------------------
                        // TABLE RESERVATION PROCESSING //Feature #2676
                        // CHECKING POS.TableLayout with TableType=1 and TableName for DINE-IN
                        // Feature #2369
                        //pOrder.type == "pickup"
                        //pOrder.type == "dine_in"
                        //pOrder.type == "table_reservation"
                            if ((intCustomerId > 0) && (pOrder.type == "table_reservation"))
                            {
                                posReservation.CustID = intCustomerId;
                                posReservation.CustName = posCustomer.FirstName + ", " + posCustomer.LastName;
                                posReservation.Phone = posCustomer.Phone;
                                dtmReservation = DateTime.SpecifyKind(DateTime.Parse(pOrder.fulfill_at), DateTimeKind.Utc);
                                util.Logger("     > Table Reservation fulfill_at Online :" + pOrder.fulfill_at);
                                util.Logger("     > Table Reservation_at UTC :" + dtmReservation.ToString("yyyy-MM-dd HH:mm:ss"));
                                posReservation.ReservDate = dtmReservation.ToString("yyyy-MM-dd"); //pOrder.fulfill_at.Substring(0, 10);
                                posReservation.ReservTime = dtmReservation.ToString("HH:mm:ss"); //pOrder.fulfill_at.Substring(11,8);
                                posReservation.Memo = pOrder.instructions;
                                posReservation.NoOfPeople = pOrder.persons;

                                POS_Add_Reservation(posReservation);
                                util.Logger("     > Table Reservation Added into POS :" + posReservation.CustName + " at " + 
                                                                                  posReservation.ReservDate + " with " +
                                                                                  posReservation.NoOfPeople + " (" +
                                                                                  posReservation.Memo + ")" 
                                                                                    );
                                continue;
                            }

                        }   
                        else
                        {
                            util.Logger("     > ### Skipping non-Accepted order ### Order.Id" + pOrder.id);
                            continue;
                        }
                        // Get Invoice No, SeqNo
                        intNewInvNo = POS_Get_NewInvoiceNo();
                        intNewSeqNo = POS_Get_NewSeqNo();
                        if ((pOrder.items.Count > 0) && (intTableId > 0))
                        {
                            gfoItems.Clear();
                            gfoItems = pOrder.items;
                            int i = 0;
                            dblTip = 0;
                            dblTipTax = 0;
                            foreach (var pItem in gfoItems)
                            {
                                util.Logger("        ---------------------------------------------- " + i++);
                                util.Logger("        -> Item Id :" + pItem.id);
                                util.Logger("        -> Item type_id :" + pItem.type_id);
                                util.Logger("        -> Item name :" + pItem.name);
                                util.Logger("        -> Item quantity :" + pItem.quantity);
                                util.Logger("        -> Item price :" + pItem.price);
                                util.Logger("        -> Item total_item_price :" + pItem.total_item_price);
                                util.Logger("        -> Item tax_value :" + pItem.tax_value);
                                util.Logger("        -> Item instructions :" + pItem.instructions);
                                util.Logger("        -> Item options.Count :" + pItem.options.Count);
                                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                // Check GF_MenuItems for matching POS Product
                                // If POS matching does not exist, Transaction should use 'OPENFOOD' product id.
                                // Create a TableTran (Parent)
                                // Add memo if instruction exists
                                if ((pItem.type_id == null) && (pItem.name == "TIP"))  // TIP
                                {
                                    dblTip = pItem.total_item_price;
                                    dblTipTax = pItem.tax_value;
                                    continue;
                                }
                            
                                posProducts = POS_Get_Matching_Product(pItem.type_id);
                                if (posProducts.Count > 0)
                                {
                                    posProduct = posProducts[0];
                                    // Matched POS Product Found
                                    intTableTranId = POS_Add_TableTran(pOrder, pItem, posProduct, intNewInvNo, intNewSeqNo, intTableId, strTableName, blnIsPaidOnline);
                                    util.Logger("        >> Matched TableTran Created :" + intTableTranId + ", ProdId=" + 
                                                            posProduct.Id + ", ProductName=" + 
                                                            posProduct.ProductName + ", InvNo=" +
                                                            intNewInvNo);
                                    blnIsParentOpenFood = false;
                                }
                                else
                                {
                                    // Not Matched ? Tran Should be created as OpenFood
                                    // Check Online OpenFood
                                    posProducts = POS_Get_OnlineOpenFood_Product();
                                    if (posProducts.Count > 0)
                                    {
                                        posProduct = posProducts[0];
                                        // Matched POS Product Found
                                        posProduct.ProductName = pItem.name;
                                        posProduct.SecondName = pItem.name;
                                        posProduct.OutUnitPrice = pItem.price;
                                        if (pItem.name.Length > 50)
                                        {
                                            posProduct.ProductName = pItem.name.Substring(0,50);
                                            posProduct.SecondName = pItem.name.Substring(0,50);
                                        }
                                        if (pItem.item_discount > 0 && pItem.cart_discount < 0)
                                        {
                                            // Cart discount
                                            posProduct.IsTax1 = false;
                                            posProduct.IsTax2 = false;
                                            posProduct.IsTax3 = false;
                                            posProduct.OutUnitPrice = pItem.cart_discount;
                                        }
                                        // if posGF_MenuItem.IsPrinter1 is not null, set posPoduct.IsPrinter1 with posGF_MenuItem.IsPrinter1
                                        // Feature #3036
                                        posGF_MenuItem = POS_Get_GFMenuItem_By_Id(pItem.type_id);
                                        if (posGF_MenuItem != null)
                                        {
                                            if (posGF_MenuItem.Match_POS_Id == 0)
                                            {
                                                if (posGF_MenuItem.IsPrinter1.HasValue) posProduct.IsPrinter1 = posGF_MenuItem.IsPrinter1.Value;
                                                if (posGF_MenuItem.IsPrinter2.HasValue) posProduct.IsPrinter2 = posGF_MenuItem.IsPrinter2.Value;
                                                if (posGF_MenuItem.IsPrinter3.HasValue) posProduct.IsPrinter3 = posGF_MenuItem.IsPrinter3.Value;
                                                if (posGF_MenuItem.IsPrinter4.HasValue) posProduct.IsPrinter4 = posGF_MenuItem.IsPrinter4.Value;
                                                if (posGF_MenuItem.IsPrinter5.HasValue) posProduct.IsPrinter5 = posGF_MenuItem.IsPrinter5.Value;
                                                if (posGF_MenuItem.IsPrinter6.HasValue) posProduct.IsPrinter6 = posGF_MenuItem.IsPrinter6.Value;
                                            }
                                        }
                                        //Feature #3031
                                        util.Logger("           => SysConfig Service Fee ? " + strServiceFeeName);
                                        if (strServiceFeeName == posProduct.ProductName)
                                        {
                                            util.Logger("           => Service Fee matched  ? " + strServiceFeeName + " " + posProduct.ProductName);
                                            // Printer Setting to only EPSON-1
                                            posProduct.IsPrinter1 = true;
                                            posProduct.IsPrinter2 = false;
                                            posProduct.IsPrinter3 = false;
                                            posProduct.IsPrinter4 = false;
                                            posProduct.IsPrinter5 = false;
                                            posProduct.IsPrinter6 = false;
                                        }
                                        intTableTranId = POS_Add_TableTran(pOrder, pItem, posProduct, intNewInvNo, intNewSeqNo, intTableId, strTableName, blnIsPaidOnline);
                                        util.Logger("        >> OpenFood TableTran Created :" + intTableTranId + ", ProdId=" +
                                                                posProduct.Id + ", ProductName=" +
                                                                posProduct.ProductName + ", InvNo=" +
                                                                intNewInvNo);
                                    }
                                    blnIsParentOpenFood = true;
                                }
                                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                // If pItem.instructions is not null, add memo to TableTran Feature #3269
                                if (pItem.instructions.Length > 0)
                                {
                                    POS_Add_TableTranMemo(intTableTranId, pItem.instructions);
                                }

                                if (pItem.options.Count > 0)
                                {
                                    gfoOptions.Clear();
                                    gfoOptions = pItem.options;
                                    int o = 0;
                                    foreach (var pOption in gfoOptions)
                                    {
                                        util.Logger("           ....................................... " + o++);
                                        util.Logger("           => Option Id :" + pOption.id);
                                        util.Logger("           => Option type_id :" + pOption.type_id);
                                        util.Logger("           => Option type :" + pOption.type);
                                        util.Logger("           => Option group_name :" + pOption.group_name);
                                        util.Logger("           => Option name :" + pOption.name);
                                        util.Logger("           => Option quantity :" + pOption.quantity);
                                        util.Logger("           => Option price :" + pOption.price);
                                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                        // Check GF_MenuItems for matching POS Product
                                        // Create a TableTran (Child)
                                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++//
                                        // Check GF_MenuItems for matching POS Product
                                        // If POS matching does not exist, Transaction should use 'OPENFOOD' product id.
                                        // Create a TableTran (Parent)
                                        // Add memo if instruction exists
                                        posProducts = POS_Get_Matching_Product(pOption.type_id);
                                        if (posProducts.Count > 0)
                                        {
                                            posProduct = posProducts[0];

                                            // Bug #2216 ??
                                            util.Logger("           => Item quantity ? :" + pItem.quantity);
                                            if (pItem.quantity > 1) 
                                            {
                                                pOption.quantity = pItem.quantity;
                                                util.Logger("           => Option quantity Change :" + pOption.quantity);
                                            }

                                            // Matched POS Product Found
                                            intChildTableTranId = POS_Add_Child_TableTran(pOrder, pOption, posProduct, intNewInvNo, intNewSeqNo, intTableId, 
                                                                                            intTableTranId, blnIsParentOpenFood, strTableName, blnIsPaidOnline);
                                            util.Logger("        >> Matched TableTran Child Created :" + intTableTranId + ",ChildTran=" + 
                                                                    intChildTableTranId + ", ProdId=" +
                                                                    posProduct.Id + ", ProductName=" +
                                                                    posProduct.ProductName + ", InvNo=" +
                                                                    intNewInvNo);
                                        }
                                        else
                                        {
                                            // Not Matched ? Tran Should be created as OpenFood
                                            // Check OpenFood
                                            posProducts = POS_Get_OnlineOpenFood_Product();
                                            if (posProducts.Count > 0)
                                            {
                                                posProduct = posProducts[0];                                                // Matched POS Product Found
                                                posProduct.ProductName = pOption.name;
                                                posProduct.SecondName = pOption.name;
                                                if (pOption.name.Length > 50)
                                                {
                                                    posProduct.ProductName = pOption.name.Substring(0, 50);
                                                    posProduct.SecondName = pOption.name.Substring(0, 50);
                                                }
                                                posProduct.OutUnitPrice = pItem.price;

                                                // if posGF_MenuItem.IsPrinter1 is not null, set posPoduct.IsPrinter1 with posGF_MenuItem.IsPrinter1
                                                // Feature #3036
                                                posGF_MenuItem = POS_Get_GFMenuItem_By_Id(pOption.type_id);
                                                if (posGF_MenuItem != null)
                                                {
                                                        if (posGF_MenuItem.Match_POS_Id == 0)
                                                        {
                                                            //Bug #3295 
                                                            if ((posGF_MenuItem.IsPrinter1.HasValue) && (posGF_MenuItem.IsPrinter1 == true))
                                                            {
                                                                posProduct.IsPrinter1 = posGF_MenuItem.IsPrinter1.Value;
                                                            }
                                                            if ((posGF_MenuItem.IsPrinter2.HasValue) && (posGF_MenuItem.IsPrinter2 == true))
                                                            {
                                                                posProduct.IsPrinter2 = posGF_MenuItem.IsPrinter2.Value;
                                                            }
                                                            if ((posGF_MenuItem.IsPrinter3.HasValue) && (posGF_MenuItem.IsPrinter3 == true))
                                                            {
                                                                posProduct.IsPrinter3 = posGF_MenuItem.IsPrinter3.Value;
                                                            }
                                                            if ((posGF_MenuItem.IsPrinter4.HasValue) && (posGF_MenuItem.IsPrinter4 == true))
                                                            {
                                                                posProduct.IsPrinter4 = posGF_MenuItem.IsPrinter4.Value;
                                                            }
                                                            if ((posGF_MenuItem.IsPrinter5.HasValue) && (posGF_MenuItem.IsPrinter5 == true))
                                                            {
                                                                posProduct.IsPrinter5 = posGF_MenuItem.IsPrinter5.Value;
                                                            }
                                                            if ((posGF_MenuItem.IsPrinter6.HasValue) && (posGF_MenuItem.IsPrinter6 == true)) 
                                                            { 
                                                                posProduct.IsPrinter6 = posGF_MenuItem.IsPrinter6.Value; 
                                                            }
                                                        }
                                                }   
                                                // Bug #2216 ??
                                                // pOption.quantity = pItem.quantity;
                                                util.Logger("           => Item quantity ? :" + pItem.quantity);
                                                if (pItem.quantity > 1)
                                                {
                                                    pOption.quantity = pItem.quantity;
                                                    util.Logger("           => Option quantity Change :" + pOption.quantity);
                                                }

                                                intChildTableTranId = POS_Add_Child_TableTran(pOrder, pOption, posProduct, intNewInvNo, intNewSeqNo, 
                                                                                            intTableId, intTableTranId, blnIsParentOpenFood, strTableName, blnIsPaidOnline);
                                                util.Logger("        >> UnMatched TableTran Child Created :" + intTableTranId + ",ChildTran=" +
                                                                        intChildTableTranId + ", ProdId=" +
                                                                        posProduct.Id + ", ProductName=" +
                                                                        posProduct.ProductName + ", InvNo=" +
                                                                        intNewInvNo);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        ////////////////////////////////////////////////////////////////////
                        // Tax diff amount to Tax1 as DISCOUNT Tran
                        dblTaxDiff = 0;
                        dblTaxAmount = POS_Get_Tax_Amount_ByTable(intTableId);
                        dblTaxAmountOnline = Math.Round(pOrder.tax_value,2);

                        // //////////////////////////////////////////////////////////////////
                        // Compare pOrder.total_price with POS Invoice Total
                        // If not matched, add a discount transaction
                        ////////////////////////////////////////////////////////////////////
                        ///
                        double dblTotalAmount = POS_Get_Total_Amount_ByTable(intTableId);
                        //Bug #3251
                        //double dblTotalDiff = pOrder.total_price - dblTotalAmount
                        double dblTotalDiff = pOrder.total_price - dblTotalAmount - dblTip;
                        dblTotalDiff = Math.Round(dblTotalDiff, 2);
                        //Bug #3251
                        util.Logger("     > Total Tip: " + dblTip + " , TipTax " + dblTipTax + " = " + dblTip + dblTipTax);
                        util.Logger("     > Total Diff (Online vs POS): " + pOrder.total_price + " vs " + dblTotalAmount + " = " + dblTotalDiff);
                        
                        dblTaxDiff = dblTaxAmountOnline - dblTaxAmount - dblTipTax;     // Exclude TipTax
                        dblTaxDiff = Math.Round(dblTaxDiff, 2);
                        util.Logger("     > Tax Diff (Online vs POS): " + dblTaxAmountOnline + " vs " + dblTaxAmount + " = " + dblTaxDiff);
                        
                        if (dblTaxDiff != 0)
                        {
                            POS_Add_DiscountTableTran(pOrder, intNewInvNo, intNewSeqNo, intTableId, dblTotalDiff - dblTaxDiff, dblTaxDiff, blnIsPaidOnline);
                        }
                        //Bug #3251
                        if (dblTip + dblTipTax != 0)
                        {
                            // Add Tip TableTran
                            POS_Add_TipTableTran(pOrder, intNewInvNo, intNewSeqNo, intTableId, dblTip, dblTipTax, blnIsPaidOnline);
                        }   


                }
            }
            //}
            return true;

        }
        //Feature #3269
        private int POS_Add_TableTranMemo(int intTableTranId, string instructions)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_TableTranMemo(intTableTranId, instructions);
        }

        //Bug #3251
        private int POS_Add_TipTableTran(GFO_OrdersModel pOrder, int intNewInvNo, int intNewSeqNo, int intTableId, double dblTip, double dblTipTax, bool blnIsPaidOnline)
        {
            DateTime dtmTemp;
            //POS_Add_TableTran(pItem, posProduct, intNewInvNo, intNewSeqNo);
            posTran.ParentTranId = 0;
            posTran.TranType = "90";
            posTran.ProductId = 0;
            posTran.ProductName = "Tip";
            posTran.SecondName = "Tip";
            posTran.ProductTypeId = 0;
            posTran.InUnitPrice = 0;
            posTran.OutUnitPrice = 0;
            posTran.IsTax1 = false;
            posTran.IsTax2 = false;
            posTran.IsTax3 = false;
            posTran.IsTaxInverseCalculation = false;
            posTran.IsPrinter1 = false;
            posTran.IsPrinter2 = false;
            posTran.IsPrinter3 = false;
            posTran.IsPrinter4 = false;
            posTran.IsPrinter5 = false;
            posTran.IsPrinter6 = false;
            posTran.Printer1Qty = 0;
            posTran.Printer2Qty = 0;
            posTran.Printer3Qty = 0;
            posTran.Printer4Qty = 0;
            posTran.Printer5Qty = 0;
            posTran.Printer6Qty = 0;
            posTran.PromoStartDate = "";
            posTran.PromoEndDate = "";
            posTran.PromoDay1 = 0;
            posTran.PromoDay2 = 0;
            posTran.PromoDay3 = 0;
            posTran.PromoPrice1 = 0;
            posTran.PromoPrice2 = 0;
            posTran.PromoPrice3 = 0;
            posTran.IsKitchenItem = false;
            posTran.IsSushiBarItem = false;
            posTran.ManualName = "";
            posTran.DCMethod = 0;
            posTran.Price = 0; // pItem.price;
            posTran.Quantity = 1;
            posTran.Amount = (float)dblTip; //dblTotalDiff; // pItem.total_item_price;
            
            posTax = POS_Get_Tax("TAX");
            if (posTax.Tax1 > 0)
            {
                posTran.Tax1Rate = posTax.Tax1 / 100;
                posTran.IsTax1 = true;
            }
            else
            {
                posTran.Tax1Rate = 0;
                posTran.IsTax1 = false;
            }
            if (posTax.Tax2 > 0)
            {
                posTran.Tax2Rate = posTax.Tax2 / 100;
                posTran.IsTax1 = true;
            }
            else
            {
                posTran.Tax2Rate = 0;
                posTran.IsTax1 = false;
            }
            if (posTax.Tax3 > 0)
            {
                posTran.Tax3Rate = posTax.Tax3 / 100;
                posTran.IsTax1 = true;
            }
            else
            {
                posTran.Tax3Rate = 0;
                posTran.IsTax1 = false;
            }
            posTran.Tax1 = (float)dblTipTax;
            posTran.Tax2 = 0;
            posTran.Tax3 = 0;

            posTran.TableId = intTableId;
            posTran.TableName = "ONLINE " + (intTableId - Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]) + 1);
            posTran.SplitId = 1;
            posTran.OldTableId = intTableId;
            posTran.InvoiceNo = intNewInvNo;
            posTran.NumInvPrt = 0;
            posTran.IsOrdered = false;
            posTran.IsAdditionalOrder = true;
            posTran.OrderSeqNo = intNewSeqNo;
            posTran.OrderNumPeople = 0;
            dtmTemp = DateTime.Parse(pOrder.accepted_at);
            posTran.OrderDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.OrderTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.OrderPasswordCode = "9999";
            posTran.OrderPasswordName = "ONLINE";
            posTran.OrderStation = "MAIN";
            posTran.IsCancelled = false;
            posTran.CancelDate = "";
            posTran.CancelTime = "";
            posTran.IsCancelPending = false;
            posTran.CancelPrintDate = "";
            posTran.CancelPrintTime = "";
            posTran.IsOK = false;
            posTran.IsCooked = false;
            posTran.IsPaidStarted = false;
            posTran.StartReceiptNo = 0;

            //Feature #2560
            if (blnIsPaidOnline)
            {
                //Feature #3222
                //posTran.PaidType = "ON";
                posTran.PaidType = pOrder.card_type.Substring(0, 2);    //store only two characters into PaidType
                posTran.IsPaidComplete = true;
                posTran.PaidStartDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
                posTran.PaidStartTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            }
            else
            {
                posTran.PaidType = "";
                posTran.IsPaidComplete = false;
                posTran.PaidStartDate = "";
                posTran.PaidStartTime = "";
            }
            posTran.CompleteReceiptNo = 0;
            posTran.CompleteDate = "";
            posTran.CompleteTime = "";
            //posTran.CreateDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.CreateTime = pOrder.accepted_at.Substring(11, 8);
            posTran.CreateDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.CreateTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.CreatePasswordCode = "9999";
            posTran.CreatePasswordName = "ONLINE";
            posTran.CreateStation = "MAIN";
            //posTran.LastModDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.LastModTime = pOrder.accepted_at.Substring(11, 8);
            posTran.LastModDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.LastModTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.LastModPasswordCode = "9999";
            posTran.LastModPasswordName = "ONLINE";
            posTran.LastModStation = "MAIN";
            posTran.IsRounding = false;
            posTran.SplitTranId = 0;
            posTran.SplitTranItemId = 0;
            posTran.SplitTranItemSplitId = 0;
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_TableTran(posTran);
        }

        private POS_GF_MenuItemsModel POS_Get_GFMenuItem_By_Id(int? item_id)
        {
            if (item_id != null)
            {
                int iItemId = item_id.Value;
                DataAccessPOS dbPos = new DataAccessPOS();
                List<POS_GF_MenuItemsModel> posGF_MenuItems = dbPos.Get_GF_MenuItems_ById(iItemId);
                if (posGF_MenuItems != null)
                {
                    if (posGF_MenuItems.Count > 0)
                    {
                        return posGF_MenuItems[0];
                    }
                    return null;
                }
                //dbPos.Get_GF_MenuItems_ById(iItemId)[0]
                return null;
            }
            else
            { return null; }    
        }
        private double POS_Get_Total_Amount_ByTable(int intTableId)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Total_Amount_ByTable(intTableId);
        }
        private POS_TableLayoutModel POS_Get_DiningTable_TableId(string strTable_number)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_DiningTableId_ByTableName(strTable_number);
        }
        private double POS_Get_Tax_Amount_ByTable(int intTableId)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Tax_Amount_ByTable(intTableId);
        }

        private bool POS1_Check_DuplicateOrder(int iOnlineOrderid)
        {
            DataAccessPOS1 dbPos1 = new DataAccessPOS1();
            return dbPos1.Check_Online_DuplicateOrder(iOnlineOrderid);
        }

        private bool POS_Check_DuplicateOrder(int iOnlineOrderid)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Check_Online_DuplicateOrder(iOnlineOrderid);
        }

        private List<POS_ProductModel> POS_Get_OnlineOpenFood_Product()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Product_OnlineOpenFood();
        }

        private POS_TaxModel POS_Get_Tax(string taxCode)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Tax_By_TaxCode(taxCode);
        }

        private int POS_Add_TableTran(GFO_OrdersModel pOrder, GFO_OrderItemsModel pItem, 
                                      POS_ProductModel posProduct, int intNewInvNo, int intNewSeqNo, int intTableId,
                                      string strTableName, bool blnIsPaidOnline)
        {
            DateTime dtmTemp;
            //POS_Add_TableTran(pItem, posProduct, intNewInvNo, intNewSeqNo);
            posTran.ParentTranId = 0;
            posTran.TranType = "20";
            posTran.ProductId = posProduct.Id;
            posTran.ProductName = posProduct.ProductName;
            posTran.SecondName = posProduct.SecondName;
            posTran.ProductTypeId = posProduct.ProductTypeId;
            posTran.InUnitPrice = posProduct.InUnitPrice;
            posTran.OutUnitPrice = pItem.price;
            posTran.IsTax1 = posProduct.IsTax1;
            posTran.IsTax2 = posProduct.IsTax2;
            posTran.IsTax3 = posProduct.IsTax3;
            posTran.IsTaxInverseCalculation = posProduct.IsTaxInverseCalculation;
            posTran.IsTaxInverseCalculation = posProduct.IsTaxInverseCalculation;
            posTran.IsPrinter1 = posProduct.IsPrinter1;
            posTran.IsPrinter2 = posProduct.IsPrinter2;
            posTran.IsPrinter3 = posProduct.IsPrinter3;
            posTran.IsPrinter4 = posProduct.IsPrinter4;
            posTran.IsPrinter5 = posProduct.IsPrinter5;
            posTran.IsPrinter6 = posProduct.IsPrinter6;
            posTran.Printer1Qty = 0;
            posTran.Printer2Qty = 0;
            posTran.Printer3Qty = 0;
            posTran.Printer4Qty = 0;
            posTran.Printer5Qty = 0;
            posTran.Printer6Qty = 0;
            posTran.PromoStartDate = posProduct.PromoStartDate;
            posTran.PromoEndDate = posProduct.PromoEndDate;
            posTran.PromoDay1 = posProduct.PromoDay1;
            posTran.PromoDay2 = posProduct.PromoDay2;
            posTran.PromoDay3 = posProduct.PromoDay3;
            posTran.PromoPrice1 = posProduct.PromoPrice1;
            posTran.PromoPrice2 = posProduct.PromoPrice2;
            posTran.PromoPrice3 = posProduct.PromoPrice3;
            posTran.IsKitchenItem = posProduct.IsKitchenItem;
            posTran.IsSushiBarItem = posProduct.IsSushiBarItem;
            posTran.ManualName = "";
            posTran.DCMethod = 0;
            posTran.Price =  pItem.price; //posProduct.OutUnitPrice; 
            posTran.Quantity = pItem.quantity;
            //posTran.Amount = posProduct.OutUnitPrice * pItem.quantity; // pItem.total_item_price;
            posTran.Amount = pItem.price * pItem.quantity; // pItem.total_item_price;
            if (!string.IsNullOrEmpty(posProduct.TaxCode))
            {
                posTax = POS_Get_Tax(posProduct.TaxCode);
                if (posTax.Tax1 > 0)
                {
                    posTran.Tax1Rate = posTax.Tax1 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax1Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax2 > 0)
                {
                    posTran.Tax2Rate = posTax.Tax2 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax2Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax3 > 0)
                {
                    posTran.Tax3Rate = posTax.Tax3 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax3Rate = 0;
                    posTran.IsTax1 = false;
                }
            }
            else
            {
                posTax = POS_Get_Tax("TAX");
                posTran.Tax1Rate = posTax.Tax1 / 100;
                posTran.Tax2Rate = posTax.Tax2 / 100;
                posTran.Tax3Rate = posTax.Tax3 / 100;
            }
            posTran.Tax1 = 0;
            posTran.Tax2 = 0;
            posTran.Tax3 = 0;
            if (posTran.IsTax1) posTran.Tax1 = posTran.Amount * posTran.Tax1Rate;
            if (posTran.IsTax2) posTran.Tax2 = posTran.Amount * posTran.Tax2Rate;
            if (posTran.IsTax3) posTran.Tax3 = posTran.Amount * posTran.Tax3Rate;

            posTran.TableId = intTableId;
            int iOnlineTableStart = Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]);
            if (iOnlineTableStart > intTableId)
            {
                posTran.TableName = strTableName;
            }
            else
            {
                posTran.TableName = "ONLINE " + (intTableId - iOnlineTableStart + 1);
            }
            posTran.SplitId = 1;
            posTran.OldTableId = intTableId;
            posTran.InvoiceNo = intNewInvNo;
            posTran.NumInvPrt = 0;
            posTran.IsOrdered = false;
            posTran.IsAdditionalOrder = true;
            posTran.OrderSeqNo = intNewSeqNo;
            posTran.OrderNumPeople = 0;
            dtmTemp = DateTime.Parse(pOrder.accepted_at);
            posTran.OrderDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.OrderTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.OrderPasswordCode = "9999";
            posTran.OrderPasswordName = "ONLINE";
            posTran.OrderStation = "MAIN";
            posTran.IsCancelled = false;
            posTran.CancelDate = "";
            posTran.CancelTime = "";
            posTran.IsCancelPending = false;
            posTran.CancelPrintDate = "";
            posTran.CancelPrintTime = "";
            posTran.IsOK = false;
            posTran.IsCooked = false;
            posTran.IsPaidStarted = false;
            //Feature #2560
            if (blnIsPaidOnline)
            {
                //Feature #3222
                //posTran.PaidType = "ON";
                //https://github.com/GlobalFood/integration_docs/blob/master/accepted_orders/README.md
                // pOrder.card_type : Only used when food clients pay by card(Excluding PayPal / iDEAL / ApplePay / G - Pay). Only used when field payment is ONLINE.Possible values (but not limited to):
                //- visa
                //- mastercard
                //- discover
                //- american_express
                //- diners_club
                //- maestro
                //- switch
                //- dankort
                //- jcb
                //- carnet
                //- elo
                //- cabal
                //- unionpay
                //- naranja
                //- alelo
                //- alia
                posTran.PaidType = pOrder.card_type.Substring(0,2); //store only two characters into PaidType
                posTran.IsPaidComplete = true;
                posTran.PaidStartDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
                posTran.PaidStartTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            }
            else
            {
                posTran.PaidType = "";
                posTran.IsPaidComplete = false;
                posTran.PaidStartDate = "";
                posTran.PaidStartTime = "";
            }
            posTran.StartReceiptNo = 0;

            posTran.CompleteReceiptNo = 0;
            posTran.CompleteDate = "";
            posTran.CompleteTime = "";
            dtmTemp = DateTime.Parse(pOrder.accepted_at);
            posTran.CreateDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.CreateTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            //posTran.CreateDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.CreateTime = pOrder.accepted_at.Substring(11, 8);
            posTran.CreatePasswordCode = "9999";
            posTran.CreatePasswordName = "ONLINE";
            posTran.CreateStation = "MAIN";
            posTran.LastModDate = dtmTemp.ToString("yyyy-MM-dd"); //pOrder.accepted_at.Substring(0, 10);
            posTran.LastModTime = dtmTemp.ToString("HH:mm:ss"); //pOrder.accepted_at.Substring(11, 8);
            posTran.LastModPasswordCode = "9999";
            posTran.LastModPasswordName = "ONLINE";
            posTran.LastModStation = "MAIN";
            posTran.IsRounding = false;
            posTran.SplitTranId = 0;
            posTran.SplitTranItemId = 0;
            posTran.SplitTranItemSplitId = 0;
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_TableTran(posTran);
        }
        private int POS_Add_Child_TableTran(GFO_OrdersModel pOrder, GFO_ItemOptionsModel pOption,
                                      POS_ProductModel posProduct, int intNewInvNo, int intNewSeqNo, int intTableId, int intParentTranId,
                                      bool blnIsParentOpenFood, string strTableName, bool blnIsPaidOnline)
        {
            DateTime dtmTemp;
            //POS_Add_TableTran(pItem, posProduct, intNewInvNo, intNewSeqNo);
            posTran.ParentTranId = intParentTranId;
            posTran.TranType = "20";
            posTran.ProductId = posProduct.Id;
            posTran.ProductName = posProduct.ProductName;
            posTran.SecondName = posProduct.SecondName;
            posTran.ProductTypeId = posProduct.ProductTypeId;
            posTran.InUnitPrice = posProduct.InUnitPrice;
            posTran.OutUnitPrice = pOption.price;
            posTran.IsTax1 = posProduct.IsTax1;
            posTran.IsTax2 = posProduct.IsTax2;
            posTran.IsTax3 = posProduct.IsTax3;
            posTran.IsTaxInverseCalculation = posProduct.IsTaxInverseCalculation;
            posTran.IsTaxInverseCalculation = posProduct.IsTaxInverseCalculation;
            posTran.IsPrinter1 = posProduct.IsPrinter1;
            posTran.IsPrinter2 = posProduct.IsPrinter2;
            posTran.IsPrinter3 = posProduct.IsPrinter3;
            posTran.IsPrinter4 = posProduct.IsPrinter4;
            posTran.IsPrinter5 = posProduct.IsPrinter5;
            posTran.IsPrinter6 = posProduct.IsPrinter6;
            posTran.Printer1Qty = 0;
            posTran.Printer2Qty = 0;
            posTran.Printer3Qty = 0;
            posTran.Printer4Qty = 0;
            posTran.Printer5Qty = 0;
            posTran.Printer6Qty = 0;
            posTran.PromoStartDate = posProduct.PromoStartDate;
            posTran.PromoEndDate = posProduct.PromoEndDate;
            posTran.PromoDay1 = posProduct.PromoDay1;
            posTran.PromoDay2 = posProduct.PromoDay2;
            posTran.PromoDay3 = posProduct.PromoDay3;
            posTran.PromoPrice1 = posProduct.PromoPrice1;
            posTran.PromoPrice2 = posProduct.PromoPrice2;
            posTran.PromoPrice3 = posProduct.PromoPrice3;
            posTran.IsKitchenItem = posProduct.IsKitchenItem;
            posTran.IsSushiBarItem = posProduct.IsSushiBarItem;
            posTran.ManualName = "";
            posTran.DCMethod = 0;
            if (blnIsParentOpenFood)
            {
            //    pOption.price = 0;  // Gloria food add option price to parent menu item. so option should be zero price on POS for OpenFood.
            }
            posTran.Price = pOption.price;
            posTran.Quantity =  pOption.quantity;
            posTran.Amount = pOption.price * pOption.quantity;
            if (!string.IsNullOrEmpty(posProduct.TaxCode))
            {
                posTax = POS_Get_Tax(posProduct.TaxCode);
                if (posTax.Tax1 > 0)
                {
                    posTran.Tax1Rate = posTax.Tax1 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax1Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax2 > 0)
                {
                    posTran.Tax2Rate = posTax.Tax2 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax2Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax3 > 0)
                {
                    posTran.Tax3Rate = posTax.Tax3 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax3Rate = 0;
                    posTran.IsTax1 = false;
                }
            }
            else
            {
                posTax = POS_Get_Tax("TAX");
                posTran.Tax1Rate = posTax.Tax1 / 100;
                posTran.Tax2Rate = posTax.Tax2 / 100;
                posTran.Tax3Rate = posTax.Tax3 / 100;
            }
            posTran.Tax1 = 0;
            posTran.Tax2 = 0;
            posTran.Tax3 = 0;
            if (posTran.IsTax1) posTran.Tax1 = posTran.Amount * posTran.Tax1Rate;
            if (posTran.IsTax2) posTran.Tax2 = posTran.Amount * posTran.Tax2Rate;
            if (posTran.IsTax3) posTran.Tax3 = posTran.Amount * posTran.Tax3Rate;
            posTran.TableId = intTableId;
            //posTran.TableName = "ONLINE " + (intTableId - Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]) + 1);
            int iOnlineTableStart = Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]);
            if (iOnlineTableStart > intTableId)
            {
                posTran.TableName = strTableName;
            }
            else
            {
                posTran.TableName = "ONLINE " + (intTableId - iOnlineTableStart + 1);
            }
            posTran.SplitId = 1;
            posTran.OldTableId = intTableId;
            posTran.InvoiceNo = intNewInvNo;
            posTran.NumInvPrt = 0;
            posTran.IsOrdered = false;
            posTran.IsAdditionalOrder = true;
            posTran.OrderSeqNo = intNewSeqNo;
            posTran.OrderNumPeople = 0;
            dtmTemp = DateTime.Parse(pOrder.accepted_at);
            posTran.OrderDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.OrderTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            //posTran.OrderDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.OrderTime = pOrder.accepted_at.Substring(12, 8);
            posTran.OrderPasswordCode = "9999";
            posTran.OrderPasswordName = "ONLINE";
            posTran.OrderStation = "MAIN";
            posTran.IsCancelled = false;
            posTran.CancelDate = "";
            posTran.CancelTime = "";
            posTran.IsCancelPending = false;
            posTran.CancelPrintDate = "";
            posTran.CancelPrintTime = "";
            posTran.IsOK = false;
            posTran.IsCooked = false;
            posTran.IsPaidStarted = false;
            //Feature #2560
            if (blnIsPaidOnline)
            {
                //https://github.com/GlobalFood/integration_docs/blob/master/accepted_orders/README.md
                // pOrder.card_type : Only used when food clients pay by card(Excluding PayPal / iDEAL / ApplePay / G - Pay). Only used when field payment is ONLINE.Possible values (but not limited to):
                //- visa
                //- mastercard
                //- discover
                //- american_express
                //- diners_club
                //- maestro
                //- switch
                //- dankort
                //- jcb
                //- carnet
                //- elo
                //- cabal
                //- unionpay
                //- naranja
                //- alelo
                //- alia
                //posTran.PaidType = "ON";
                posTran.PaidType = pOrder.card_type.Substring(0,2); //store only two characters into PaidType
                posTran.IsPaidComplete = true;
                posTran.PaidStartDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
                posTran.PaidStartTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            }
            else
            {
                posTran.PaidType = "";
                posTran.IsPaidComplete = false;
                posTran.PaidStartDate = "";
                posTran.PaidStartTime = "";
            }
            posTran.StartReceiptNo = 0;
            posTran.CompleteReceiptNo = 0;
            posTran.CompleteDate = "";
            posTran.CompleteTime = "";
            //posTran.CreateDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.CreateTime = pOrder.accepted_at.Substring(12, 8);
            posTran.CreateDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.CreateTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.CreatePasswordCode = "9999";
            posTran.CreatePasswordName = "ONLINE";
            posTran.CreateStation = "MAIN";
            posTran.LastModDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.LastModTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            //posTran.LastModDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.LastModTime = pOrder.accepted_at.Substring(12, 8);
            posTran.LastModPasswordCode = "9999";
            posTran.LastModPasswordName = "ONLINE";
            posTran.LastModStation = "MAIN";
            posTran.IsRounding = false;
            posTran.SplitTranId = 0;
            posTran.SplitTranItemId = 0;
            posTran.SplitTranItemSplitId = 0;
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_TableTran(posTran);
        }
        private int POS_Add_DiscountTableTran(GFO_OrdersModel pOrder, int intNewInvNo, int intNewSeqNo, int intTableId, double dblTotalDiff, double dblTaxDiff, bool blnIsPaidOnline)
        {
            DateTime dtmTemp;
            //POS_Add_TableTran(pItem, posProduct, intNewInvNo, intNewSeqNo);
            posTran.ParentTranId = 0;
            posTran.TranType = "20";
            posTran.ProductId = 0;
            posTran.ProductName = "TAX Adjust";
            posTran.SecondName = "TAX Adjust";
            posTran.ProductTypeId = 0;
            posTran.InUnitPrice = 0;
            posTran.OutUnitPrice = 0;
            posTran.IsTax1 = false;
            posTran.IsTax2 = false;
            posTran.IsTax3 = false;
            posTran.IsTaxInverseCalculation = false;
            posTran.IsPrinter1 = false;
            posTran.IsPrinter2 = false;
            posTran.IsPrinter3 = false;
            posTran.IsPrinter4 = false;
            posTran.IsPrinter5 = false;
            posTran.IsPrinter6 = false;
            posTran.Printer1Qty = 0;
            posTran.Printer2Qty = 0;
            posTran.Printer3Qty = 0;
            posTran.Printer4Qty = 0;
            posTran.Printer5Qty = 0;
            posTran.Printer6Qty = 0;
            posTran.PromoStartDate = "";
            posTran.PromoEndDate = "";
            posTran.PromoDay1 = 0;
            posTran.PromoDay2 = 0;
            posTran.PromoDay3 = 0;
            posTran.PromoPrice1 = 0;
            posTran.PromoPrice2 = 0;
            posTran.PromoPrice3 = 0;
            posTran.IsKitchenItem = false;
            posTran.IsSushiBarItem = false;
            posTran.ManualName = "";
            posTran.DCMethod = (float)Math.Round((dblTotalDiff == 0? dblTaxDiff:dblTotalDiff), 2);
            posTran.Price = 0; // pItem.price;
            posTran.Quantity = 1;
            posTran.Amount = (float)dblTotalDiff; // pItem.total_item_price;
            if (!string.IsNullOrEmpty(posProduct.TaxCode))
            {
                posTax = POS_Get_Tax(posProduct.TaxCode);
                if (posTax.Tax1 > 0)
                {
                    posTran.Tax1Rate = posTax.Tax1 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax1Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax2 > 0)
                {
                    posTran.Tax2Rate = posTax.Tax2 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax2Rate = 0;
                    posTran.IsTax1 = false;
                }
                if (posTax.Tax3 > 0)
                {
                    posTran.Tax3Rate = posTax.Tax3 / 100;
                    posTran.IsTax1 = true;
                }
                else
                {
                    posTran.Tax3Rate = 0;
                    posTran.IsTax1 = false;
                }
            }
            posTran.Tax1 = (float)dblTaxDiff;
            posTran.Tax2 = 0;
            posTran.Tax3 = 0;

            posTran.TableId = intTableId;
            posTran.TableName = "ONLINE " + (intTableId - Convert.ToInt32(ConfigurationManager.AppSettings["CON_ONLINE_ORDER_START"]) + 1);
            posTran.SplitId = 1;
            posTran.OldTableId = intTableId;
            posTran.InvoiceNo = intNewInvNo;
            posTran.NumInvPrt = 0;
            posTran.IsOrdered = false;
            posTran.IsAdditionalOrder = true;
            posTran.OrderSeqNo = intNewSeqNo;
            posTran.OrderNumPeople = 0;
            dtmTemp = DateTime.Parse(pOrder.accepted_at);
            posTran.OrderDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.OrderTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.OrderPasswordCode = "9999";
            posTran.OrderPasswordName = "ONLINE";
            posTran.OrderStation = "MAIN";
            posTran.IsCancelled = false;
            posTran.CancelDate = "";
            posTran.CancelTime = "";
            posTran.IsCancelPending = false;
            posTran.CancelPrintDate = "";
            posTran.CancelPrintTime = "";
            posTran.IsOK = false;
            posTran.IsCooked = false;
            posTran.IsPaidStarted = false;
            posTran.StartReceiptNo = 0;

            //Feature #2560
            if (blnIsPaidOnline)
            {
                //Feature #3222
                //posTran.PaidType = "ON";
                posTran.PaidType = pOrder.card_type.Substring(0, 2);    //store only two characters into PaidType
                posTran.IsPaidComplete = true;
                posTran.PaidStartDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
                posTran.PaidStartTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            }
            else
            {
                posTran.PaidType = "";
                posTran.IsPaidComplete = false;
                posTran.PaidStartDate = "";
                posTran.PaidStartTime = "";
            }
            posTran.CompleteReceiptNo = 0;
            posTran.CompleteDate = "";
            posTran.CompleteTime = "";
            //posTran.CreateDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.CreateTime = pOrder.accepted_at.Substring(11, 8);
            posTran.CreateDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.CreateTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.CreatePasswordCode = "9999";
            posTran.CreatePasswordName = "ONLINE";
            posTran.CreateStation = "MAIN";
            //posTran.LastModDate = pOrder.accepted_at.Substring(0, 10);
            //posTran.LastModTime = pOrder.accepted_at.Substring(11, 8);
            posTran.LastModDate = dtmTemp.ToString("yyyy-MM-dd");    //pOrder.accepted_at.Substring(0, 10);
            posTran.LastModTime = dtmTemp.ToString("HH:mm:ss");     //pOrder.accepted_at.Substring(11, 8);
            posTran.LastModPasswordCode = "9999";
            posTran.LastModPasswordName = "ONLINE";
            posTran.LastModStation = "MAIN";
            posTran.IsRounding = false;
            posTran.SplitTranId = 0;
            posTran.SplitTranItemId = 0;
            posTran.SplitTranItemSplitId = 0;
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_TableTran(posTran);
        }
        private int POS_Get_NewSeqNo()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posSeqNo = dbPos.Get_SeqNo();
            
            int iNewSeqNo = posSeqNo.seqNo + 1;
            int iIssued = dbPos.Update_SeqNo(iNewSeqNo);
            if (iIssued == 1)
            {
                return iNewSeqNo;
            }
            return -1;
        }

        private int POS_Get_NewInvoiceNo()
        {
            DataAccessPOS1 dbPos1 = new DataAccessPOS1();
            pos1InvNos = dbPos1.Get_InvNo();
            int iNewInvNo = pos1InvNos[0].InvNo + 1;
            int iIssued = dbPos1.Issue_New_InvNo(iNewInvNo);
            if (iIssued == 1)
            {
                return iNewInvNo;
            }
            return -1;
        }
        private int POS_Add_Reservation(POS_ReservationModel posRservation)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_Reservation(posRservation);
        }
        private List<POS_ProductModel> POS_Get_Matching_Product(int? type_id)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Product_By_GFTypeID(type_id);
        }

        private int POS_Add_PhoneOrder(POS_PhoneOrderModel posPhoneOrder)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Insert_PhoneOrder(posPhoneOrder);
        }

        private int POS_Get_Empty_PhoneOrder_TableId()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            return dbPos.Get_Empty_PhoneOrder_TableId();
        }

        // Implementing Global Foods WEB API Calls
        // Fetch Menu API
        // Accept Orders API
        // curl "https://pos.globalfoodsoft.com/pos/menu" \
        //        -X GET \
        //        -H "Authorization: 8yCPCvb3dDo1k" \
        //        -H "Accept: application/json" \
        //        -H "Glf-Api-Version: 2"
        //

        private void Sync_GF_MenuItems_Process()
        {
            util.Logger("----------------------< MENU FETCHING START >------------------------");
            string strAuthKey = ConfigurationManager.AppSettings["GFFetChMenuAPIRestaurantKey"];
            util.Logger("GFFetChMenuAPIRestaurantKey = " + strAuthKey);
            if (string.IsNullOrEmpty(strAuthKey))
            {
                return;
            }
            //Create an instance of a HttpClient class and set the base address with the listener URI of the Web API.
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://pos.globalfoodsoft.com");
            //specify to use TLS 1.2 as default connection : to connect to our server must support TLS 1.2 or 1.3.
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strAuthKey);
            client.DefaultRequestHeaders.Add("Authorization", strAuthKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Glf-Api-Version", "2");
            //Use the HttpClient.GetAsync method to send a GET request to the Web API.Provide the routing pattern.
            // List all menu items.

            try
            {
                HttpResponseMessage response = client.GetAsync("pos/menu").Result;

                //util.Logger("MENU API Response = " + response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    //gfItems = response.Content.ReadAsAsync<List<GF_ItemsModel>>().Result;
                    string apiResponse = response.Content.ReadAsStringAsync().Result;
                    util.Logger("API Success Response = " + apiResponse);
                    // Entire menu data into gfMenu
                    gfMenu = JsonConvert.DeserializeObject<GF_MenusModel>(apiResponse);

                    //util.Logger("Categories = " + gfMenu.Categories.Count);

                    if (gfMenu.Categories.Count > 0)
                    {
                        // Download Menu Data into POS Database for matching
                        Check_POS_GF_MenuItems();
                    }

                }
                else
                {
                    util.Logger("MENU API Error Response = " + response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                util.Logger("MENU API Error Exception = " + e.Message);
            }
            //util.Logger("----------------------< MENU FETCHING FINISH >------------------------");
        }

        private void Check_POS_GF_MenuItems()
        {
            util.Logger("Account_Id = " + gfMenu.Account_Id);

            foreach (var pCat in gfMenu.Categories)
            {
                util.Logger("Category(Id,Name) = " + pCat.Id + "," + pCat.Name);
                util.Logger("   MenuItems = " + pCat.Items.Count);
                foreach (var pItem in pCat.Items)
                {
                    util.Logger("   MenuItems(Id,Name) = " + pItem.Id + "," + pItem.Name + "," + pItem.Price);
                    util.Logger("       Sizes = " + pItem.Sizes.Count);
                    util.Logger("       Groups = " + pItem.Groups.Count);
                    // Add or Update MenuItem ----------------------------
                    POS_Add_Parent_MenuItem(pCat, pItem);
                    // Add or Update MenuItem ----------------------------
                    if (pItem.Sizes.Count > 0)
                    {
                        foreach (var pSize in pItem.Sizes)
                        {
                            util.Logger("       Size(Id,Name) = " + pSize.Id + "," + pSize.Name + "," + pSize.Price);
                            util.Logger("           Groups = " + pSize.Groups.Count);
                            POS_Add_Child_MenuItem_From_Size(pCat, pItem, pSize);
                        }
                    }
                    if (pItem.Groups.Count > 0)
                    {
                        foreach (var pGroup in pItem.Groups)
                        {
                            util.Logger("       Group(Id,Name) = " + pGroup.Id + "," + pGroup.Name);
                            util.Logger("           Options = " + pGroup.Options.Count);
                            if (pGroup.Options.Count > 0)
                            {
                                foreach (var pOption in pGroup.Options)
                                {
                                    util.Logger("           Option(Id,Name) = " + pOption.Id + "," + pOption.Name + "," + pOption.Price);
                                    POS_Add_Child_MenuItem_From_Option(pCat, pItem, pOption);
                                }
                            }
                        }

                    }
                    

                }
                if (pCat.Groups.Count > 0)
                {
                    foreach (var pGroup in pCat.Groups)
                    {
                        util.Logger("   Cat Group(Id,Name) = " + pGroup.Id + "," + pGroup.Name);
                        util.Logger("   Cat     Options = " + pGroup.Options.Count);
                        if (pGroup.Options.Count > 0)
                        {
                            foreach (var pOption in pGroup.Options)
                            {
                                util.Logger("    Cat Option(Id,Name) = " + pOption.Id + "," + pOption.Name + "," + pOption.Price);
                                POS_Add_Child_MenuItem_From_Option(pCat, pCat.Items[0], pOption);
                            }
                        }
                    }
                }
            }
        }

        private void POS_Add_Child_MenuItem_From_Option(GF_CategoriesModel pCat,GF_ItemsModel pItem, GF_OptionsModel pOption)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posGF_MenuItems.Clear();
            posGF_MenuItems = dbPos.Get_GF_MenuItems_ById(pOption.Id);

            posGF_MenuItem.Id = pOption.Id;
            posGF_MenuItem.Menu_Category_Id = pItem.Menu_category_id;
            posGF_MenuItem.Name = pOption.Name;
            posGF_MenuItem.Description = pItem.Description;
            posGF_MenuItem.Price = pOption.Price;
            posGF_MenuItem.Active = pItem.Active;
            posGF_MenuItem.ParentId = pItem.Id;
            posGF_MenuItem.SubType = 2;
            posGF_MenuItem.SubName = pOption.Name;
            posGF_MenuItem.CategoryName = pCat.Name;
            //posGF_MenuItem.Match_DTTM = DateTime.Now; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (posGF_MenuItems.Count == 1)
            {
                // Update
                util.Logger("POS_Add_Child_MenuItem_From_Option Update_GF_MenuItems = " + posGF_MenuItem.Id + " posGF_MenuItem.Match_POS_Id = " + posGF_MenuItem.Match_POS_Id.ToString());
                dbPos.Update_GF_MenuItems(posGF_MenuItem, false);
            }
            else if (posGF_MenuItems.Count == 0)
            {
                posGF_MenuItem.Match_POS_Id = 0;
                // Insert
                util.Logger("POS_Add_Child_MenuItem_From_Option Insert_GF_MenuItems = " + posGF_MenuItem.Id + " posGF_MenuItem.Match_POS_Id = " + posGF_MenuItem.Match_POS_Id.ToString());
                dbPos.Insert_GF_MenuItems(posGF_MenuItem);
            }
        }

        private void POS_Add_Child_MenuItem_From_Size(GF_CategoriesModel pCat, GF_ItemsModel pItem, GF_SizesModel pSize)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posGF_MenuItems.Clear();
            posGF_MenuItems = dbPos.Get_GF_MenuItems_ById(pSize.Id);

            posGF_MenuItem.Id = pSize.Id;
            posGF_MenuItem.Menu_Category_Id = pItem.Menu_category_id;
            posGF_MenuItem.Name = pSize.Name;
            posGF_MenuItem.Description = pItem.Description;
            posGF_MenuItem.Price = pSize.Price;
            posGF_MenuItem.Active = pItem.Active;
            posGF_MenuItem.ParentId = pItem.Id;
            posGF_MenuItem.SubType = 1;
            posGF_MenuItem.SubName = pSize.Name;
            posGF_MenuItem.CategoryName = pCat.Name;
            posGF_MenuItem.Match_POS_Id = 0;
            //posGF_MenuItem.Match_DTTM = DateTime.Now; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (posGF_MenuItems.Count == 1)
            {
                // Update
                util.Logger("POS_Add_Child_MenuItem_From_Size Update_GF_MenuItems = " + posGF_MenuItem.Id + " posGF_MenuItem.Match_POS_Id = " + posGF_MenuItem.Match_POS_Id.ToString());
                dbPos.Update_GF_MenuItems(posGF_MenuItem, false);
            }
            else if (posGF_MenuItems.Count == 0)
            {
                // Insert
                util.Logger("POS_Add_Child_MenuItem_From_Size Insert_GF_MenuItems = " + posGF_MenuItem.Id + " posGF_MenuItem.Match_POS_Id = " + posGF_MenuItem.Match_POS_Id.ToString());
                dbPos.Insert_GF_MenuItems(posGF_MenuItem);
            }
        }

        private void POS_Add_Parent_MenuItem(GF_CategoriesModel pCat, GF_ItemsModel pItem)
        {
            bool blnMatched = false;
            DataAccessPOS dbPos = new DataAccessPOS();
            posGF_MenuItems.Clear();
            posGF_MenuItems = dbPos.Get_GF_MenuItems_ById(pItem.Id);

            posGF_MenuItem.Id = pItem.Id;
            posGF_MenuItem.Menu_Category_Id = pItem.Menu_category_id;
            posGF_MenuItem.Name = pItem.Name;
            posGF_MenuItem.Description = pItem.Description;
            posGF_MenuItem.Price = pItem.Price;
            posGF_MenuItem.Active = pItem.Active;
            posGF_MenuItem.ParentId = 0;
            posGF_MenuItem.SubType = 0;
            posGF_MenuItem.SubName = "";
            posGF_MenuItem.CategoryName = pCat.Name;
            //posGF_MenuItem.Match_POS_Id = 0;
            //posGF_MenuItem.Match_DTTM = DateTime.Now; //DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Check POS for matching Menu Item : Feature #3234
            posProducts = dbPos.Get_Product_By_Name(pItem.Name);
            blnMatched = false;
            if (posProducts.Count == 1)
            {
                if (posProducts[0].OutUnitPrice == posGF_MenuItem.Price)
                {
                    util.Logger("-------------------------------------------------------------------");
                    util.Logger("POS Menu Item Matched posProducts    = " + posProducts[0].Id + " , "+ posProducts[0].ProductName + " : " + posProducts[0].OutUnitPrice);
                    util.Logger("POS Menu Item Matched posGF_MenuItem = " + posGF_MenuItem.Name + " : " + posGF_MenuItem.Price);
                    posGF_MenuItem.Match_POS_Id = posProducts[0].Id;
                    posGF_MenuItem.Match_DTTM = DateTime.Now;
                    blnMatched = true;
                }
                else
                {
                    posGF_MenuItem.Match_POS_Id = 0;
                    blnMatched = false;
                }
            }
            else
            {
                posGF_MenuItem.Match_POS_Id = 0;
                blnMatched = false;
            }

            if (posGF_MenuItems.Count == 1)
            {
                // Update
                dbPos.Update_GF_MenuItems(posGF_MenuItem, blnMatched);
            }
            else if(posGF_MenuItems.Count == 0)
            {
                // Insert
                dbPos.Insert_GF_MenuItems(posGF_MenuItem);
            }
        }
        private bool ChecCoonectivityPOSDB()
        {
            //this.textBoxMsg.Text = "Checking POS Database";
            //this.listViewActions.Items.Add(this.textBoxMsg.Text);
            //util.Logger("1.Checking POS Database ----------------");
            try
            {
                DataAccessPOS dbPos = new DataAccessPOS();
                passWords = dbPos.UserLogin("1");
                //this.listViewActions.Items.Add("POS Database connected");
                util.Logger("==> POS Database connected");
            }
            catch (Exception ex)
            {
                util.Logger("POS DB : Failed = "+ex.Message);
                return false;
            }
            return true;

        }
        private int Add_Or_Update_Customer_on_POS(POS_CustomerModel pCustomer)
        {
            DataAccessPOS dbPOS = new DataAccessPOS();
            /* ---------------------------------------------------------------------*/
            /* check POS for customer data                                          */
            posCustomers = dbPOS.Get_Customer_by_PhoneNo(pCustomer.Phone);
            if (posCustomers.Count == 0)
            {
                posCustomers.Add(new POS_CustomerModel()
                {
                    FirstName = pCustomer.FirstName,
                    LastName = pCustomer.LastName,
                    Phone = pCustomer.Phone,
                    Address1 = pCustomer.Address1,
                    Address2 = pCustomer.Address2,
                    Zip = pCustomer.Zip,
                    DateMarried = pCustomer.DateMarried,
                    DateOfBirth = pCustomer.DateOfBirth,
                    Memo = pCustomer.Memo,
                    WebId = pCustomer.Id,
                    Email = pCustomer.Email
                    //SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                });
                return dbPOS.Insert_Customer(posCustomers[0]);
            }
            else
            {
                if (posCustomers.Count == 1)
                {
                    posCustomers[0].FirstName = pCustomer.FirstName;
                    posCustomers[0].LastName = pCustomer.LastName;
                    posCustomers[0].Phone = pCustomer.Phone;
                    //posCustomers[0].Address1 = pCustomer.Address1;
                    //posCustomers[0].Address2 = pCustomer.Address2;
                    //posCustomers[0].Zip = pCustomer.Zip;
                    //posCustomers[0].DateMarried = pCustomer.DateMarried;
                    //posCustomers[0].DateOfBirth = pCustomer.DateOfBirth;
                    //posCustomers[0].Memo = pCustomer.Memo;
                    //posCustomers[0].WebId = pCustomer.Id;
                    posCustomers[0].Email = pCustomer.Email;
                    dbPOS.Update_Customer(posCustomers[0]);
                    return posCustomers[0].Id;
                }
                else
                {
                    /* More than one cusomter exists */
                    return -1;
                }
            }
            /* ---------------------------------------------------------------------*/
        }
        private int Get_Invoice_No(int iTranID)
        {
            int iInvNo = 0;
            int iNewInvNo = 0;
            DataAccessPOS dbPOS = new DataAccessPOS();
            DataAccessPOS1 dbPOS1 = new DataAccessPOS1();
            posPhoneOrders = dbPOS.Get_OnlineOrder_by_ooTranID(iTranID);
            if (posPhoneOrders.Count > 0)
            {
                //iInvNo = posPhoneOrders[0].invoiceNo;
            }
            else
            {
                pos1InvNos = dbPOS1.Get_InvNo();
                iNewInvNo = pos1InvNos[0].InvNo + 1;
                int iIssued = dbPOS1.Issue_New_InvNo(iNewInvNo);
                if (iIssued == 1)
                {
                    iInvNo = iNewInvNo;
                }
            }
            return iInvNo;
        }
 /*       private void Check_OO_Activities()
        {
            try
            {
                DataAccessOO dbOO = new DataAccessOO();
                DataAccessPOS dbPOS = new DataAccessPOS();
                ooTrans = dbOO.Get_All_Pending_Transactions(strSiteCode);

                int iTranID;
                string strPhoneNo;
                string strLastName;
                int iCustomerId;
                int iTableId = 0;
                if (ooTrans.Count > 0)
                {
                    //////////////////////////////////////////////////////////////////
                    // 0. Process each Pending Online transaction
                    //////////////////////////////////////////////////////////////////
                    foreach (var pTran in ooTrans)
                    {
                        iTranID = pTran.Id;
                        strPhoneNo = pTran.Phone;
                        strLastName = pTran.CustomerName;
                        iCustomerId = pTran.CustomerId;
                        //util.Logger("> Start Processing OO Transaction ==> TranID :" + pTran.Id + ", CustomerId:" + pTran.CustomerId + ", Phone#:" + strPhoneNo + ", Name:" + strLastName);

                        //ooCustomers = dbOO.Get_Customer_by_PhoneName(strPhoneNo, strLastName);
                        // 2019.Jul.19
                        // Upone adding CustomerId field on OOTransaction Table
                        //////////////////////////////////////////////////////////////////
                        // 1. Getting Customer info from dbOO
                        //////////////////////////////////////////////////////////////////
                        ooCustomers = dbOO.Get_Customer_by_CustomerId(iCustomerId);
                        if (ooCustomers.Count == 1)
                        {
                            //util.Logger(" > #CustID :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName);
                            //////////////////////////////////////////////////////////////////
                            // 2. Add or Update POS Customer Information if not exists on POS db
                            //////////////////////////////////////////////////////////////////
                            // Add/Update customer on POS 
                            if (Add_Customer_on_POS(ooCustomers[0]) != 1)
                            {
                                // Failed to add customer on POS 
                            }
                            //////////////////////////////////////////////////////////////////
                            // 3. Get Ordered Items for the Online Transaction
                            //////////////////////////////////////////////////////////////////
                            ooItems = dbOO.Get_TranItems_by_TranID(strSiteCode,iTranID);
                            int iInvNo = Get_Invoice_No(iTranID);
                            posCustomers = dbPOS.Get_Customer_by_WebId(iCustomerId);

                            if (ooItems.Count > 0)
                            {
                                foreach (var pItem in ooItems)
                                {
                                    // ordered item found 
                                    //util.Logger(" > Start Process #Item ==> " + " Customerid:" + iCustomerId + ", TransactionId:" + pItem.TransactionId + ", iTemID:" + pItem.Id + 
                                    //            ",ProdID:"+pItem.ProductId+", PName:"+ pItem.ProductName);
                                    /////////////////////////////////////////////////////////////////////
                                    // 4. Check OnlineOrder is already in POS
                                    // Required data : pTran, posCustomers[0], pItem, iInvNo           
                                    /////////////////////////////////////////////////////////////////////
                                    posProducts = dbPOS.Get_Product_By_ID(pItem.ProductId);
                                    posPhoneOrders = dbPOS.Get_OnlineOrder_by_InvNo_CustID(iInvNo, posCustomers[0].Id);
                                    /////////////////////////////////////////////////////////////////////
                                    // 5. If OnlineOrder is not exists on POS, create the Order on POS
                                    /////////////////////////////////////////////////////////////////////
                                    if ((posPhoneOrders.Count == 0) && (posProducts.Count == 1) && (iInvNo > 0))
                                    {
                                        iTableId = dbPOS.Get_Empty_OnlineOrder_TableId();
                                        posPhoneOrders.Add(new POS_PhoneOrderModel()
                                        {
                                            invoiceNo = iInvNo,
                                            customerId = posCustomers[0].Id,
                                            oo_tranId = pTran.Id,
                                            oo_OrderDate = pTran.OrderDate,
                                            oo_OrderTime = pTran.OrderTime,
                                            oo_PickupDate = pTran.PickUpDate,
                                            oo_PickupTime = pTran.PickUpTime,
                                            oo_IsDelivered = pTran.IsDelivery,
                                            oo_IsPaid = pTran.IsPaid,
                                            oo_Amount = pTran.Amount,
                                            oo_Tax1 = pTran.Tax1,
                                            oo_Tax2 = pTran.Tax2,
                                            oo_Tax3 = pTran.Tax3,
                                            oo_TotalDue = pTran.TotalDue,
                                            oo_TotalPaid = pTran.TotalPaid,
                                            CreatedDttm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            IsOOUpdated = false,
                                            OOUpdatedDttm = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                            TableId = iTableId 
                                        });
                                        int iPOSOrderId = dbPOS.Insert_OnlineOrder(posPhoneOrders[0]);
                                        if (iPOSOrderId > 0)
                                        {
                                            // strLog = " > #OnlineOrder has created on POS ==> POS Tran Id = " + iPOSOrderId + 
                                            //             " InvoiceNo:" + posPhoneOrders[0].invoiceNo + 
                                            //", CustomerId:" + posPhoneOrders[0].customerId + 
                                            //", OO TranId:" + posPhoneOrders[0].oo_tranId;
                                            util.Logger(strLog);
                                        }
                                    }
                                    else if (posPhoneOrders.Count == 1)
                                    {
                                        iTableId = posPhoneOrders[0].TableId;
                                    }
                                    /////////////////////////////////////////////////////////////////////
                                    // 6. Check the ordered item exists on POS
                                    /////////////////////////////////////////////////////////////////////
                                    // Add transaction 
                                    posTrans = dbPOS.Get_TableTran_by_InvNo_ProdId(iInvNo, posProducts[0].Id);
                                    /////////////////////////////////////////////////////////////////////
                                    // 7. If Ordered Item is not exists on POS TableTran, create the TableTran
                                    /////////////////////////////////////////////////////////////////////
                                    if ((posTrans.Count == 0) && (iInvNo > 0))
                                    {
                                        posTrans.Add(new POS_TableTranModel()
                                        {
                                            //ParentTranId = "0",
                                            ParentTranId = pItem.ParentId,
                                            TranType = "20",
                                            ProductId = posProducts[0].Id,
                                            ProductName = posProducts[0].ProductName,
                                            SecondName = posProducts[0].SecondName,
                                            ProductTypeId = posProducts[0].ProductTypeId,
                                            InUnitPrice = posProducts[0].InUnitPrice,
                                            OutUnitPrice = posProducts[0].OutUnitPrice,
                                            IsTax1 = posProducts[0].IsTax1,
                                            IsTax2 = posProducts[0].IsTax2,
                                            IsTax3 = posProducts[0].IsTax3,
                                            IsTaxInverseCalculation = posProducts[0].IsTaxInverseCalculation,
                                            IsPrinter1 = posProducts[0].IsPrinter1,
                                            IsPrinter2 = posProducts[0].IsPrinter2,
                                            IsPrinter3 = posProducts[0].IsPrinter3,
                                            IsPrinter4 = posProducts[0].IsPrinter4,
                                            IsPrinter5 = posProducts[0].IsPrinter5,
                                            Printer1Qty = 0,
                                            Printer2Qty = 0,
                                            Printer3Qty = 0,
                                            Printer4Qty = 0,
                                            Printer5Qty = 0,
                                            PromoStartDate = posProducts[0].PromoStartDate,
                                            PromoEndDate = posProducts[0].PromoEndDate,
                                            PromoDay1 = posProducts[0].PromoDay1,
                                            PromoDay2 = posProducts[0].PromoDay2,
                                            PromoDay3 = posProducts[0].PromoDay3,
                                            PromoPrice1 = posProducts[0].PromoPrice1,
                                            PromoPrice2 = posProducts[0].PromoPrice2,
                                            PromoPrice3 = posProducts[0].PromoPrice3,
                                            IsKitchenItem = false,
                                            IsSushiBarItem = false,
                                            ManualName = "",
                                            DCMethod = 0,
                                            Price = posProducts[0].OutUnitPrice,
                                            Quantity = pItem.Quantity,
                                            Amount = pItem.Amount,
                                            Tax1Rate = pItem.Tax1Rate,   // update later 
                                            Tax2Rate = pItem.Tax2Rate,   // update later 
                                            Tax3Rate = pItem.Tax3Rate,   // update later 
                                            Tax1 = pItem.Tax1,   // update later 
                                            Tax2 = pItem.Tax2,   // update later 
                                            Tax3 = pItem.Tax3,   // update later 
                                            TableId = iTableId,
                                            TableName = "Online " + iTableId.ToString(),
                                            SplitId = 1,
                                            OldTableId = iTableId,
                                            InvoiceNo = iInvNo,
                                            NumInvPrt = 0,
                                            InvPrtDate = "",
                                            InvPrtTime = "",
                                            IsOrdered = false,      //default to false for Print Order 
                                            IsAdditionalOrder = true,
                                            OrderSeqNo = 1,
                                            OrderNumPeople = 0,
                                            OrderDate = pTran.OrderDate,
                                            OrderTime = pTran.OrderTime,
                                            OrderPasswordCode = "4",
                                            OrderPasswordName = "Online",
                                            OrderStation = "MAIN",
                                            IsCancelled = 0,
                                            CancelDate = "",
                                            CancelTime = "",
                                            IsCancelPending = false,
                                            CancelPrintDate = "",
                                            CancelPrintTime = "",
                                            IsOK = false,
                                            IsCooked = false,
                                            IsPicked = false,
                                            IsPaidStarted = false,
                                            PaidType = "",
                                            StartReceiptNo = 0,
                                            PaidStartDate = "",
                                            PaidStartTime = "",
                                            IsPaidComplete = pTran.IsPaid,
                                            CompleteReceiptNo = 0,
                                            CompleteDate = "",
                                            CompleteTime = "",
                                            CreateDate = DateTime.Now.ToString("yyyy-MM-dd"),
                                            CreateTime = DateTime.Now.ToString("HH:mm:ss"),
                                            CreatePasswordCode = "4",
                                            CreatePasswordName = "Online",
                                            CreateStation = "MAIN",
                                            LastModDate = pTran.OrderDate,
                                            LastModTime = pTran.OrderTime,
                                            LastModPasswordCode = "4",
                                            LastModPasswordName = "Online",
                                            LastModStation = "MAIN",
                                            IsRounding = false,
                                            SplitTranId = 0,
                                            SplitTranItemId = 0,
                                            SplitTranItemSplitId = 0
                                        });
                                        int iPOSTranId = dbPOS.Insert_TableTran(posTrans[0]);
                                        if (iPOSTranId > 0)
                                        {
                                            strLog = "   > #TableTran has created on POS ==> POS TranId = " + iPOSTranId + " InvoiceNo:" + posTrans[0].InvoiceNo + ", ProductId:" + posTrans[0].ProductId + ", TranId:" + posTrans[0].Id;
                                            util.Logger(strLog);
                                            /////////////////////////////////////////////////////////////////////
                                            // 8. If ordered item is created successfully, update dbOO
                                            /////////////////////////////////////////////////////////////////////
                                            dbOO.Transaction_Sync_Completed(strSiteCode, pTran.Id);
                                            //////////////////////////////////////////////////////////////////
                                            // 8.1 Get Child Items for the OOTranItem
                                            //////////////////////////////////////////////////////////////////
                                            ooChildItems = dbOO.Get_ChildTranItems_by_TranID(strSiteCode, pItem.Id);
                                            if (ooChildItems.Count > 0)
                                            {
                                                ProcessChildTranItems(iPOSTranId, ooChildItems, iInvNo, iTableId);
                                            }
                                        }
                                    }
                                    else if (posTrans.Count == 1)
                                    {
                                        /////////////////////////////////////////////////////////////////////
                                        // 8. If ordered item is created successfully, update dbOO
                                        /////////////////////////////////////////////////////////////////////
                                        dbOO.Transaction_Sync_Completed(strSiteCode,pTran.Id);
                                    }
                                    else
                                    {
                                        //Ordered item found more than one on POS 
                                        dbOO.Transaction_Sync_Error(strSiteCode, iTranID);
                                        strLog = " > #Item found more than oneon POS : " + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                        //util.Logger(strLog);
                                    }
                                }   // foreach
                            }
                            else
                            {
                                // Ordered item not found 
                                dbOO.Transaction_Sync_Error(strSiteCode,iTranID);
                                strLog = " > #Items Not found :" + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                //util.Logger(strLog);
                            }
                        }
                        else
                        {
                            if (ooCustomers.Count > 1)
                            {
                                // duplicated customer exists 
                                strLog = " > #CustID duplication found CustomerID :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName;
                                //util.Logger(strLog);
                            }
                            else
                            {
                                // Customer not found 
                                strLog = " > #CustID Not found :" + iTranID + ", CustomerId#:" + iCustomerId + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                //util.Logger(strLog);
                            }

                            dbOO.Transaction_Sync_Error(strSiteCode,iTranID);
                        }
                    }
                }
                //util.Logger("OO to POS Transaction Sync Records : " + ooTrans.Count.ToString());

                int iSyncCount = ooTrans.Count;

                ooTrans = dbOO.Get_All_Error_Transactions(strSiteCode);
                if (ooTrans.Count > 0)
                {
                    util.Logger("Transaction Error : OO Count = " + ooTrans.Count.ToString() + " Sync Count = " + iSyncCount.ToString());
                }
                else
                {
                    util.Logger("Transactions : OO Count = " + ooTrans.Count.ToString() + " Sync Count = " + iSyncCount.ToString());
                }
                util.Logger("------------- Online Activity Checking : OK ---------------- : " + strSiteCode);
            }
            catch (Exception ex)
            {
                util.Logger("Online DB : Failed : "+ex.Message);
                return;
            }
        }
    */
        private void ProcessChildTranItems(int iPOSParentTranId, List<OO_ItemModel> ooChildItems, int iInvNo, int iTableId)
        {
            DataAccessOO dbOO = new DataAccessOO();
            DataAccessPOS dbPOS = new DataAccessPOS();

            if (ooChildItems.Count > 0)
            {
                foreach (var pItem in ooChildItems)
                {
                    /* ordered item found */
                    //util.Logger(" > Start Process #Item ==> " + " Customerid:" + iCustomerId + ", TransactionId:" + pItem.TransactionId + ", iTemID:" + pItem.Id + 
                    //            ",ProdID:"+pItem.ProductId+", PName:"+ pItem.ProductName);
                    /////////////////////////////////////////////////////////////////////
                    // 4. Check OnlineOrder is already in POS
                    /* Required data : pTran, posCustomers[0], pItem, iInvNo           */
                    /////////////////////////////////////////////////////////////////////
                    posChildProducts = dbPOS.Get_Product_By_ID(pItem.ProductId);
                    posChildTrans.Clear();
                    posChildTrans.Add(new POS_TableTranModel()
                    {
                        //ParentTranId = "0",
                        ParentTranId = iPOSParentTranId,
                        TranType = "20",
                        ProductId = posChildProducts[0].Id,
                        ProductName = posChildProducts[0].ProductName,
                        SecondName = posChildProducts[0].SecondName,
                        ProductTypeId = posChildProducts[0].ProductTypeId,
                        InUnitPrice = posChildProducts[0].InUnitPrice,
                        OutUnitPrice = posChildProducts[0].OutUnitPrice,
                        IsTax1 = posChildProducts[0].IsTax1,
                        IsTax2 = posChildProducts[0].IsTax2,
                        IsTax3 = posChildProducts[0].IsTax3,
                        IsTaxInverseCalculation = posChildProducts[0].IsTaxInverseCalculation,
                        IsPrinter1 = posChildProducts[0].IsPrinter1,
                        IsPrinter2 = posChildProducts[0].IsPrinter2,
                        IsPrinter3 = posChildProducts[0].IsPrinter3,
                        IsPrinter4 = posChildProducts[0].IsPrinter4,
                        IsPrinter5 = posChildProducts[0].IsPrinter5,
                        Printer1Qty = 0,
                        Printer2Qty = 0,
                        Printer3Qty = 0,
                        Printer4Qty = 0,
                        Printer5Qty = 0,
                        PromoStartDate = posChildProducts[0].PromoStartDate,
                        PromoEndDate = posChildProducts[0].PromoEndDate,
                        PromoDay1 = posChildProducts[0].PromoDay1,
                        PromoDay2 = posChildProducts[0].PromoDay2,
                        PromoDay3 = posChildProducts[0].PromoDay3,
                        PromoPrice1 = posChildProducts[0].PromoPrice1,
                        PromoPrice2 = posChildProducts[0].PromoPrice2,
                        PromoPrice3 = posChildProducts[0].PromoPrice3,
                        IsKitchenItem = false,
                        IsSushiBarItem = false,
                        ManualName = "",
                        DCMethod = 0,
                        Price = posChildProducts[0].OutUnitPrice,
                        Quantity = pItem.Quantity,
                        Amount = pItem.Amount,
                        Tax1Rate = pItem.Tax1Rate,   /* update later */
                        Tax2Rate = pItem.Tax2Rate,   /* update later */
                        Tax3Rate = pItem.Tax3Rate,   /* update later */
                        Tax1 = pItem.Tax1,   /* update later */
                        Tax2 = pItem.Tax2,   /* update later */
                        Tax3 = pItem.Tax3,   /* update later */
                        TableId = iTableId,
                        TableName = "Online " + iTableId.ToString(),
                        SplitId = 1,
                        OldTableId = iTableId,
                        InvoiceNo = iInvNo,
                        NumInvPrt = 0,
                        InvPrtDate = "",
                        InvPrtTime = "",
                        IsOrdered = false,      /* default to false for Print Order */
                        IsAdditionalOrder = true,
                        OrderSeqNo = 1,
                        OrderNumPeople = 0,
                        OrderDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        OrderTime = DateTime.Now.ToString("HH:mm:ss"),
                        OrderPasswordCode = "4",
                        OrderPasswordName = "Online",
                        OrderStation = "MAIN",
                        IsCancelled = false,
                        CancelDate = "",
                        CancelTime = "",
                        IsCancelPending = false,
                        CancelPrintDate = "",
                        CancelPrintTime = "",
                        IsOK = false,
                        IsCooked = false,
                        IsPicked = false,
                        IsPaidStarted = false,
                        PaidType = "",
                        StartReceiptNo = 0,
                        PaidStartDate = "",
                        PaidStartTime = "",
                        IsPaidComplete = false,
                        CompleteReceiptNo = 0,
                        CompleteDate = "",
                        CompleteTime = "",
                        CreateDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        CreateTime = DateTime.Now.ToString("HH:mm:ss"),
                        CreatePasswordCode = "4",
                        CreatePasswordName = "Online",
                        CreateStation = "MAIN",
                        LastModDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        LastModTime = DateTime.Now.ToString("HH:mm:ss"),
                        LastModPasswordCode = "4",
                        LastModPasswordName = "Online",
                        LastModStation = "MAIN",
                        IsRounding = false,
                        SplitTranId = 0,
                        SplitTranItemId = 0,
                        SplitTranItemSplitId = 0
                    });
                    int iPOSTranId = dbPOS.Insert_TableTran(posChildTrans[0]);
                    if (iPOSTranId > 0)
                    {
                        strLog = "     > Child #TableTran has created on POS ==> POS TranId = " + iPOSTranId + " InvoiceNo:" + posTrans[0].InvoiceNo + ", ProductId:" + posTrans[0].ProductId + ", TranId:" + posTrans[0].Id;
                        util.Logger(strLog);
                    }
                }
            }

        }
    }
}
