namespace OOSyncDB
{
    partial class MonitoringForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonEnd = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.textBoxOODB = new System.Windows.Forms.TextBox();
            this.textBoxPOS = new System.Windows.Forms.TextBox();
            this.dataGridActivity = new System.Windows.Forms.DataGridView();
            this.timerPOS = new System.Windows.Forms.Timer(this.components);
            this.timerOO = new System.Windows.Forms.Timer(this.components);
            this.timerSync = new System.Windows.Forms.Timer(this.components);
            this.dgvTblState = new System.Windows.Forms.DataGridView();
            this.buttonReSync = new System.Windows.Forms.Button();
            this.dgOOActivity = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridActivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTblState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOOActivity)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonEnd
            // 
            this.buttonEnd.Location = new System.Drawing.Point(673, 536);
            this.buttonEnd.Margin = new System.Windows.Forms.Padding(7);
            this.buttonEnd.Name = "buttonEnd";
            this.buttonEnd.Size = new System.Drawing.Size(157, 57);
            this.buttonEnd.TabIndex = 1;
            this.buttonEnd.Text = "Exit";
            this.buttonEnd.UseVisualStyleBackColor = true;
            this.buttonEnd.Click += new System.EventHandler(this.buttonEnd_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(502, 536);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(7);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(157, 57);
            this.buttonStop.TabIndex = 2;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(331, 536);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(7);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(157, 57);
            this.buttonStart.TabIndex = 3;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // textBoxOODB
            // 
            this.textBoxOODB.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxOODB.Location = new System.Drawing.Point(426, 12);
            this.textBoxOODB.Name = "textBoxOODB";
            this.textBoxOODB.Size = new System.Drawing.Size(404, 39);
            this.textBoxOODB.TabIndex = 5;
            this.textBoxOODB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxPOS
            // 
            this.textBoxPOS.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxPOS.Location = new System.Drawing.Point(16, 12);
            this.textBoxPOS.Name = "textBoxPOS";
            this.textBoxPOS.Size = new System.Drawing.Size(404, 39);
            this.textBoxPOS.TabIndex = 6;
            this.textBoxPOS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // dataGridActivity
            // 
            this.dataGridActivity.AllowUserToDeleteRows = false;
            this.dataGridActivity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridActivity.Location = new System.Drawing.Point(16, 219);
            this.dataGridActivity.Name = "dataGridActivity";
            this.dataGridActivity.ReadOnly = true;
            this.dataGridActivity.RowHeadersVisible = false;
            this.dataGridActivity.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridActivity.ShowEditingIcon = false;
            this.dataGridActivity.Size = new System.Drawing.Size(404, 307);
            this.dataGridActivity.TabIndex = 7;
            // 
            // timerPOS
            // 
            this.timerPOS.Tick += new System.EventHandler(this.timerPOS_Tick);
            // 
            // timerOO
            // 
            this.timerOO.Tick += new System.EventHandler(this.timerOO_Tick);
            // 
            // timerSync
            // 
            this.timerSync.Tick += new System.EventHandler(this.timerSync_Tick);
            // 
            // dgvTblState
            // 
            this.dgvTblState.AllowUserToDeleteRows = false;
            this.dgvTblState.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTblState.Location = new System.Drawing.Point(16, 57);
            this.dgvTblState.Name = "dgvTblState";
            this.dgvTblState.ReadOnly = true;
            this.dgvTblState.RowHeadersVisible = false;
            this.dgvTblState.ShowEditingIcon = false;
            this.dgvTblState.Size = new System.Drawing.Size(814, 134);
            this.dgvTblState.TabIndex = 8;
            // 
            // buttonReSync
            // 
            this.buttonReSync.Location = new System.Drawing.Point(16, 536);
            this.buttonReSync.Margin = new System.Windows.Forms.Padding(7);
            this.buttonReSync.Name = "buttonReSync";
            this.buttonReSync.Size = new System.Drawing.Size(157, 57);
            this.buttonReSync.TabIndex = 9;
            this.buttonReSync.Text = "ReSync";
            this.buttonReSync.UseVisualStyleBackColor = true;
            this.buttonReSync.Click += new System.EventHandler(this.buttonReSync_Click);
            // 
            // dgOOActivity
            // 
            this.dgOOActivity.AllowUserToDeleteRows = false;
            this.dgOOActivity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgOOActivity.Location = new System.Drawing.Point(426, 219);
            this.dgOOActivity.Name = "dgOOActivity";
            this.dgOOActivity.ReadOnly = true;
            this.dgOOActivity.RowHeadersVisible = false;
            this.dgOOActivity.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgOOActivity.ShowEditingIcon = false;
            this.dgOOActivity.Size = new System.Drawing.Size(404, 307);
            this.dgOOActivity.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 194);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 22);
            this.label1.TabIndex = 11;
            this.label1.Text = "POS to OO Activities";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(422, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 22);
            this.label2.TabIndex = 12;
            this.label2.Text = "OO to POS Activities";
            // 
            // MonitoringForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(846, 609);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgOOActivity);
            this.Controls.Add(this.buttonReSync);
            this.Controls.Add(this.dgvTblState);
            this.Controls.Add(this.dataGridActivity);
            this.Controls.Add(this.textBoxPOS);
            this.Controls.Add(this.textBoxOODB);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonEnd);
            this.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(7);
            this.Name = "MonitoringForm";
            this.Text = "OO Database Sync";
            this.Load += new System.EventHandler(this.MonitoringForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridActivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTblState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgOOActivity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button buttonEnd;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.TextBox textBoxOODB;
        private System.Windows.Forms.TextBox textBoxPOS;
        private System.Windows.Forms.DataGridView dataGridActivity;
        private System.Windows.Forms.Timer timerPOS;
        private System.Windows.Forms.Timer timerOO;
        private System.Windows.Forms.Timer timerSync;
        private System.Windows.Forms.DataGridView dgvTblState;
        private System.Windows.Forms.Button buttonReSync;
        private System.Windows.Forms.DataGridView dgOOActivity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

