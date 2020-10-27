namespace RPS_SerialPort
{
    partial class SerialPortWindow
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
            this.cmbPorts = new System.Windows.Forms.ComboBox();
            this.cmbBaud = new System.Windows.Forms.ComboBox();
            this.bttnConnection = new System.Windows.Forms.Button();
            this.bttnDigitial = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.chkMaster = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trckAnalog = new System.Windows.Forms.TrackBar();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.grpConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckAnalog)).BeginInit();
            this.grpData.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbPorts
            // 
            this.cmbPorts.FormattingEnabled = true;
            this.cmbPorts.Location = new System.Drawing.Point(44, 34);
            this.cmbPorts.Name = "cmbPorts";
            this.cmbPorts.Size = new System.Drawing.Size(121, 21);
            this.cmbPorts.TabIndex = 0;
            // 
            // cmbBaud
            // 
            this.cmbBaud.FormattingEnabled = true;
            this.cmbBaud.Location = new System.Drawing.Point(44, 61);
            this.cmbBaud.Name = "cmbBaud";
            this.cmbBaud.Size = new System.Drawing.Size(121, 21);
            this.cmbBaud.TabIndex = 1;
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
            // bttnDigitial
            // 
            this.bttnDigitial.BackColor = System.Drawing.Color.Gray;
            this.bttnDigitial.Location = new System.Drawing.Point(61, 75);
            this.bttnDigitial.Name = "bttnDigitial";
            this.bttnDigitial.Size = new System.Drawing.Size(75, 23);
            this.bttnDigitial.TabIndex = 3;
            this.bttnDigitial.UseVisualStyleBackColor = false;
            this.bttnDigitial.Click += new System.EventHandler(this.bttnDigitial_Click);
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
            this.grpConnection.TabIndex = 5;
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
            // trckAnalog
            // 
            this.trckAnalog.Location = new System.Drawing.Point(52, 24);
            this.trckAnalog.Maximum = 1023;
            this.trckAnalog.Name = "trckAnalog";
            this.trckAnalog.Size = new System.Drawing.Size(104, 45);
            this.trckAnalog.TabIndex = 6;
            this.trckAnalog.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trckAnalog_MouseUp);
            // 
            // grpData
            // 
            this.grpData.Controls.Add(this.label4);
            this.grpData.Controls.Add(this.label3);
            this.grpData.Controls.Add(this.bttnDigitial);
            this.grpData.Controls.Add(this.trckAnalog);
            this.grpData.Enabled = false;
            this.grpData.Location = new System.Drawing.Point(270, 12);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(216, 105);
            this.grpData.TabIndex = 7;
            this.grpData.TabStop = false;
            this.grpData.Text = "Playground";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Digital";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Analog";
            // 
            // SerialPortWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 127);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.grpConnection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "SerialPortWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serial Port Node";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialPortWindow_FormClosing);
            this.Load += new System.EventHandler(this.SerialPortWindow_Load);
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckAnalog)).EndInit();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Button bttnConnection;
        private System.Windows.Forms.Button bttnDigitial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trckAnalog;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkMaster;
    }
}

