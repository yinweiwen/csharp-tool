namespace AiLaboratory
{
    partial class Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbxDownsampling = new System.Windows.Forms.ComboBox();
            this.cbShowGraphic = new System.Windows.Forms.CheckBox();
            this.cbUsePsd = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbWucha = new System.Windows.Forms.TextBox();
            this.tbSmoothPower = new System.Windows.Forms.TextBox();
            this.tbSelfPower = new System.Windows.Forms.TextBox();
            this.tbForce = new System.Windows.Forms.TextBox();
            this.tbJipin = new System.Windows.Forms.TextBox();
            this.tbMasterFreq = new System.Windows.Forms.TextBox();
            this.tbCableWeight = new System.Windows.Forms.TextBox();
            this.tbCableLen = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.btnSelectNext = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnExportTo = new System.Windows.Forms.Button();
            this.tbExportTo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Data Path:";
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Location = new System.Drawing.Point(0, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(629, 21);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBrowse.Location = new System.Drawing.Point(0, 33);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(629, 23);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "&Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbExportTo);
            this.groupBox1.Controls.Add(this.cbxDownsampling);
            this.groupBox1.Controls.Add(this.cbShowGraphic);
            this.groupBox1.Controls.Add(this.cbUsePsd);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbWucha);
            this.groupBox1.Controls.Add(this.tbSmoothPower);
            this.groupBox1.Controls.Add(this.tbSelfPower);
            this.groupBox1.Controls.Add(this.tbForce);
            this.groupBox1.Controls.Add(this.tbJipin);
            this.groupBox1.Controls.Add(this.tbMasterFreq);
            this.groupBox1.Controls.Add(this.tbCableWeight);
            this.groupBox1.Controls.Add(this.tbCableLen);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 340);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(629, 202);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Operation";
            // 
            // cbxDownsampling
            // 
            this.cbxDownsampling.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDownsampling.FormattingEnabled = true;
            this.cbxDownsampling.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32"});
            this.cbxDownsampling.Location = new System.Drawing.Point(369, 45);
            this.cbxDownsampling.Name = "cbxDownsampling";
            this.cbxDownsampling.Size = new System.Drawing.Size(111, 20);
            this.cbxDownsampling.TabIndex = 6;
            // 
            // cbShowGraphic
            // 
            this.cbShowGraphic.AutoSize = true;
            this.cbShowGraphic.Location = new System.Drawing.Point(541, 20);
            this.cbShowGraphic.Name = "cbShowGraphic";
            this.cbShowGraphic.Size = new System.Drawing.Size(96, 16);
            this.cbShowGraphic.TabIndex = 5;
            this.cbShowGraphic.Text = "Show Graphic";
            this.cbShowGraphic.UseVisualStyleBackColor = true;
            this.cbShowGraphic.CheckedChanged += new System.EventHandler(this.cbShowGraphic_CheckedChanged);
            // 
            // cbUsePsd
            // 
            this.cbUsePsd.AutoSize = true;
            this.cbUsePsd.Location = new System.Drawing.Point(318, 20);
            this.cbUsePsd.Name = "cbUsePsd";
            this.cbUsePsd.Size = new System.Drawing.Size(156, 16);
            this.cbUsePsd.TabIndex = 5;
            this.cbUsePsd.Text = "使用功率谱密度估计分析";
            this.cbUsePsd.UseVisualStyleBackColor = true;
            this.cbUsePsd.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(33, 167);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(198, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "计算(&C)";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 113);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "容许误差(Hz)";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ForeColor = System.Drawing.Color.MediumBlue;
            this.label10.Location = new System.Drawing.Point(313, 171);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 19);
            this.label10.TabIndex = 3;
            this.label10.Text = "索力(N)：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.MediumBlue;
            this.label9.Location = new System.Drawing.Point(283, 143);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 19);
            this.label9.TabIndex = 3;
            this.label9.Text = "一阶频率(Hz)：";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(310, 102);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 3;
            this.label11.Text = "平滑次数";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(310, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 3;
            this.label8.Text = "倍增系数";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "模糊基频(Hz)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(322, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "降采样";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(38, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "索重(kg/m)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "索长(m)";
            // 
            // tbWucha
            // 
            this.tbWucha.Location = new System.Drawing.Point(138, 110);
            this.tbWucha.Name = "tbWucha";
            this.tbWucha.Size = new System.Drawing.Size(100, 21);
            this.tbWucha.TabIndex = 2;
            this.tbWucha.Text = "0";
            this.tbWucha.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbWucha.TextChanged += new System.EventHandler(this.tbWucha_TextChanged);
            // 
            // tbSmoothPower
            // 
            this.tbSmoothPower.Location = new System.Drawing.Point(369, 98);
            this.tbSmoothPower.Name = "tbSmoothPower";
            this.tbSmoothPower.Size = new System.Drawing.Size(111, 21);
            this.tbSmoothPower.TabIndex = 2;
            this.tbSmoothPower.Text = "0";
            this.tbSmoothPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbSmoothPower.TextChanged += new System.EventHandler(this.tbSmoothPower_TextChanged);
            // 
            // tbSelfPower
            // 
            this.tbSelfPower.Location = new System.Drawing.Point(369, 71);
            this.tbSelfPower.Name = "tbSelfPower";
            this.tbSelfPower.Size = new System.Drawing.Size(111, 21);
            this.tbSelfPower.TabIndex = 2;
            this.tbSelfPower.Text = "5";
            this.tbSelfPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbSelfPower.TextChanged += new System.EventHandler(this.tbSelfPower_TextChanged);
            // 
            // tbForce
            // 
            this.tbForce.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbForce.Location = new System.Drawing.Point(395, 167);
            this.tbForce.Name = "tbForce";
            this.tbForce.ReadOnly = true;
            this.tbForce.Size = new System.Drawing.Size(156, 26);
            this.tbForce.TabIndex = 2;
            this.tbForce.Text = "0";
            this.tbForce.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbJipin
            // 
            this.tbJipin.Location = new System.Drawing.Point(138, 83);
            this.tbJipin.Name = "tbJipin";
            this.tbJipin.Size = new System.Drawing.Size(100, 21);
            this.tbJipin.TabIndex = 2;
            this.tbJipin.Text = "0";
            this.tbJipin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbJipin.TextChanged += new System.EventHandler(this.tbJipin_TextChanged);
            // 
            // tbMasterFreq
            // 
            this.tbMasterFreq.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbMasterFreq.Location = new System.Drawing.Point(395, 139);
            this.tbMasterFreq.Name = "tbMasterFreq";
            this.tbMasterFreq.Size = new System.Drawing.Size(156, 26);
            this.tbMasterFreq.TabIndex = 2;
            this.tbMasterFreq.Text = "0";
            this.tbMasterFreq.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbMasterFreq.TextChanged += new System.EventHandler(this.tbMasterFreq_TextChanged);
            // 
            // tbCableWeight
            // 
            this.tbCableWeight.Location = new System.Drawing.Point(138, 56);
            this.tbCableWeight.Name = "tbCableWeight";
            this.tbCableWeight.Size = new System.Drawing.Size(100, 21);
            this.tbCableWeight.TabIndex = 2;
            this.tbCableWeight.Text = "0";
            this.tbCableWeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbCableWeight.TextChanged += new System.EventHandler(this.tbCableWeight_TextChanged);
            // 
            // tbCableLen
            // 
            this.tbCableLen.Location = new System.Drawing.Point(138, 29);
            this.tbCableLen.Name = "tbCableLen";
            this.tbCableLen.Size = new System.Drawing.Size(100, 21);
            this.tbCableLen.TabIndex = 2;
            this.tbCableLen.Text = "0";
            this.tbCableLen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbCableLen.TextChanged += new System.EventHandler(this.tbCableLen_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 56);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnExportTo);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectNext);
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(629, 284);
            this.splitContainer1.SplitterDistance = 370;
            this.splitContainer1.TabIndex = 5;
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(0, 0);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(370, 284);
            this.listBox2.TabIndex = 0;
            // 
            // btnSelectNext
            // 
            this.btnSelectNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectNext.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSelectNext.Location = new System.Drawing.Point(174, 261);
            this.btnSelectNext.Name = "btnSelectNext";
            this.btnSelectNext.Size = new System.Drawing.Size(76, 19);
            this.btnSelectNext.TabIndex = 7;
            this.btnSelectNext.Text = "Next(&Z)";
            this.btnSelectNext.UseVisualStyleBackColor = true;
            this.btnSelectNext.Click += new System.EventHandler(this.btnSelectNext_Click);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(255, 284);
            this.listBox1.TabIndex = 0;
            // 
            // btnExportTo
            // 
            this.btnExportTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportTo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExportTo.Location = new System.Drawing.Point(7, 261);
            this.btnExportTo.Name = "btnExportTo";
            this.btnExportTo.Size = new System.Drawing.Size(76, 19);
            this.btnExportTo.TabIndex = 7;
            this.btnExportTo.Text = "&Export";
            this.btnExportTo.UseVisualStyleBackColor = true;
            this.btnExportTo.Click += new System.EventHandler(this.btnExportTo_Click);
            // 
            // tbExportTo
            // 
            this.tbExportTo.Location = new System.Drawing.Point(266, 0);
            this.tbExportTo.Name = "tbExportTo";
            this.tbExportTo.Size = new System.Drawing.Size(363, 21);
            this.tbExportTo.TabIndex = 2;
            this.tbExportTo.Text = "D:\\,4000,1";
            this.tbExportTo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbExportTo.TextChanged += new System.EventHandler(this.tbExportTo_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(204, 3);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 3;
            this.label12.Text = "export to:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 542);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Main";
            this.Text = "AiCableForce";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbWucha;
        private System.Windows.Forms.TextBox tbJipin;
        private System.Windows.Forms.TextBox tbCableWeight;
        private System.Windows.Forms.TextBox tbCableLen;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox cbUsePsd;
        private System.Windows.Forms.ComboBox cbxDownsampling;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSelectNext;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbSelfPower;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox tbForce;
        private System.Windows.Forms.TextBox tbMasterFreq;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbSmoothPower;
        private System.Windows.Forms.CheckBox cbShowGraphic;
        private System.Windows.Forms.Button btnExportTo;
        private System.Windows.Forms.TextBox tbExportTo;
        private System.Windows.Forms.Label label12;
    }
}