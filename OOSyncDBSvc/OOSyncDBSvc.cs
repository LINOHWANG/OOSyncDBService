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
        List<POS_ChildGroupModel> posChildGroups = new List<POS_ChildGroupModel>();
        List<POS_ChildProdModel> posChildProds = new List<POS_ChildProdModel>();
        List<POS_TaxModel> posTaxs = new List<POS_TaxModel>();
        POS_TaxModel posTax = new POS_TaxModel();
        List<POS_ProductTypeModel> posProdTypes = new List<POS_ProductTypeModel>();
        List<POS_ProductDetailModel> posProdDetails = new List<POS_ProductDetailModel>();
        List<POS_CustomerModel> posCustomers = new List<POS_CustomerModel>();
        List<POS_OnlineOrderModel> posOOrders = new List<POS_OnlineOrderModel>();
        List<POS_TableTranModel> posTrans = new List<POS_TableTranModel>();
        List<POS_SysConfigModel> posSysConfigs = new List<POS_SysConfigModel>();
        List<POS1_InvNoModel> pos1InvNos = new List<POS1_InvNoModel>();
        List<OO_ProductModel> ooProducts = new List<OO_ProductModel>();
        List<OO_ChildGroupModel> ooChildGroups = new List<OO_ChildGroupModel>();
        List<OO_ChildProdModel> ooChildProds = new List<OO_ChildProdModel>();
        List<OO_ProductTypeModel> ooProdTypes = new List<OO_ProductTypeModel>();
        List<OO_ProductDetailModel> ooProdDetails = new List<OO_ProductDetailModel>();
        List<OO_TaxModel> ooTaxs = new List<OO_TaxModel>();
        List<OO_TranModel> ooTrans = new List<OO_TranModel>();
        List<OO_CustomerModel> ooCustomers = new List<OO_CustomerModel>();
        List<OO_ItemModel> ooItems = new List<OO_ItemModel>();
        List<OO_SiteModel> ooSites = new List<OO_SiteModel>();
        List<OO_SiteConfigModel> ooSiteConfigs = new List<OO_SiteConfigModel>();

        OO_SiteModel ooSite = new OO_SiteModel();
        OO_SiteConfigModel ooSiteConfig = new OO_SiteConfigModel();

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

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        public OOSyncDBSvc()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("OOSyncDBSvc"))
            {
                System.Diagnostics.EventLog.CreateEventSource("OOSyncDBSvc", "");
            }
            eventLog1.Source = "OOSyncDBSvc";
            eventLog1.Log = "";
        }
        public void onDebug()
        {
            util.Logger("++++++++++++++++++++++++ DEBUGGING OOSyncDBSvc Start ++++++++++++++++++++++++");
            Sync_Process();
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
            Timer timer = new Timer();
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
            util.Logger("OOSyncDB Service is starting... : Interval = " + iTimerInterval);
            timer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

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
            eventLog1.WriteEntry("Monitoring OOSyncDB System : " + System.IO.Directory.GetCurrentDirectory(), EventLogEntryType.Information, eventId++);
            util.Logger("OOSyncDBSvc Monitoring ....");
            Sync_Process();
        }
        //////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sync Main
        /// </summary>
        //////////////////////////////////////////////////////////////////////////
        private void Sync_Process()
        {
            isPOSOK = ChecCoonectivityPOSDB();
            isOOOK = ChecCoonectivityOODB();

            isSiteDetailExists = false;

            if ((isPOSOK) && (isOOOK))
            {
                isSiteDetailExists = Get_Site_Details();
                if (isSiteDetailExists)
                {
                    if (Check_OO_Site())
                    {
                        // Since Site details exists, proceed data sync
                        Check_Master_Tables();
                        Check_POS_Activities();
                        Check_OO_Activities();
                    }
                }
            }
        }

        private bool Check_OO_Site()
        {
            strSiteCode = Get_Site_Code("OO_SITE_CODE");
            strSiteName = Get_Site_Code("OO_SITE_NAME");
            strSiteAddress = Get_Site_Code("CON_ADDRESS1") + " " + Get_Site_Code("CON_ADDRESS2");
            strSitePhone = Get_Site_Code("CON_TELNO");
            strSiteGSTNumber = Get_Site_Code("CON_TAX1NO");
            strSiteBizHourStart = Get_Site_Code("OO_BIZ_HOUR_START");
            strSiteBizHourFinish = Get_Site_Code("OO_BIZ_HOUR_FINISH");
            strSiteBizLastCallHour = Get_Site_Code("OO_BIZ_LASTCALL_HOUR");
            strSiteBizBreakStart = Get_Site_Code("OO_BIZ_BREAK_START");
            strSiteBizBreakFinish = Get_Site_Code("OO_BIZ_BREAK_FINISH");

            ooSite.SiteCode = strSiteCode;
            ooSite.SiteName = strSiteName;
            ooSite.SiteAddress = strSiteAddress;
            ooSite.SitePhoneNumber = strSitePhone;
            ooSite.SiteGSTNumber = strSiteGSTNumber;
            //ooSite.Site_Biz_Hour_Start = strSiteBizHourStart;
            //ooSite.Site_Biz_Hour_Finish = strSiteBizHourFinish;
            //ooSite.Site_Biz_Hour_LastCall_Hour = strSiteBizLastCallHour;
            //ooSite.Site_Biz_Break_Start = strSiteBizBreakStart;
            //ooSite.Site_Biz_Break_Finish = strSiteBizBreakFinish;

            if (Check_OO_Site_Details(ooSite))
            {
                return true;
            };
            return false;
        }


        private bool Get_Site_Details()
        {
            strSiteCode = Get_Site_Code("OO_SITE_CODE");
            strSiteName = Get_Site_Code("OO_SITE_NAME");
            strSiteAddress = Get_Site_Code("CON_ADDRESS1") + " " + Get_Site_Code("CON_ADDRESS2");
            strSitePhone = Get_Site_Code("CON_TELNO");
            strSiteGSTNumber = Get_Site_Code("CON_TAX1NO");


            if (String.IsNullOrEmpty(strSiteCode))
            {
                return false;
            }
            if (String.IsNullOrEmpty(strSiteName))
            {
                return false;
            }
            if (String.IsNullOrEmpty(strSiteAddress))
            {
                return false;
            }
            if (String.IsNullOrEmpty(strSitePhone))
            {
                return false;
            }
            if (String.IsNullOrEmpty(strSiteGSTNumber))
            {
                return false;
            }
            return true;            
        }

        private bool Check_OO_Site_Details(OO_SiteModel site)
        {
            DataAccessOO dbOO = new DataAccessOO();
            ooSites = dbOO.Get_Site_By_Code(site.SiteCode);
            if (ooSites.Count == 1)
            {
                if ((ooSites[0].SiteName != site.SiteName)||
                    (ooSites[0].SiteAddress != site.SiteAddress) ||
                    (ooSites[0].SitePhoneNumber != site.SitePhoneNumber) ||
                    (ooSites[0].SiteGSTNumber != site.SiteGSTNumber) ||
                    (ooSites[0].IsSiteLive != site.IsSiteLive)
                    //(ooSites[0].Site_Biz_Hour_Start != site.Site_Biz_Hour_Start) ||
                    //(ooSites[0].Site_Biz_Hour_Finish != site.Site_Biz_Hour_Finish) ||
                    //(ooSites[0].Site_Biz_Hour_LastCall_Hour != site.Site_Biz_Hour_LastCall_Hour) ||
                    //(ooSites[0].Site_Biz_Break_Start != site.Site_Biz_Break_Start) ||
                    //(ooSites[0].Site_Biz_Break_Finish != site.Site_Biz_Break_Finish)
                    )
                {
                    dbOO.Update_Site(site);
                    util.Logger("Check_OO_Site_Details : Update Site Details");
                }
                return true;
            }
            if (ooSites.Count == 0)
            {
                site.SiteCreatedDTTM = DateTime.Now;
                dbOO.Insert_Site(site);
                util.Logger("Check_OO_Site_Details : Insert Site Details");
                // Full Sync is performed 1st time integration
                ReSync_All_Master();
                util.Logger("Check_OO_Site_Details : ReSync_All Master");
                return true;
            }
            util.Logger("Check_OO_Site_Details : Please fix duplicated Site Code = " + site.SiteCode);
            return false;
        }

        private string Get_Site_Code(string strConfigName)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posSysConfigs = dbPos.Get_SysConfig_By_Name(strConfigName);
            if (posSysConfigs.Count == 1)
            {
                //util.Logger("Get_Site_Code : " + strConfigName + " = " + posSysConfigs[0].config_value);
                return posSysConfigs[0].config_value;
            }
            return String.Empty;
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
        private bool ChecCoonectivityOODB()
        {
            //this.textBoxMsg.Text = "Checking Online Order Database";
            //this.listViewActions.Items.Add(this.textBoxMsg.Text);
            //util.Logger("2.Checking Online Order Database ----------------");
            DataAccessPOS dbPos = new DataAccessPOS();
            try
            {
                DataAccessOO dbOO = new DataAccessOO();
                ooSites = dbOO.Get_Site_By_Code(strSiteCode);
                //this.listViewActions.Items.Add("Online Order Database connected");
                util.Logger("==> Online Order Database connected");
                dbPos.Update_SysConfig_OnlineStatus("SUCCESS");
            }
            catch (Exception ex)
            {
                util.Logger("Online Order DB : Failed" + ex.Message);
                dbPos.Update_SysConfig_OnlineStatus("FAILED");
                return false;
            }
            return true;
        }
        private void Check_POS_Activities()
        {
            //try
            //{
            DataAccessPOS dbPos = new DataAccessPOS();
            posProdSyncs = dbPos.Get_OO_Prod_Sync_By_Flag(0);
            if (posProdSyncs.Count > 0)
            {
                foreach (var pSync in posProdSyncs)
                {
                    strActivity = pSync.Activity;
                    //if (String.Compare(strActivity, "INSERT") == 0)
                    //{
                    //    Update_POS_To_OO_Product(pSync.id, pSync.prodid);
                    //}
                    //else if (String.Compare(strActivity, "UPDATE") == 0)
                    //{
                    //    Update_POS_To_OO_Product(pSync.id, pSync.prodid);
                    //}
                    //else 
                    if (String.Compare(strActivity, "DELETE") == 0)
                    {
                        Delete_POS_To_OO_Product(pSync.id, pSync.prodid);
                    }
                    //this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), pSync.prodid + " " + pSync.Activity });
                    //this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
                }
            }
            
            posProducts = dbPos.Get_ALL_Products_To_Sync();
            if (posProducts.Count > 0)
            {
                foreach (var prdSync in posProducts)
                {
                    Update_POS_To_OO_Product(prdSync);
                }
            }
            //util.Logger("Check_POS_Activities : OK");
            //}
            //catch (Exception ex)
            //{
            //    util.Logger("Check_POS_Activities : Failed" + ex.Message);
            //    return;
            //}

            //util.Logger("Check_POS_Activities Product Sync Records : " + posProdSyncs.Count.ToString());
            Check_Master_Tables();

        }
        private void Update_POS_To_OO_Product(POS_ProductModel posProd)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
//            posProducts = dbPos.Get_Product_By_ID(prodid);

            DataAccessOO dbOO = new DataAccessOO();
            ooProducts = dbOO.Get_Product_By_ID(strSiteCode, posProd.Id);

//            if (posProducts.Count == 1)
//            {
                int iUCount = 0;
                int iICount = 0;

                if (ooProducts.Count == 1)
                {
                    ooProducts[0].SiteCode = strSiteCode;
                    ooProducts[0].PosProdId = posProd.Id;
                    ooProducts[0].ProductName = posProd.ProductName;
                    ooProducts[0].SecondName = posProd.SecondName;
                    ooProducts[0].ProductTypeId = posProd.ProductTypeId;
                    ooProducts[0].IsSubItem = posProd.IsSubItem;
                    ooProducts[0].UnitPrice = posProd.OutUnitPrice;
                    ooProducts[0].IsTax1 = posProd.IsTax1;
                    ooProducts[0].IsTax2 = posProd.IsTax2;
                    ooProducts[0].IsTax3 = posProd.IsTax3;
                    ooProducts[0].IsTaxInverseCalculation = posProd.IsTaxInverseCalculation;
                    ooProducts[0].PromoStartDate = posProd.PromoStartDate;
                    ooProducts[0].PromoEndDate = posProd.PromoEndDate;
                    ooProducts[0].PromoDay1 = posProd.PromoDay1;
                    ooProducts[0].PromoDay2 = posProd.PromoDay2;
                    ooProducts[0].PromoDay3 = posProd.PromoDay3;
                    ooProducts[0].PromoPrice1 = posProd.PromoPrice1;
                    ooProducts[0].PromoPrice2 = posProd.PromoPrice2;
                    ooProducts[0].PromoPrice3 = posProd.PromoPrice3;
                    ooProducts[0].IsSoldOut = posProd.IsSoldOut;
                    ooProducts[0].SpiceLevel = posProd.SpiceLevel;
                    ooProducts[0].SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                    ooProducts[0].IsSubItem = posProd.IsSubItem;

                iUCount = dbOO.Update_Product(ooProducts[0]);
                }
                else if (ooProducts.Count == 0)
                {
                    ooProducts.Add(new OO_ProductModel()
                    {
                        SiteCode = strSiteCode,
                        PosProdId = posProd.Id,
                        ProductName = posProd.ProductName,
                        SecondName = posProd.SecondName,
                        ProductTypeId = posProd.ProductTypeId,
                        IsSubItem = posProd.IsSubItem,
                        UnitPrice = posProd.OutUnitPrice,
                        IsTax1 = posProd.IsTax1,
                        IsTax2 = posProd.IsTax2,
                        IsTax3 = posProd.IsTax3,
                        IsTaxInverseCalculation = posProd.IsTaxInverseCalculation,
                        PromoStartDate = posProd.PromoStartDate,
                        PromoEndDate = posProd.PromoEndDate,
                        PromoDay1 = posProd.PromoDay1,
                        PromoDay2 = posProd.PromoDay2,
                        PromoDay3 = posProd.PromoDay3,
                        PromoPrice1 = posProd.PromoPrice1,
                        PromoPrice2 = posProd.PromoPrice2,
                        PromoPrice3 = posProd.PromoPrice3,
                        IsSoldOut = posProd.IsSoldOut,
                        SpiceLevel = posProd.SpiceLevel,
                        SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                    });
                    iICount = dbOO.Insert_Product(ooProducts[0]);
                }
                if ((iUCount > 0) || (iICount > 0))
                {
                    //dbPos.Update_Product_Sync_Table(syncId, 1);
                    dbPos.Update_Product_Synced(posProd.Id, 1);
                    util.Logger("Product Sync Completed : " + posProd.Id + ", " + posProd.ProductName + ", " + iUCount.ToString() + "," + iICount.ToString());                
                }
            //            }
            //            else
            //{
            //    dbPos.Update_Product_Sync_Table(syncid, 99);
            //}
            if (posProd.IsSubItem != true)
            {
                Update_POS_To_OO_ChildGroup(posProd.Id);
            }
        }

        private void Update_POS_To_OO_ChildGroup(int p_intProdId)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            DataAccessOO dbOO = new DataAccessOO();
            posChildGroups = dbPos.Get_All_ChildGroups_To_Sync(p_intProdId);
            if (posChildGroups.Count > 0)
            {
                foreach (var posChildGroup in posChildGroups)
                {
                    ooChildGroups = dbOO.Get_ChildGroup_By_ID(strSiteCode, posChildGroup.Id, posChildGroup.ParentProdId);
                    
                    int iUCount = 0;
                    int iICount = 0;

                    if (ooChildGroups.Count == 1)
                    {
                        ooChildGroups[0].SiteCode = strSiteCode;
                        ooChildGroups[0].Id = posChildGroup.Id;
                        ooChildGroups[0].ParentProdId = posChildGroup.ParentProdId;

                        if ((string.Compare(ooChildGroups[0].ChildGroupName, posChildGroup.ChildGroupName) != 0) ||
                                (ooChildGroups[0].IsMultiChoice != posChildGroup.IsMultiChoice) ||
                                (ooChildGroups[0].IsMandatory != posChildGroup.IsMandatory))
                        {
                            ooChildGroups[0].ChildGroupName = posChildGroup.ChildGroupName;
                            ooChildGroups[0].IsMultiChoice = posChildGroup.IsMultiChoice;
                            ooChildGroups[0].IsMandatory = posChildGroup.IsMandatory;

                            iUCount = dbOO.Update_ChildGroup(ooChildGroups[0]);
                        }
                    }
                    else if (ooChildGroups.Count == 0)
                    {
                        ooChildGroups.Add(new OO_ChildGroupModel()
                        {
                            SiteCode = strSiteCode,
                            Id = posChildGroup.Id,
                            ParentProdId = posChildGroup.ParentProdId,
                            ChildGroupName = posChildGroup.ChildGroupName,
                            IsMultiChoice = posChildGroup.IsMultiChoice,
                            IsMandatory = posChildGroup.IsMandatory
                        });
                        iICount = dbOO.Insert_ChildGroup(ooChildGroups[0]);
                    }
                    if ((iUCount > 0) || (iICount > 0))
                    {
                        util.Logger("ChildGroup Sync Completed : " + posChildGroup.Id + ", " + posChildGroup.ParentProdId + ", " + posChildGroup.ChildGroupName + 
                                                                ", " + iUCount.ToString() + "," + iICount.ToString());
                    }

                    Update_POS_To_OO_ChildProd(posChildGroup);
                }
            }
        }

        private void Update_POS_To_OO_ChildProd(POS_ChildGroupModel posChildGroup)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            DataAccessOO dbOO = new DataAccessOO();

            posChildProds = dbPos.Get_All_ChildProd_By_ID(posChildGroup.Id, posChildGroup.ParentProdId);
            if (posChildProds.Count > 0)
            {
                foreach (var posChildProd in posChildProds)
                {
                    //Get_ChildProd_By_ID(string siteCode, int p_intChildGroupId, int p_intProdID, int p_intParentProdId)
                    ooChildProds = dbOO.Get_ChildProd_By_ID(strSiteCode, posChildProd.ChildGroupId, posChildProd.ProdId, posChildProd.ParentProdId);

                    int iUCount = 0;
                    int iICount = 0;

                    if (ooChildProds.Count == 1)
                    {
                        ooChildProds[0].SiteCode = strSiteCode;
                        ooChildProds[0].ProdId = posChildProd.ProdId;
                        ooChildProds[0].ParentProdId = posChildProd.ParentProdId;
                        ooChildProds[0].ChildGroupId = posChildProd.ChildGroupId;

                        if (ooChildProds[0].Seq != posChildProd.Seq)
                        {
                            ooChildProds[0].Seq = posChildProd.Seq;
                            iUCount = dbOO.Update_ChildProd(ooChildProds[0]);
                        }
                    }
                    else if (ooChildProds.Count == 0)
                    {
                        ooChildProds.Add(new OO_ChildProdModel()
                        {
                            SiteCode = strSiteCode,
                            ProdId = posChildProd.ProdId,
                            ParentProdId = posChildProd.ParentProdId,
                            ChildGroupId = posChildProd.ChildGroupId,
                            Seq = posChildProd.Seq
                        });
                        iICount = dbOO.Insert_ChildProd(ooChildProds[0]);
                    }
                    if ((iUCount > 0) || (iICount > 0))
                    {
                        util.Logger("ChildProd Sync Completed : " + posChildProd.ProdId + ", " + posChildProd.ParentProdId + ", " + posChildProd.ChildGroupId +
                                                                ", " + iUCount.ToString() + "," + iICount.ToString());
                    }
                }
            }
        }

        private void Delete_POS_To_OO_Product(int syncid, int prodid)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            DataAccessOO dbOO = new DataAccessOO();
            int iDeleteCount = dbOO.Delete_Product_By_ID(strSiteCode,prodid);

            if (iDeleteCount > 0)
            {
                dbPos.Update_Product_Sync_Table(syncid, 1);
                util.Logger("Product Deleted : " + prodid + ", " + iDeleteCount.ToString());
            }
            else
            {
                dbPos.Update_Product_Sync_Table(syncid, 99);
            }
        }
        private void Check_Master_Tables()
        {
            Check_Product_Tables();
            Check_Product_Type_Tables();
            Check_ProductDetail_Tables();
            Check_Tax_Tables();
            Check_SiteConfig_Tables();
        }

        private void Check_SiteConfig_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posSysConfigs = dbPos.Get_All_SysConfig();
            DataAccessOO dbOO = new DataAccessOO();
            ooSiteConfigs = dbOO.Get_All_SiteConfig(strSiteCode);

            if (posSysConfigs.Count > 0)
            {
                foreach (var pSysConfig in posSysConfigs)
                {
                    ooSiteConfigs = dbOO.Get_SiteConfig_By_ConfigName(pSysConfig.config_name, strSiteCode);

                    ooSiteConfig.SiteCode = strSiteCode;
                    ooSiteConfig.ConfigName = pSysConfig.config_name;
                    ooSiteConfig.ConfigValue = pSysConfig.config_value;
                    ooSiteConfig.ConfigDesc = pSysConfig.config_desc;

                    if (ooSiteConfigs.Count == 0)
                    {
                        dbOO.Insert_SiteConfig(ooSiteConfig);
                        util.Logger("SiteConfig Inserted : " + ooSiteConfig.ConfigName);
                    }
                    else if (ooSiteConfigs.Count == 1)
                    {
                        if ((ooSiteConfig.ConfigValue != ooSiteConfigs[0].ConfigValue) ||
                            (ooSiteConfig.ConfigDesc != ooSiteConfigs[0].ConfigDesc) 
                            )
                        {
                            dbOO.Update_SiteConfig(ooSiteConfig, strSiteCode);
                            util.Logger("SiteConfig Updated : " + ooSiteConfig.ConfigName);
                        }
                    }
                }
                util.Logger("Site Config Sync is done : " + posSysConfigs.Count.ToString());
            }
        }

        private void Check_ProductDetail_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProdDetails = dbPos.Get_All_ProductDetail();
            DataAccessOO dbOO = new DataAccessOO();
            ooProdDetails = dbOO.Get_All_ProductDetail(strSiteCode);
            util.Logger("Check_ProductDetail_Tables : POS = " + posProdDetails.Count.ToString() + " OO = " + ooProdDetails.Count.ToString());

            if (posProdDetails.Count > 0)
            {
                foreach (var pDetail in posProdDetails)
                {
                    ooProdDetails = dbOO.Get_ProductDetail(strSiteCode, pDetail);

                    if (ooProdDetails.Count == 0)
                    {
                        ooProdDetails.Add(new OO_ProductDetailModel()
                        {
                            SiteCode = strSiteCode,
                            PosProdId = pDetail.ProductId,
                            ItemNo = pDetail.ItemNo,
                            Detail = pDetail.Detail
                        });

                        dbOO.Insert_ProductDetail(ooProdDetails[0]);
                    }
                    else if (ooProdDetails.Count > 0)
                    {
                        foreach (var oDetail in ooProdDetails)
                        {
                            oDetail.Detail = pDetail.Detail;
                            dbOO.Update_ProductDetail(strSiteCode, oDetail);
                        }
                    }
                    else
                    {
                    }
                }
                util.Logger("Product Detail Sync is done : " + posProdDetails.Count.ToString());
            }
        }

        private void Check_Tax_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posTaxs = dbPos.Get_All_Tax();
            DataAccessOO dbOO = new DataAccessOO();
            ooTaxs = dbOO.Get_All_Tax(strSiteCode);
            util.Logger("Check_Tax_Tables : POS = " + posTaxs.Count.ToString() + " OO = " + ooTaxs.Count.ToString());

            if (posTaxs.Count > 0)
            {
                foreach (var pTax in posTaxs)
                {
                    ooTaxs = dbOO.Get_Tax(strSiteCode, pTax);

                    if (ooTaxs.Count == 0)
                    {
                        ooTaxs.Add(new OO_TaxModel()
                        {
                            SiteCode = strSiteCode,
                            PosCode = pTax.Code,
                            Tax1 = pTax.Tax1,
                            Tax2 = pTax.Tax2,
                            Tax3 = pTax.Tax3
                        });
                        dbOO.Insert_Tax(ooTaxs[0]);
                        util.Logger("Tax Sync Insert done : " + posTaxs.Count.ToString());
                    }
                    else if (ooTaxs.Count == 1)
                    {
                        if ((pTax.Tax1 != ooTaxs[0].Tax1) | (pTax.Tax2 != ooTaxs[0].Tax2) | (pTax.Tax3 != ooTaxs[0].Tax3))
                        {
                            ooTaxs[0].SiteCode = strSiteCode;
                            ooTaxs[0].PosCode = pTax.Code;
                            ooTaxs[0].Tax1 = pTax.Tax1;
                            ooTaxs[0].Tax2 = pTax.Tax2;
                            ooTaxs[0].Tax3 = pTax.Tax3;
                            dbOO.Update_Tax(strSiteCode,ooTaxs[0]);
                            util.Logger("Tax Sync Update done : " + posTaxs.Count.ToString()); 
                        }
                    }
                    else
                    {
                    }
                }
            }
        }

        private void Check_Product_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProducts = dbPos.Get_ALL_Products();

            DataAccessOO dbOO = new DataAccessOO();
            ooProducts = dbOO.Get_ALL_Products(strSiteCode);

            util.Logger("Check_Product_Tables : POS = " + posProducts.Count.ToString() + " OO = " + ooProducts.Count.ToString());
        }
        private void Check_Product_Type_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProdTypes = dbPos.Get_ALL_ProductTypes();
            DataAccessOO dbOO = new DataAccessOO();
            ooProdTypes = dbOO.Get_ALL_ProductTypes(strSiteCode);
            util.Logger("Check_Product_Type_Tables : POS = " + posProdTypes.Count.ToString() + " OO = " + ooProdTypes.Count.ToString());
            // Sync Product Types
            //if (posProdTypes.Count != ooProdTypes.Count)
            if (posProdTypes.Count > 0)
            {
                foreach (var pType in posProdTypes)
                {
                    ooProdTypes = dbOO.Get_ProductType_By_ID(strSiteCode,pType.id);

                    if (ooProdTypes.Count == 0)
                    {
                        ooProdTypes.Add(new OO_ProductTypeModel()
                        {
                            SiteCode = strSiteCode,
                            ProductTypeId = pType.id,
                            TypeName = pType.TypeName
                        });   
                        dbOO.Insert_ProductType(ooProdTypes[0]);
                        util.Logger("Product Type Sync Insert Done : " + posProdTypes.Count.ToString());
                    }
                    else if (ooProdTypes.Count == 1)
                    {
                        if (pType.TypeName.CompareTo(ooProdTypes[0].TypeName) != 0)
                        {
                            ooProdTypes[0].TypeName = pType.TypeName;
                            dbOO.Update_ProductType(ooProdTypes[0]);
                            util.Logger("Product Type Sync Update Done : " + posProdTypes.Count.ToString());
                        }
                    }
                    else
                    {
                    }
                }
                

            }

        }
        private void ReSync_All_Master()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProducts = dbPos.Get_ALL_Products();
            DataAccessOO dbOO = new DataAccessOO();

            dbOO.Delete_All_Product(strSiteCode);

            if (posProducts.Count > 0)
            {
                foreach (var pProd in posProducts)
                {
                    ooProducts = dbOO.Get_Product_By_ID(strSiteCode,pProd.Id);
                    if (ooProducts.Count == 0)
                    {
                        ooProducts.Add(new OO_ProductModel()
                        {
                            SiteCode = strSiteCode,
                            PosProdId = pProd.Id,
                            ProductName = pProd.ProductName,
                            SecondName = pProd.SecondName,
                            ProductTypeId = pProd.ProductTypeId,
                            IsSubItem = pProd.IsSubItem,
                            UnitPrice = pProd.OutUnitPrice,
                            IsTax1 = pProd.IsTax1,
                            IsTax2 = pProd.IsTax2,
                            IsTax3 = pProd.IsTax3,
                            IsTaxInverseCalculation = pProd.IsTaxInverseCalculation,
                            PromoStartDate = pProd.PromoStartDate,
                            PromoEndDate = pProd.PromoEndDate,
                            PromoDay1 = pProd.PromoDay1,
                            PromoDay2 = pProd.PromoDay2,
                            PromoDay3 = pProd.PromoDay3,
                            PromoPrice1 = pProd.PromoPrice1,
                            PromoPrice2 = pProd.PromoPrice2,
                            PromoPrice3 = pProd.PromoPrice3,
                            IsSoldOut = pProd.IsSoldOut,
                            SpiceLevel = pProd.SpiceLevel,
                            SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                        });
                        dbOO.Insert_Product(ooProducts[0]);
                    }
                    else if (ooProducts.Count == 1)
                    {
                        ooProducts[0].SiteCode = strSiteCode;
                        ooProducts[0].PosProdId = pProd.Id;
                        ooProducts[0].ProductName = pProd.ProductName;
                        ooProducts[0].SecondName = pProd.SecondName;
                        ooProducts[0].ProductTypeId = pProd.ProductTypeId;
                        ooProducts[0].IsSubItem = pProd.IsSubItem;
                        ooProducts[0].UnitPrice = pProd.OutUnitPrice;
                        ooProducts[0].IsTax1 = pProd.IsTax1;
                        ooProducts[0].IsTax2 = pProd.IsTax2;
                        ooProducts[0].IsTax3 = pProd.IsTax3;
                        ooProducts[0].IsTaxInverseCalculation = pProd.IsTaxInverseCalculation;
                        ooProducts[0].PromoStartDate = pProd.PromoStartDate;
                        ooProducts[0].PromoEndDate = pProd.PromoEndDate;
                        ooProducts[0].PromoDay1 = pProd.PromoDay1;
                        ooProducts[0].PromoDay2 = pProd.PromoDay2;
                        ooProducts[0].PromoDay3 = pProd.PromoDay3;
                        ooProducts[0].PromoPrice1 = pProd.PromoPrice1;
                        ooProducts[0].PromoPrice2 = pProd.PromoPrice2;
                        ooProducts[0].PromoPrice3 = pProd.PromoPrice3;
                        ooProducts[0].IsSoldOut = pProd.IsSoldOut;
                        ooProducts[0].SpiceLevel = pProd.SpiceLevel;
                        ooProducts[0].SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                        dbOO.Update_Product(ooProducts[0]);
                    }
                    else
                    {
                    }
                    //util.Logger("Product Master Resync : " + pProd.ProductName + "," + pProd.Id);
                }
            }
            dbOO.Delete_All_ProductType(strSiteCode);
            Check_Master_Tables();
        }

        private int Add_Customer_on_POS(OO_CustomerModel ooCustomer)
        {
            DataAccessPOS dbPOS = new DataAccessPOS();
            /* ---------------------------------------------------------------------*/
            /* check POS for customer data                                          */
            posCustomers = dbPOS.Get_Customer_by_WebId(ooCustomer.Id);
            if (posCustomers.Count == 0)
            {
                posCustomers.Add(new POS_CustomerModel()
                {
                    FirstName = ooCustomer.FirstName,
                    LastName = ooCustomer.LastName,
                    Phone = ooCustomer.Phone,
                    Address1 = ooCustomer.Address1,
                    Address2 = ooCustomer.Address2,
                    Zip = ooCustomer.Zip,
                    DateMarried = ooCustomer.DateMarried,
                    DateOfBirth = ooCustomer.DateOfBirth,
                    Memo = ooCustomer.Memo,
                    WebId = ooCustomer.Id,
                    Email = ooCustomer.Email
                    //SyncDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                });
                return dbPOS.Insert_Customer(posCustomers[0]);
            }
            else
            {
                if (posCustomers.Count == 1)
                {
                    posCustomers[0].FirstName = ooCustomer.FirstName;
                    posCustomers[0].LastName = ooCustomer.LastName;
                    posCustomers[0].Phone = ooCustomer.Phone;
                    posCustomers[0].Address1 = ooCustomer.Address1;
                    posCustomers[0].Address2 = ooCustomer.Address2;
                    posCustomers[0].Zip = ooCustomer.Zip;
                    posCustomers[0].DateMarried = ooCustomer.DateMarried;
                    posCustomers[0].DateOfBirth = ooCustomer.DateOfBirth;
                    posCustomers[0].Memo = ooCustomer.Memo;
                    posCustomers[0].WebId = ooCustomer.Id;
                    posCustomers[0].Email = ooCustomer.Email;
                    return dbPOS.Update_Customer_by_WebId(posCustomers[0]);
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
            posOOrders = dbPOS.Get_OnlineOrder_by_ooTranID(iTranID);
            if (posOOrders.Count > 0)
            {
                iInvNo = posOOrders[0].invoiceNo;
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
        private void Check_OO_Activities()
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
                            /* Add/Update customer on POS */
                            if (Add_Customer_on_POS(ooCustomers[0]) != 1)
                            {
                                /* Failed to add customer on POS */
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
                                    /* ordered item found */
                                    //util.Logger(" > Start Process #Item ==> " + " Customerid:" + iCustomerId + ", TransactionId:" + pItem.TransactionId + ", iTemID:" + pItem.Id + 
                                    //            ",ProdID:"+pItem.ProductId+", PName:"+ pItem.ProductName);
                                    /////////////////////////////////////////////////////////////////////
                                    // 4. Check OnlineOrder is already in POS
                                    /* Required data : pTran, posCustomers[0], pItem, iInvNo           */
                                    /////////////////////////////////////////////////////////////////////
                                    posProducts = dbPOS.Get_Product_By_ID(pItem.ProductId);
                                    posOOrders = dbPOS.Get_OnlineOrder_by_InvNo_CustID(iInvNo, posCustomers[0].Id);
                                    /////////////////////////////////////////////////////////////////////
                                    // 5. If OnlineOrder is not exists on POS, create the Order on POS
                                    /////////////////////////////////////////////////////////////////////
                                    if ((posOOrders.Count == 0) && (posProducts.Count == 1) && (iInvNo > 0))
                                    {
                                        iTableId = dbPOS.Get_Empty_OnlineOrder_TableId();
                                        posOOrders.Add(new POS_OnlineOrderModel()
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
                                            CreatedDttm = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                                            IsOOUpdated = false,
                                            OOUpdatedDttm = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                                            TableId = iTableId
                                        });
                                        int iPOSOrderCnt = dbPOS.Insert_OnlineOrder(posOOrders[0]);
                                        if (iPOSOrderCnt == 1)
                                        {
                                            strLog = " > #OnlineOrder has created on POS ==> InvoiceNo:" + posOOrders[0].invoiceNo + ", CustomerId:" + posOOrders[0].customerId + ", TranId:" + posOOrders[0].oo_tranId;
                                            //util.Logger(strLog);
                                        }
                                    }
                                    else if (posOOrders.Count == 1)
                                    {
                                        iTableId = posOOrders[0].TableId;
                                    }
                                    /////////////////////////////////////////////////////////////////////
                                    // 6. Check the ordered item exists on POS
                                    /////////////////////////////////////////////////////////////////////
                                        /* Add transaction */
                                    posTrans = dbPOS.Get_TableTran_by_InvNo_ProdId(iInvNo, posProducts[0].Id);
                                    /////////////////////////////////////////////////////////////////////
                                    // 7. If Ordered Item is not exists on POS TableTran, create the TableTran
                                    /////////////////////////////////////////////////////////////////////
                                    if ((posTrans.Count == 0) && (iInvNo > 0))
                                    {
                                        posTrans.Add(new POS_TableTranModel()
                                        {
                                            ParentTranId = "0",
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
                                            CreateDate = DateTime.Now.ToString("MM/dd/yyyy"),
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
                                        int iPOSTranCnt = dbPOS.Insert_TableTran(posTrans[0]);
                                        if (iPOSTranCnt == 1)
                                        {
                                            strLog = " > #TableTran has created on POS ==> InvoiceNo:" + posTrans[0].InvoiceNo + ", ProductId:" + posTrans[0].ProductId + ", TranId:" + posTrans[0].Id;
                                            //util.Logger(strLog);
                                            /////////////////////////////////////////////////////////////////////
                                            // 8. If ordered item is created successfully, update dbOO
                                            /////////////////////////////////////////////////////////////////////
                                            dbOO.Transaction_Sync_Completed(strSiteCode, pTran.Id);
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
                                        /* Ordered item found more than one on POS */
                                        dbOO.Transaction_Sync_Error(strSiteCode, iTranID);
                                        strLog = " > #Item found more than oneon POS : " + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                        //util.Logger(strLog);
                                    }
                                }   // foreach
                            }
                            else
                            {
                                /* Ordered item not found */
                                dbOO.Transaction_Sync_Error(strSiteCode,iTranID);
                                strLog = " > #Items Not found :" + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                //util.Logger(strLog);
                            }
                        }
                        else
                        {
                            if (ooCustomers.Count > 1)
                            {
                                /* duplicated customer exists */
                                strLog = " > #CustID duplication found CustomerID :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName;
                                //util.Logger(strLog);
                            }
                            else
                            {
                                /* Customer not found */
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
    }
}
