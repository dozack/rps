namespace rps_serialport_potentiometer
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trckAnalog = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkMaster = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckAnalog)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.bttnDigitial.Location = new System.Drawing.Point(61, 75);
            this.bttnDigitial.Name = "bttnDigitial";
            this.bttnDigitial.Size = new System.Drawing.Size(75, 23);
            this.bttnDigitial.TabIndex = 3;
            this.bttnDigitial.Text = "button2";
            this.bttnDigitial.UseVisualStyleBackColor = true;
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkMaster);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cmbPorts);
            this.groupBox1.Controls.Add(this.bttnConnection);
            this.groupBox1.Controls.Add(this.cmbBaud);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 105);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Serial Port Selection";
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
            this.trckAnalog.Name = "trckAnalog";
            this.trckAnalog.Size = new System.Drawing.Size(104, 45);
            this.trckAnalog.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.bttnDigitial);
            this.groupBox2.Controls.Add(this.trckAnalog);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(270, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(216, 105);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Playground";
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
            // chkMaster
            // 
            this.chkMaster.AutoSize = true;
            this.chkMaster.Location = new System.Drawing.Point(171, 63);
            this.chkMaster.Name = "chkMaster";
            this.chkMaster.Size = new System.Drawing.Size(58, 17);
            this.chkMaster.TabIndex = 6;
            this.chkMaster.Text = "Master";
            this.chkMaster.UseVisualStyleBackColor = true;
            // 
            // SerialPortWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 127);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "SerialPortWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Serial Port Node";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialPortWindow_FormClosing);
            this.Load += new System.EventHandler(this.SerialPortWindow_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trckAnalog)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbPorts;
        private System.Windows.Forms.ComboBox cmbBaud;
        private System.Windows.Forms.Button bttnConnection;
        private System.Windows.Forms.Button bttnDigitial;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trckAnalog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkMaster;
    }
}

