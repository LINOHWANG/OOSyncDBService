using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OOSyncDB.Model;

namespace OOSyncDB
{
    public partial class MonitoringForm : Form
    {
        //private string strActivity[] = "";
        List<POS_PassWordModel> passWords = new List<POS_PassWordModel>();
        List<POS_OO_Prod_SyncModel> posProdSyncs = new List<POS_OO_Prod_SyncModel>();
        List<POS_ProductModel> posProducts = new List<POS_ProductModel>();
        List<POS_TaxModel> posTaxs = new List<POS_TaxModel>();
        List<POS_ProductTypeModel> posProdTypes = new List<POS_ProductTypeModel>();
        List<POS_CustomerModel> posCustomers = new List<POS_CustomerModel>();
        List<POS_OnlineOrderModel> posOOrders = new List<POS_OnlineOrderModel>();
        List<POS_TableTranModel> posTrans = new List<POS_TableTranModel>();
        List<POS1_InvNoModel> pos1InvNos = new List<POS1_InvNoModel>();
        List<OO_ProductModel> ooProducts = new List<OO_ProductModel>();
        List<OO_ProductTypeModel> ooProdTypes = new List<OO_ProductTypeModel>();
        List<OO_TaxModel> ooTaxs = new List<OO_TaxModel>();
        List<OO_TranModel> ooTrans = new List<OO_TranModel>();
        List<OO_CustomerModel> ooCustomers = new List<OO_CustomerModel>();
        List<OO_ItemModel> ooItems = new List<OO_ItemModel>();

        Utilities util = new Utilities();
        private bool tickcolor1;
        private bool tickcolor2;
        private bool isPOSOK;
        private bool isOOOK;
        private string strActivity;
        private string strLog;
        public MonitoringForm()
        {
            InitializeComponent();
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            this.buttonStart.Enabled = true;
            TblStateGrid_Initialize();
            DataGrid_Initialize();
            DataGrid_OO_Initialize();
            timerPOS.Stop();
            timerOO.Stop();
            this.buttonStart.Enabled = false;
            this.buttonStop.Enabled = true;
            isPOSOK = false;
            isOOOK = false;
            isPOSOK = ChecCoonectivityPOSDB();
            isOOOK = ChecCoonectivityOODB();
            if (isPOSOK && isOOOK)
            {
                //this.buttonStart.PerformClick();
                this.buttonStart_Click(this, null);
            }
            else
            {
                this.buttonStart.Enabled = true;
                this.buttonStop.Enabled = false;
                //this.listViewActions.Items.Add("Please fix the connectivity issues or Contact Administrator!");
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Please fix the connectivity issues or Contact Administrator!" });
            }
        }
        private void DataGrid_Initialize()
        {
            this.dataGridActivity.AutoSize = false;
            //this.dataGridActivity.AutoGenerateColumns = false;
            //this.dataGridActivity.RowHeadersVisible = false;
            //this.dataGridActivity.MultiSelect = false;
            this.dataGridActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridActivity.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridActivity.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridActivity.ColumnCount = 2;
            this.dataGridActivity.Columns[0].Name = "Date Time";
            this.dataGridActivity.Columns[0].Width = 130;
            this.dataGridActivity.Columns[1].Name = "Message";
            this.dataGridActivity.Columns[1].Width = 400;
            this.dataGridActivity.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            this.dataGridActivity.EnableHeadersVisualStyles = false;
            this.dataGridActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            this.dataGridActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.LightPink;
        }
        private void DataGrid_OO_Initialize()
        {
            this.dgOOActivity.AutoSize = false;
            //this.dataGridActivity.AutoGenerateColumns = false;
            //this.dataGridActivity.RowHeadersVisible = false;
            //this.dataGridActivity.MultiSelect = false;
            this.dgOOActivity.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgOOActivity.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dgOOActivity.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dgOOActivity.ColumnCount = 2;
            this.dgOOActivity.Columns[0].Name = "Date Time";
            this.dgOOActivity.Columns[0].Width = 130;
            this.dgOOActivity.Columns[1].Name = "Message";
            this.dgOOActivity.Columns[1].Width = 400;
            this.dgOOActivity.DefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            this.dgOOActivity.EnableHeadersVisualStyles = false;
            this.dgOOActivity.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12F, GraphicsUnit.Pixel);
            this.dgOOActivity.ColumnHeadersDefaultCellStyle.BackColor = Color.LightPink;
        }
        private void TblStateGrid_Initialize()
        {
            this.dgvTblState.AutoSize = false;
            //this.dataGridActivity.AutoGenerateColumns = false;
            //this.dataGridActivity.RowHeadersVisible = false;
            //this.dataGridActivity.MultiSelect = false;
            this.dgvTblState.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTblState.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvTblState.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dgvTblState.ColumnCount = 3;
            this.dgvTblState.Columns[0].Name = "Data Type";
            this.dgvTblState.Columns[0].Width = 190;
            this.dgvTblState.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvTblState.Columns[1].Name = "Records on POS DB";
            this.dgvTblState.Columns[1].Width = 300;
            this.dgvTblState.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvTblState.Columns[2].Name = "Records on Online Order";
            this.dgvTblState.Columns[2].Width = 300;
            this.dgvTblState.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvTblState.DefaultCellStyle.Font = new Font("Arial", 16F, GraphicsUnit.Pixel);
            this.dgvTblState.EnableHeadersVisualStyles = false;
            this.dgvTblState.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 18F, GraphicsUnit.Pixel);
            this.dgvTblState.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dgvTblState.ColumnHeadersDefaultCellStyle.BackColor = Color.LightBlue;
        }
        private bool ChecCoonectivityPOSDB()
        {
            //this.textBoxMsg.Text = "Checking POS Database";
            //this.listViewActions.Items.Add(this.textBoxMsg.Text);
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Checking POS Database" });
            try
            {
                DataAccessPOS dbPos = new DataAccessPOS();
                passWords = dbPos.UserLogin("1");
                //this.listViewActions.Items.Add("POS Database connected");
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "POS Database connected" });
                this.textBoxPOS.BackColor = Color.Green;
                this.textBoxPOS.Text = "POS DB : OK";
            }
            catch (Exception ex)
            {
                this.textBoxPOS.BackColor = Color.Red;
                this.textBoxPOS.Text = "POS DB : Failed";
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "POS DB : Failed" });
               // this.listViewActions.Items.Add(ex.Message);
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message});
                util.Logger(ex.Message);
                //this.buttonStart.Enabled = false;
                timerPOS.Interval = 500;
                timerPOS.Start();
                return false;
            }
            return true;

        }
        private bool ChecCoonectivityOODB()
        {
            //this.textBoxMsg.Text = "Checking Online Order Database";
            //this.listViewActions.Items.Add(this.textBoxMsg.Text);
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Checking Online Order Database" });
            try
            {
                DataAccessOO dbOO = new DataAccessOO();
                ooProducts = dbOO.GetProductByProdID("1");
                //this.listViewActions.Items.Add("Online Order Database connected");
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Online Order Database connected" });
                this.textBoxOODB.BackColor = Color.Green;
                this.textBoxOODB.Text = "Online Order DB : OK";
            }
            catch (Exception ex)
            {
                this.textBoxOODB.BackColor = Color.Red;
                this.textBoxOODB.Text = "Online Order DB : Failed";
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Online Order DB : Failed" });
                //this.listViewActions.Items.Add(ex.Message);
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message });
                strLog = this.textBoxOODB.Text + " " + ex.Message;
                util.Logger(strLog);
                //this.buttonStop.Enabled = false;
                timerOO.Interval = 500;
                timerOO.Start();
                return false;
            }
            return true;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),"Database Sync Started ............" });
            timerSync.Interval = 5000;
            timerSync.Start();
            this.buttonStart.Enabled = false;
            this.buttonStop.Enabled = true;
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Database Sync Stopped ............" });
            timerSync.Stop();
            timerPOS.Stop();
            timerOO.Stop();
            this.buttonStart.Enabled = true;
            this.buttonStop.Enabled = false;
        }
        private void buttonEnd_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void timerPOS_Tick(object sender, EventArgs e)
        {
            if (tickcolor1) this.textBoxPOS.BackColor = Color.Red;
            if (!tickcolor1) this.textBoxPOS.BackColor = Color.White;
            tickcolor1 = !tickcolor1;
        }

        private void timerOO_Tick(object sender, EventArgs e)
        {
            if (tickcolor2) this.textBoxOODB.BackColor = Color.Red;
            if (!tickcolor2) this.textBoxOODB.BackColor = Color.White;
            tickcolor2 = !tickcolor2;
        }

        private void timerSync_Tick(object sender, EventArgs e)
        {
            if (dataGridActivity.RowCount > 2000)
            {
                dataGridActivity.Rows.Clear();
                Maintenace_OO_Prod_Sync_Table();    // Remove OO_Prod_Sync_Table older than 30 days
            }
            if (dgvTblState.RowCount > 100)
            {
                dgvTblState.Rows.Clear();
            }
            Check_POS_Activities();
            Check_OO_Activities();


        }
        private void Check_POS_Activities()
        {
            try { 
                DataAccessPOS dbPos = new DataAccessPOS();
                posProdSyncs = dbPos.Get_OO_Prod_Sync_By_Flag(0);
                if (posProdSyncs.Count > 0)
                {
                    foreach (var pSync in posProdSyncs)
                    {
                        strActivity = pSync.Activity;
                        if (String.Compare(strActivity, "INSERT") == 0)
                        {
                            Update_POS_To_OO_Product(pSync.id, pSync.prodid);
                        }
                        else if (String.Compare(strActivity, "UPDATE") == 0)
                        {
                            Update_POS_To_OO_Product(pSync.id, pSync.prodid);
                        }
                        else if (String.Compare(strActivity, "DELETE") == 0)
                        {
                            Delete_POS_To_OO_Product(pSync.id, pSync.prodid);
                        }
                        //this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), pSync.prodid + " " + pSync.Activity });
                        //this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
                    }
                }
                this.textBoxPOS.BackColor = Color.Green;
                this.textBoxPOS.Text = "POS DB : OK";
            }
            catch (Exception ex)
            {
                this.textBoxPOS.BackColor = Color.Red;
                this.textBoxPOS.Text = "POS DB : Failed";
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "POS DB : Failed" });
                // this.listViewActions.Items.Add(ex.Message);
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message });
                strLog = this.textBoxPOS.Text + " " + ex.Message;
                util.Logger(strLog);
                return;
            }
            
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Product Sync Records : " + posProdSyncs.Count.ToString() });
            this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
            Check_Master_Tables();

        }
        private void Maintenace_OO_Prod_Sync_Table()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            int iDeletedCount = dbPos.Delete_OO_Prod_Sync();
            this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Maintenance Interface Table : " + iDeletedCount.ToString() });
            this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
        }

        private void Update_POS_To_OO_Product(int syncid,int prodid)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProducts = dbPos.Get_Product_By_ID(prodid);

            DataAccessOO dbOO = new DataAccessOO();
            ooProducts = dbOO.Get_Product_By_ID(prodid);

            if (posProducts.Count == 1)
            {
                int iUCount = 0;
                int iICount = 0;

                if (ooProducts.Count == 1)
                {
                    iUCount = dbOO.Update_Product(posProducts[0]);

                }
                else if (ooProducts.Count == 0)
                {
                    iICount = dbOO.Insert_Product(posProducts[0]);
                }
                if ((iUCount > 0) || (iICount > 0))
                {
                    dbPos.Update_Product_Sync_Table(syncid, 1);
                    this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Product Sync Completed : " + posProducts[0].Id + ", " + posProducts[0].ProductName + ", " + iUCount.ToString() + ","+iICount.ToString()});
                    this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
                }
            }
            else
            {
                dbPos.Update_Product_Sync_Table(syncid, 99);
            }
        }
        private void Delete_POS_To_OO_Product(int syncid, int prodid)
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            DataAccessOO dbOO = new DataAccessOO();
            int iDeleteCount = dbOO.Delete_Product_By_ID(prodid);

            if (iDeleteCount > 0)
            {
                dbPos.Update_Product_Sync_Table(syncid, 1);
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Product Deleted : " + posProducts[0].Id + ", " + iDeleteCount.ToString() });
                this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
                strLog = "Product Deleted : " + posProducts[0].Id + ", " + iDeleteCount.ToString();
                util.Logger(strLog);
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
            Check_Tax_Tables();
        }

        private void Check_Tax_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posTaxs = dbPos.Get_All_Tax();
            DataAccessOO dbOO = new DataAccessOO();
            ooTaxs = dbOO.Get_All_Tax();
            this.dgvTblState.Rows.Add(new String[] { "Tax", posTaxs.Count.ToString(), ooTaxs.Count.ToString() });
            this.dgvTblState.FirstDisplayedScrollingRowIndex = dgvTblState.RowCount - 1;
            if (posTaxs.Count != ooTaxs.Count)
            {
                this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.Red;
            }
            else
            {
                this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            if (posTaxs.Count > 0)
            {
                foreach (var pTax in posTaxs)
                {
                    ooTaxs = dbOO.Get_Tax(pTax);

                    if (ooTaxs.Count == 0)
                    {
                        dbOO.Insert_Tax(pTax);
                    }
                    else if (ooTaxs.Count == 1)
                    {
                        if ((pTax.Tax1 != ooTaxs[0].Tax1) | (pTax.Tax2 != ooTaxs[0].Tax2) | (pTax.Tax3 != ooTaxs[0].Tax3))
                        {
                            dbOO.Update_Tax(pTax);
                        }
                    }
                    else
                    {
                    }
                }
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Tax Sync done : " + posTaxs.Count.ToString() });
                this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
            }
        }

        private void Check_Product_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProducts = dbPos.Get_ALL_Products();

            DataAccessOO dbOO = new DataAccessOO();
            ooProducts = dbOO.Get_ALL_Products();

            this.dgvTblState.Rows.Add(new String[] { "Product", posProducts.Count.ToString(), ooProducts.Count.ToString() });
            this.dgvTblState.FirstDisplayedScrollingRowIndex = dgvTblState.RowCount - 1;

            if (posProducts.Count != ooProducts.Count)
            {
                this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.Red;
            }
            else
            {
                this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.LightGreen;
            }
         }
        private void Check_Product_Type_Tables()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProdTypes = dbPos.Get_ALL_ProductTypes();
            DataAccessOO dbOO = new DataAccessOO();
            ooProdTypes = dbOO.Get_ALL_ProductTypes();
            this.dgvTblState.Rows.Add(new String[] { "Product Type", posProdTypes.Count.ToString(), ooProdTypes.Count.ToString() });
            this.dgvTblState.FirstDisplayedScrollingRowIndex = dgvTblState.RowCount - 1;
            if (posProdTypes.Count != ooProdTypes.Count)
            {
                this.dgvTblState.Rows[dgvTblState.RowCount-2].DefaultCellStyle.BackColor = Color.Red;
            }
            else {
                this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.LightGreen;
            }
            // Sync Product Types
            //if (posProdTypes.Count != ooProdTypes.Count)
            if (posProdTypes.Count > 0)
            {
                foreach (var pType in posProdTypes)
                {
                    ooProdTypes = dbOO.Get_ProductType_By_ID(pType.id);

                    if (ooProdTypes.Count == 0)
                    {
                        dbOO.Insert_ProductType(pType);
                    }
                    else if (ooProdTypes.Count == 1)
                    {
                        if (pType.TypeName.CompareTo(ooProdTypes[0].TypeName) != 0)
                        { 
                            dbOO.Update_ProductType(pType);
                        }
                    }
                    else
                    {
                    }
                }
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Product Type Sync Done : " + posProdTypes.Count.ToString()});
                this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
            }

        }
        private void ReSync_All_Master()
        {
            DataAccessPOS dbPos = new DataAccessPOS();
            posProducts = dbPos.Get_ALL_Products();
            DataAccessOO dbOO = new DataAccessOO();

            dbOO.Delete_All_Product();

            if (posProducts.Count > 0)
            {
                foreach (var pProd in posProducts)
                {
                    ooProducts = dbOO.Get_Product_By_ID(pProd.Id);
                    if (ooProducts.Count == 0)
                    {
                        dbOO.Insert_Product(pProd);
                    }
                    else if (ooProducts.Count == 1)
                    {
                        dbOO.Update_Product(pProd);
                    }
                    else
                    {
                    }
                    this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "Product Master Resync : " + pProd.ProductName + "," + pProd.Id});
                    this.dataGridActivity.FirstDisplayedScrollingRowIndex = dataGridActivity.RowCount - 1;
                }
            }
            dbOO.Delete_All_ProductType();
            Check_Master_Tables();
        }

        private void buttonReSync_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete All Master data\non Online Ordering Database and Repopulate them ?","Resync", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                ReSync_All_Master();
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }
        }
        private int Add_Customer_on_POS(OO_CustomerModel ooCustomer)
        {
            DataAccessPOS dbPOS = new DataAccessPOS();
            /* ---------------------------------------------------------------------*/
            /* check POS for customer data                                          */
            posCustomers = dbPOS.Get_Customer_by_WebId(ooCustomer.Id);
            if (posCustomers.Count == 0)
            {
                return dbPOS.Insert_Customer(ooCustomer);
            }
            else
            {
                if (posCustomers.Count == 1)
                {
                    return dbPOS.Update_Customer(ooCustomer);
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
            try { 
                DataAccessOO dbOO = new DataAccessOO();
                DataAccessPOS dbPOS = new DataAccessPOS();
                ooTrans = dbOO.Get_All_Pending_Transactions();

                int iTranID;
                string strPhoneNo;
                string strLastName;
                int iCustomerId;
                if (ooTrans.Count > 0)
                {
                    foreach (var pTran in ooTrans)
                    {
                        iTranID = pTran.Id;
                        strPhoneNo = pTran.Phone;
                        strLastName = pTran.CustomerName;
                        iCustomerId = pTran.CustomerId;
                        this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "TranID :" + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName });
                        this.dgOOActivity.FirstDisplayedScrollingRowIndex = dgOOActivity.RowCount - 1;

                        //ooCustomers = dbOO.Get_Customer_by_PhoneName(strPhoneNo, strLastName);
                        // 2019.Jul.19
                        // Upone adding CustomerId field on OOTransaction Table
                        ooCustomers = dbOO.Get_Customer_by_CustomerId(iCustomerId);
                        if (ooCustomers.Count == 1)
                        {
                            this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " >#CustID :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName });
                            this.dgOOActivity.FirstDisplayedScrollingRowIndex = dgOOActivity.RowCount - 1;
                            /* Add/Update customer on POS */
                            if (Add_Customer_on_POS(ooCustomers[0]) != 1)
                            {
                                /* Failed to add customer on POS */
                            }
                            ooItems = dbOO.Get_OrderItem_by_TranID(iTranID);
                            int iInvNo = Get_Invoice_No(iTranID);
                            posCustomers = dbPOS.Get_Customer_by_WebId(iCustomerId);

                            if (ooItems.Count > 0)
                            {
                                foreach (var pItem in ooItems)
                                {
                                    /* ordered item found */
                                    this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " >#ItemID :" + pItem.Id + ", ProdID:"
                                        + pItem.TransactionId + ",ProdID:"+pItem.ProductId+", PName:"+ pItem.ProductName});
                                    this.dgOOActivity.FirstDisplayedScrollingRowIndex = dgOOActivity.RowCount - 1;
                                    /* ---------------------------------------------------------------- */
                                    /* Add into POS Transaction and Online Order Table                  */
                                    /* Required data : pTran, posCustomers[0], pItem, iInvNo            */
                                    /* ---------------------------------------------------------------- */
                                    posProducts = dbPOS.Get_Product_By_ID(pItem.ProductId);
                                    posOOrders = dbPOS.Get_OnlineOrder_by_InvNo_CustID(iInvNo, posCustomers[0].Id);
                                    /* Online Order created */
                                    if ((posOOrders.Count == 0) && (posProducts.Count == 1) && (iInvNo > 0) )
                                    {
                                        posOOrders.Add(new POS_OnlineOrderModel()
                                        {
                                            invoiceNo = iInvNo,
                                            customerId = posCustomers[0].Id,
                                            oo_tranId = pTran.Id,
                                            oo_OrderDate = pTran.OrderDate,
                                            oo_OrderTime = pTran.OrderTime,
                                            oo_PickupDate = "",
                                            oo_PickupTime = "",
                                            oo_IsDelivered = pTran.IsDelivery,
                                            oo_IsPaid = pTran.IsPaid,
                                            oo_Amount = pTran.Amount,
                                            oo_Tax1 = pTran.Tax1,
                                            oo_Tax2 = pTran.Tax2,
                                            oo_Tax3 = pTran.Tax3,
                                            oo_TotalDue = pTran.TotalDue,
                                            oo_TotalPaid = pTran.TotalPaid,
                                            CreatedDttm = DateTime.Today,
                                            IsOOUpdated = false,
                                            OOUpdatedDttm = DateTime.Today
                                        });
                                        int iPOSOrderCnt = dbPOS.Insert_OnlineOrder(posOOrders[0]);
                                    }
                                    /* Add transaction */
                                    posTrans = dbPOS.Get_TableTran_by_InvNo_ProdId(iInvNo, posProducts[0].Id);
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
                                            Tax1Rate = 0,   /* update later */
                                            Tax2Rate = 0,   /* update later */
                                            Tax3Rate = 0,   /* update later */
                                            Tax1 = 0,   /* update later */
                                            Tax2 = 0,   /* update later */
                                            Tax3 = 0,   /* update later */
                                            TableId = iInvNo,
                                            TableName = "TB"+ iInvNo.ToString(),
                                            SplitId = 1,
                                            OldTableId = iInvNo,
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
                                            OrderPasswordName = "Grade 4",
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
                                            CreateDate = "",
                                            CreateTime = "",
                                            CreatePasswordCode = "4",
                                            CreatePasswordName = "Grade 4",
                                            CreateStation = "MAIN",
                                            LastModDate = pTran.OrderDate,
                                            LastModTime = pTran.OrderTime,
                                            LastModPasswordCode = "",
                                            LastModPasswordName = "",
                                            LastModStation = "",
                                            IsRounding = false,
                                            SplitTranId = 0,
                                            SplitTranItemId = 0,
                                            SplitTranItemSplitId = 0
                                        });
                                        int iPOSTranCnt = dbPOS.Insert_TableTran(posTrans[0]);
                                    }
                                    if (posTrans.Count == 1)
                                    {
                                        dbOO.Transaction_Sync_Completed(pTran.Id);
                                    }
                                }   // foreach
                            }
                            else
                            {
                                /* Ordered item not found */
                                this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " >#Items Not found :" + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName });
                                dbOO.Transaction_Sync_Error(iTranID);
                                strLog = " >#Items Not found :" + iTranID + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                util.Logger(strLog);
                            }
                        }
                        else
                        {
                            if (ooCustomers.Count > 1)
                            {
                                /* duplicated customer exists */
                                this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " >#CustID duplication found :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName });
                                strLog = " >#CustID duplication found CustomerID :" + ooCustomers[0].Id + ", Phone#:" + ooCustomers[0].Phone + ", Name:" + ooCustomers[0].LastName;
                                util.Logger(strLog);
                            }
                            else
                            {
                                /* Customer not found */
                                this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), " >#CustID Not found :" + iTranID + ", CustomerId#:" + iCustomerId + ", Phone#:" + strPhoneNo + ", Name:" + strLastName });
                                strLog = " >#CustID Not found :" + iTranID + ", CustomerId#:" + iCustomerId + ", Phone#:" + strPhoneNo + ", Name:" + strLastName;
                                util.Logger(strLog);
                            }
                            int nRows = dgOOActivity.Rows.Count;
                            this.dgOOActivity.Rows[nRows-2].DefaultCellStyle.BackColor = Color.Red;
                            this.dgOOActivity.FirstDisplayedScrollingRowIndex = dgOOActivity.RowCount - 1;
                            dbOO.Transaction_Sync_Error(iTranID);
                        }
                    }
                }
                this.dgOOActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "OO to POS Transaction Sync Records : " + ooTrans.Count.ToString() });
                this.dgOOActivity.FirstDisplayedScrollingRowIndex = dgOOActivity.RowCount - 2;
                int iSyncCount = ooTrans.Count;

                ooTrans = dbOO.Get_All_Error_Transactions();
                if (ooTrans.Count > 0)
                {
                    this.dgvTblState.Rows.Add(new String[] { "Transaction Error", ooTrans.Count.ToString(), iSyncCount.ToString() });
                    this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    this.dgvTblState.Rows.Add(new String[] { "Transactions", iSyncCount.ToString(), iSyncCount.ToString() });
                    this.dgvTblState.Rows[dgvTblState.RowCount - 2].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                this.textBoxOODB.BackColor = Color.Green;
                this.textBoxOODB.Text = "Online DB : OK";
            }
            catch (Exception ex)
            {
                this.textBoxOODB.BackColor = Color.Red;
                this.textBoxOODB.Text = "Online DB : Failed";
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), "OO DB : Failed" });
                // this.listViewActions.Items.Add(ex.Message);
                this.dataGridActivity.Rows.Add(new String[] { DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), ex.Message});
                strLog = this.textBoxOODB.Text + " " + ex.Message;
                util.Logger(strLog);
                return;
            }
            this.dgvTblState.FirstDisplayedScrollingRowIndex = dgvTblState.RowCount - 2;
        }
    }
}
