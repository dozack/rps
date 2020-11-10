namespace RPS_Modbus
{
    partial class ModbusWindow
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
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.chkMaster = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPorts = new System.Windows.Forms.ComboBox();
            this.bttnConnection = new System.Windows.Forms.Button();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.trck_Holding = new System.Windows.Forms.TrackBar();
            this.lblHolding = new System.Windows.Forms.Label();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.bttnCoil = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblHoldingVal = new System.Windows.Forms.Label();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trck_Holding)).BeginInit();
            this.grpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.chkMaster);
            this.grpConnection.Controls.Add(this.label2);
            this.grpConnection.Controls.Add(this.label1);
            this.grpConnection.Controls.Add(this.cmbPorts);
            this.grpConnection.Controls.Add(this.bttnConnection);
            this.grpConnection.Controls.Add(this.cmbBaud);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(252, 105);
            this.grpConnection.TabIndex = 6;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "Serial Port Selection";
            // 
            // chkMaster
            // 
            this.chkMaster.AutoSize = true;
            this.chkMaster.Location = new System.Drawing.Point(171, 63);
            this.chkMaster.Name = "chkMaster";
            this.chkMaster.Size = new System.Drawing.Size(57, 17);
            this.chkMaster.TabIndex = 6;
            this.chkMaster.Text = "Server";
            this.chkMaster.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Baud";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Port";
            // 
            // cmbPorts
            // 
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(44, 34);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(121, 21);
            this.cmbPorts.TabIndex = 0;
            // 
            // bttnConnection
            // 
            this.bttnConnection.Location = new System.Drawing.Point(171, 32);
            this.bttnConnection.Name = "bttnConnection";
            this.bttnConnection.Size = new System.Drawing.Size(75, 23);
            this.bttnConnection.TabIndex = 2;
            this.bttnConnection.Text = "Connect";
            this.bttnConnection.UseVisualStyleBackColor = true;
            this.bttnConnection.Click += new System.EventHandler(this.bttnConnection_Click);
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Location = new System.Drawing.Point(44, 61);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbBaud.TabIndex = 1;
            // 
            // trck_Holding
            // 
            this.trck_Holding.Location = new System.Drawing.Point(115, 19);
            this.trck_Holding.Maximum = 65535;
            this.trck_Holding.Name = "trck_Holding";
            this.trck_Holding.Size = new System.Drawing.Size(218, 45);
            this.trck_Holding.TabIndex = 7;
            this.trck_Holding.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trck_Holding_MouseUp);
            // 
            // lblHolding
            // 
            this.lblHolding.AutoSize = true;
            this.lblHolding.Location = new System.Drawing.Point(6, 32);
            this.lblHolding.Name = "lblHolding";
            this.lblHolding.Size = new System.Drawing.Size(103, 13);
            this.lblHolding.TabIndex = 8;
            this.lblHolding.Text = "Holding register 0x0:";
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.lblHoldingVal);
            this.grpData.Controls.Add(this.bttnCoil);
            this.grpData.Controls.Add(this.label3);
            this.grpData.Controls.Add(this.trck_Holding);
            this.grpData.Controls.Add(this.lblHolding);
            this.grpData.Enabled = false;
            this.grpData.Location = new System.Drawing.Point(270, 17);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(436, 100);
            this.grpData.TabIndex = 9;
            this.grpData.TabStop = false;
            this.grpData.Text = "Playground";
            // 
            // bttnCoil
            // 
            this.bttnCoil.BackColor = System.Drawing.Color.White;
            this.bttnCoil.Location = new System.Drawing.Point(129, 59);
            this.bttnCoil.Name = "bttnCoil";
            this.bttnCoil.Size = new System.Drawing.Size(68, 23);
            this.bttnCoil.TabIndex = 10;
            this.bttnCoil.UseVisualStyleBackColor = false;
            this.bttnCoil.Click += new System.EventHandler(this.bttnCoil_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Coil 0x0:";
            // 
            // lblHoldingVal
            // 
            this.lblHoldingVal.AutoSize = true;
            this.lblHoldingVal.Location = new System.Drawing.Point(339, 32);
            this.lblHoldingVal.Name = "lblHoldingVal";
            this.lblHoldingVal.Size = new System.Drawing.Size(0, 13);
            this.lblHoldingVal.TabIndex = 11;
            // 
            // ModbusWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 128);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.grpConnection);
            this.Name = "ModbusWindow";
            this.Text = "Modbus Master ";
            this.Load += new System.EventHandler(this.ModbusWindow_Load);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trck_Holding)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.CheckBox chkMaster;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.Button bttnConnection;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.TrackBar trck_Holding;
        private System.Windows.Forms.Label lblHolding;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Button bttnCoil;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblHoldingVal;
    }
}

