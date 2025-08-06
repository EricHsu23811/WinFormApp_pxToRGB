namespace WinFormApp_pxToRGB
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnImgSelect = new System.Windows.Forms.Button();
            this.FilePath1 = new System.Windows.Forms.TextBox();
            this.btnToBGR = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSWver = new System.Windows.Forms.Label();
            this.txtMac = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.btnPxsToHex = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.btn1from9 = new System.Windows.Forms.Button();
            this.FilePath2 = new System.Windows.Forms.TextBox();
            this.btnSelectText = new System.Windows.Forms.Button();
            this.btnDecToHex = new System.Windows.Forms.Button();
            this.btnPicToDec = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnImgSelect
            // 
            this.btnImgSelect.Location = new System.Drawing.Point(15, 60);
            this.btnImgSelect.Name = "btnImgSelect";
            this.btnImgSelect.Size = new System.Drawing.Size(138, 40);
            this.btnImgSelect.TabIndex = 0;
            this.btnImgSelect.Text = "SelectImage";
            this.btnImgSelect.UseVisualStyleBackColor = true;
            this.btnImgSelect.Click += new System.EventHandler(this.btnImgSelect_Click);
            // 
            // FilePath1
            // 
            this.FilePath1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FilePath1.Location = new System.Drawing.Point(159, 71);
            this.FilePath1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePath1.Name = "FilePath1";
            this.FilePath1.Size = new System.Drawing.Size(485, 25);
            this.FilePath1.TabIndex = 7;
            // 
            // btnToBGR
            // 
            this.btnToBGR.Location = new System.Drawing.Point(650, 60);
            this.btnToBGR.Name = "btnToBGR";
            this.btnToBGR.Size = new System.Drawing.Size(138, 40);
            this.btnToBGR.TabIndex = 8;
            this.btnToBGR.Text = "ToBGR image";
            this.btnToBGR.UseVisualStyleBackColor = true;
            this.btnToBGR.Click += new System.EventHandler(this.btnToBGR_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 343);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 25);
            this.statusStrip1.TabIndex = 93;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(81, 19);
            this.toolStripStatusLabel1.Text = "Status text";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSWver
            // 
            this.lblSWver.AutoSize = true;
            this.lblSWver.Location = new System.Drawing.Point(12, 317);
            this.lblSWver.Name = "lblSWver";
            this.lblSWver.Size = new System.Drawing.Size(83, 15);
            this.lblSWver.TabIndex = 94;
            this.lblSWver.Text = "SW_ver_date";
            // 
            // txtMac
            // 
            this.txtMac.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMac.Enabled = false;
            this.txtMac.Font = new System.Drawing.Font("新細明體", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.txtMac.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtMac.Location = new System.Drawing.Point(599, 37);
            this.txtMac.Margin = new System.Windows.Forms.Padding(4);
            this.txtMac.Name = "txtMac";
            this.txtMac.Size = new System.Drawing.Size(188, 21);
            this.txtMac.TabIndex = 96;
            this.txtMac.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label13.Location = new System.Drawing.Point(522, 39);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 19);
            this.label13.TabIndex = 97;
            this.label13.Text = "PC MAC :";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 27);
            this.menuStrip1.TabIndex = 98;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(47, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.Enabled = false;
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(73, 23);
            this.settingToolStripMenuItem.Text = "Setting";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(55, 23);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // btnPxsToHex
            // 
            this.btnPxsToHex.Location = new System.Drawing.Point(649, 106);
            this.btnPxsToHex.Name = "btnPxsToHex";
            this.btnPxsToHex.Size = new System.Drawing.Size(138, 40);
            this.btnPxsToHex.TabIndex = 99;
            this.btnPxsToHex.Text = "Img_to_HEX/Dec";
            this.btnPxsToHex.UseVisualStyleBackColor = true;
            this.btnPxsToHex.Click += new System.EventHandler(this.btnPxsToHex_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(15, 106);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(629, 132);
            this.richTextBox1.TabIndex = 100;
            this.richTextBox1.Text = "";
            // 
            // btn1from9
            // 
            this.btn1from9.Location = new System.Drawing.Point(650, 198);
            this.btn1from9.Name = "btn1from9";
            this.btn1from9.Size = new System.Drawing.Size(138, 40);
            this.btn1from9.TabIndex = 101;
            this.btn1from9.Text = "pick 1px from 9pxs";
            this.btn1from9.UseVisualStyleBackColor = true;
            this.btn1from9.Click += new System.EventHandler(this.btn1from9_Click);
            // 
            // FilePath2
            // 
            this.FilePath2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FilePath2.Location = new System.Drawing.Point(159, 279);
            this.FilePath2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FilePath2.Name = "FilePath2";
            this.FilePath2.Size = new System.Drawing.Size(485, 25);
            this.FilePath2.TabIndex = 103;
            // 
            // btnSelectText
            // 
            this.btnSelectText.Location = new System.Drawing.Point(15, 268);
            this.btnSelectText.Name = "btnSelectText";
            this.btnSelectText.Size = new System.Drawing.Size(138, 40);
            this.btnSelectText.TabIndex = 102;
            this.btnSelectText.Text = "SelectText";
            this.btnSelectText.UseVisualStyleBackColor = true;
            this.btnSelectText.Click += new System.EventHandler(this.btn25344_Click);
            // 
            // btnDecToHex
            // 
            this.btnDecToHex.Location = new System.Drawing.Point(649, 268);
            this.btnDecToHex.Name = "btnDecToHex";
            this.btnDecToHex.Size = new System.Drawing.Size(138, 40);
            this.btnDecToHex.TabIndex = 104;
            this.btnDecToHex.Text = "DecToHex(NoUse)";
            this.btnDecToHex.UseVisualStyleBackColor = true;
            this.btnDecToHex.Click += new System.EventHandler(this.btnDecToHex_Click);
            // 
            // btnPicToDec
            // 
            this.btnPicToDec.Location = new System.Drawing.Point(649, 152);
            this.btnPicToDec.Name = "btnPicToDec";
            this.btnPicToDec.Size = new System.Drawing.Size(138, 40);
            this.btnPicToDec.TabIndex = 105;
            this.btnPicToDec.Text = "ImgToDec[25344]";
            this.btnPicToDec.UseVisualStyleBackColor = true;
            this.btnPicToDec.Click += new System.EventHandler(this.btnPicToDec_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 368);
            this.Controls.Add(this.btnPicToDec);
            this.Controls.Add(this.btnDecToHex);
            this.Controls.Add(this.FilePath2);
            this.Controls.Add(this.btnSelectText);
            this.Controls.Add(this.btn1from9);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.btnPxsToHex);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.txtMac);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lblSWver);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnToBGR);
            this.Controls.Add(this.FilePath1);
            this.Controls.Add(this.btnImgSelect);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImgSelect;
        private System.Windows.Forms.TextBox FilePath1;
        private System.Windows.Forms.Button btnToBGR;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblSWver;
        private System.Windows.Forms.TextBox txtMac;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button btnPxsToHex;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button btn1from9;
        private System.Windows.Forms.TextBox FilePath2;
        private System.Windows.Forms.Button btnSelectText;
        private System.Windows.Forms.Button btnDecToHex;
        private System.Windows.Forms.Button btnPicToDec;
    }
}

