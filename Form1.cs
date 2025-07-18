﻿/*****************************************************************************
 **
 **  (c) All Rights Reserved.
 **  
 **
 **
 ** 打開輸出圖檔所在資料夾:2025-06-18
 ** MacAdress格式更新，2025-06-19
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;   //
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormApp_pxToRGB
{
    /* ---------------------------------------------------------
     * WINAPI STUFF
     * ------------------------------------------------------ */
    public enum pxColor : Byte 
    {
        Red = 0,
        Green = 1,
        Blue = 2,
    }
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /*******************************************************************************
         * Constant 
         ******************************************************************************/
        /* These are constant variables */
        public static readonly string userDocFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        /*******************************************************************************
         * Gobal Variable 
         ******************************************************************************/
        static string userAppFolder = userDocFolder + "\\pxToRGB";
        // 可選：獲取完整路徑
        string executablePath = Assembly.GetExecutingAssembly().Location;
        // 獲取目前執行檔的名稱
        static string fileName = Assembly.GetExecutingAssembly().GetName().Name;
        //獲取執行檔版本
        string fileVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        string buildDateTime = System.IO.File.GetCreationTime(Assembly.GetExecutingAssembly().Location).ToString("yyyy-MM-dd HH:mm");    //ToString("yyyy-MM-dd HH:mm:ss")

        string strFilePath = "";    //userAppFolder + "\\" + "H4_IQC_LBS_BURN.csv";
        static string strDailyLogDate = DateTime.Now.ToString("yyyy-MM-dd");
        static string strFileDailyFolder = userAppFolder + "\\" + DateTime.Now.ToString("yyyy")
            + "\\" + DateTime.Now.ToString("MM") + "\\" + DateTime.Now.ToString("dd");
        static string strLogNameDaily = ""; //strFileDailyFolder + "\\" + "H4_IQC_LBS_BURN" + "_" + strDailyLogDate + ".csv";
        string strFileDailyPath = "";   //userAppFolder + "\\" + strLogNameDaily;    //@"\Burn1to10.csv";
        string strSwVer = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion.ToString();

        private void Form1_Load(object sender, EventArgs e)
        {
            txtFilePath.ReadOnly = true;
            txtFilePath.BackColor = SystemColors.Info;
            toolStripStatusLabel1.Text = "Select a image file to convert.";
            btnToBGR.Enabled = false;

            txtMac.Text = GetMacAddress().ToString();
            lblSWver.Text = "SW : " + fileName + " - ver: " + strSwVer + " ; 程式碼日期 : " + buildDateTime;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            utilHelpAbout();
        }
        private void utilHelpAbout()  
        {
            string company = "Megaforce(R) " + fileName + " Ver:" + fileVersion + "\n";
            string copyright = "(C)Copyright Megaforce Corp 2023-2033.\n";
            string buildDT = "Built on " + buildDateTime + "\n";
            string Author = "Author: Eric Hsu\n";
            MessageBox.Show(company + copyright + buildDT + Author, "About " + fileName);
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)  
        {
            this.Close();
        }
        public static string GetMacAddress()
        {
            string macAddresses = "";

            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)  //2023-03-21
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
                else if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    macAddresses += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            macAddresses = AddSpaceEveryNChar(macAddresses, 2); //2025-06-19
            return macAddresses;
        }
        private void utilCheckUserFile()
        {
            if (Directory.Exists(userAppFolder) == false)
            { Directory.CreateDirectory(userAppFolder); }
            //if (Directory.Exists(strFileDailyFolder) == false)
            //{ Directory.CreateDirectory(strFileDailyFolder); }
        }
        public bool IsFileLocked(string filename) 
        {
            bool Locked = false;
            try
            {
                FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                Locked = true;
            }
            return Locked;
        }

        private void btnImgSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Select A File";
            openDialog.Filter = "Image Files (*.png;*.jpg;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif" + "|" + "All Files (*.*)|*.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openDialog.FileName;
                txtFilePath.Text = file;
                toolStripStatusLabel1.Text = "InputFile= " + txtFilePath.Text;
            }
            if (txtFilePath.Text != "") { btnToBGR.Enabled = true; }
            else { btnToBGR.Enabled = false; }

            toolStripStatusLabel1.Text = "按下 ToBGRimage 按鈕以產出圖檔";
        }

        private void btnToBGR_Click(object sender, EventArgs e)
        {
            // 設定輸入和輸出檔案路徑
            string inputImagePath = txtFilePath.Text;   //"D:\\temporary\\imgs\\ITRI_img1-72x176.png"; //"input.png"; // 替換為你的輸入圖片路徑
            string outputImagePath = userAppFolder + "\\output.png";    //"output.png"; // 替換為你的輸出圖片路徑
            utilCheckUserFile(); Boolean bRGBmix = false; //非純RGB是否要混色

            try
            {
                // 讀取圖片
                Bitmap inputImage = new Bitmap(inputImagePath);

                // 建立輸出圖片:原圖每px展開成RGB的3pxs
                Bitmap outputImage = new Bitmap(inputImage.Width * 3, inputImage.Height);

                // 遍歷每個像素
                for (int y = 0; y < inputImage.Height; y++)
                {
                    for (int x = 0; x < inputImage.Width; x++)
                    {
                        // 獲取像素的 RGB 值
                        Color pixelColor = inputImage.GetPixel(x, y);
                        byte r = pixelColor.R;
                        byte g = pixelColor.G;
                        byte b = pixelColor.B;
                        Color newColor = Color.FromArgb(0, 0, 0);   //default
                        byte Cut = 32;  //no show out if under the value

                        for (int i = 0; i < 3; i++) //原圖每px展開成RGB分開的3pxs
                        {
                            int j = x * 3 + i;
                            switch (i)
                            {
                                case 0: //b
                                    // 建立新的顏色
                                    if (bRGBmix && b > Cut && (r != 0 || g != 0))
                                    { newColor = Color.FromArgb(r, g, b); }
                                    else
                                    { newColor = Color.FromArgb(0, 0, b); }
                                    break;
                                case 1: //g
                                    // 建立新的顏色
                                    if (bRGBmix && g > Cut && (r != 0 || b != 0))
                                    { newColor = Color.FromArgb(r, g, b); }
                                    else
                                    { newColor = Color.FromArgb(0, g, 0); }
                                    break;
                                case 2: //r
                                    // 建立新的顏色
                                    if (bRGBmix && r > Cut && (g != 0 || b != 0))
                                    { newColor = Color.FromArgb(r, g, b); }
                                    else
                                    { newColor = Color.FromArgb(r, 0, 0); }
                                    break;
                                default:
                                    Console.WriteLine("Error1");
                                    break;
                            }
                            // 設定輸出圖片的像素
                            outputImage.SetPixel(j, y, newColor);
                        }
                    }
                }

                // 儲存輸出圖片
                outputImage.Save(outputImagePath, ImageFormat.Png);//或其他你想要的格式

                // 釋放資源
                inputImage.Dispose();
                outputImage.Dispose();

                Console.WriteLine("圖片處理完成，已儲存至 " + outputImagePath);
                toolStripStatusLabel1.Text = "圖片處理完成，已儲存至 " + outputImagePath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder); //打開輸出圖檔所在資料夾:2025-06-18
            }
            catch (Exception ex)
            {
                Console.WriteLine("發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "發生錯誤: " + ex.Message;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Environment.Exit(Environment.ExitCode);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit？", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

            }
            else { e.Cancel = true; }
        }

        protected override void WndProc(ref Message m) 
        {
            const int WM_DEVICECHANGE = 0x219;
            const int DBT_DEVICEARRIVAL = 0x8000;
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            const int SC_CLICK = 0xF093;
            const int SC_DOUBLE_CLICK = 0xF063;

            object ojb = new object();
            try   //2023-09-25
            {
                // WM_DEVICECHANGE Message : 電腦硬體裝置改變時產生的訊息
                if (m.Msg == WM_DEVICECHANGE)
                {
                    switch (m.WParam.ToInt32())
                    {
                        case WM_DEVICECHANGE:
                            Console.WriteLine("DEVICE CHANGEed");
                            break;
                        // DBT_DEVICEARRIVAL Event : 裝置插入並且可以使用時，產生的系統訊息
                        case DBT_DEVICEARRIVAL:
                            string[] portnames = SerialPort.GetPortNames();
                            Console.WriteLine("DEVICE was inserted.");
                            toolStripStatusLabel1.Text = "DEVICE was inserted";
                            break;

                        // DBT_DEVICEREMOVECOMPLETE Event : 裝置卸載或移除時產生的系統訊息
                        case DBT_DEVICEREMOVECOMPLETE:
                            portnames = SerialPort.GetPortNames();
                            Console.WriteLine("DEVICE was removed.");
                            toolStripStatusLabel1.Text = "DEVICE was removed";
                            break;
                    }
                }
                else if (m.Msg == WM_SYSCOMMAND)
                {
                    if (m.WParam.ToInt32() == SC_CLOSE)
                    {
                        // 点击winform右上关闭按钮 
                        // 加入想要的逻辑处理
                        //utilWindowClosingWithConfirm(false);
                        //return;
                    }
                    else if (m.WParam.ToInt32() == SC_CLICK)
                    {
                        return;
                    }
                    else if (m.WParam.ToInt32() == SC_DOUBLE_CLICK)
                    {                        
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WndProc");
            }
            base.WndProc(ref m);
        }
        static string AddSpaceEveryNChar(string str, int split)   //2023-08-11
        {
            for (int a = 2; a <= str.Length - 1; a = a + split + 1)
            {
                str = str.Insert(a, "-");
            }
            Console.WriteLine(str);
            return str;
        }
    }
}
