/*****************************************************************************
 **
 **  (c) All Rights Reserved.
 **  
 **
 **
 ** 打開輸出圖檔所在資料夾:2025-06-18
 ** MacAdress格式更新，2025-06-19
 ** btnPxsToHex_Click，2025-07-23
 ** btn1from9_Click，3x3 pxs select 1 px at center，2025-07-30
 ** btn1from9_Click add 3x3 or 9x1 selection，2025-07-31
 ** btnPicToDec_Click，將圖檔轉為RGB565分兩個Byte傳輸，共72*176*2=25344 bytes，2025-08-05
 ** btnPicToDec_Click程序中加入 resize圖片為72x176 pxs，2025-08-08
 ** 
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
            FilePath1.ReadOnly = true; FilePath2.ReadOnly = true; richTextBox1.ReadOnly = true;
            FilePath1.BackColor = SystemColors.Info;
            toolStripStatusLabel1.Text = "Select a image file to convert.";
            btnToBGR.Enabled = false; btnPxsToHex.Enabled = false; //2025-07-22
            btn1from9.Enabled = false; //2025-07-30
            btnDecToHex.Enabled = false;    //2025-08-04
            btnPicToDec.Enabled = false; //2025-08-05
            btnPicToDec.ForeColor = Color.Red;

            txtMac.Text = GetMacAddress().ToString();
            lblSWver.Text = /*"SW : " +*/ fileName + " - ver: " + strSwVer + " ; 程式碼日期 : " + buildDateTime;
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
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Environment.Exit(Environment.ExitCode);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure to exit？", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {  }
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
            { MessageBox.Show(ex.Message, "WndProc"); }
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

        private void btnImgSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Select A File";
            openDialog.Filter = "Image Files (*.png;*.jpg;*.bmp;*.gif)|*.png;*.jpg;*.bmp;*.gif" + "|" + "All Files (*.*)|*.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openDialog.FileName;
                FilePath1.Text = file;
                toolStripStatusLabel1.Text = "InputFile= " + FilePath1.Text;
            }
            if (FilePath1.Text != "") { btnToBGR.Enabled = true; btnPxsToHex.Enabled = true; btn1from9.Enabled = true; btnPicToDec.Enabled = true; }  //2025-08-06
            else { btnToBGR.Enabled = false; btnPxsToHex.Enabled = false; btn1from9.Enabled = false; btnPicToDec.Enabled = false; }

            toolStripStatusLabel1.Text = "按下 ToBGRimage 或者 To_Hex 按鈕以產出結果";
        }

        private void btnToBGR_Click(object sender, EventArgs e) //圖片pxs轉成RGB分開的3pxs，72*176 pxs會轉成216*176 pxs的圖片
        {
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   //"D:\\temporary\\imgs\\ITRI_img1-72x176.png"; //"input.png"; // 替換為你的輸入圖片路徑
            string outputImagePath = userAppFolder + "\\pxToRGB.png";    //"output.png"; // 替換為你的輸出圖片路徑
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
                                    Console.WriteLine("Error at btnToBGR_Click");
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
                Console.WriteLine("btnToBGR_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btnToBGR_Click" + "發生錯誤: " + ex.Message;
            }
        }


        private void btnPxsToHex_Click(object sender, EventArgs e)  //2025-07-22：圖片pxs轉成十六進位數字
        {
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\PxsToHex.text";    //"輸出路徑檔案
            utilCheckUserFile();
            //2025-07-23
            Boolean bRGBseparate = true; //RGB是否要分色
            File.Delete(outputTextPath); //刪除舊檔案  
            richTextBox1.Text = ""; //清空richTextBox1
            Boolean bPxsToDec = true; //是否要轉成十進位數字，預設為false
            int iWidthDiv = 1; //取RGB單一px時，原圖width每iWidthDiv個px取中間1px出來

            try
            {
                // 讀取圖片
                Bitmap inputImage = new Bitmap(inputImagePath);

                // 遍歷每個像素
                for (int y = 0; y < inputImage.Height; y++)
                {
                    for (int x = 0; x < inputImage.Width/ iWidthDiv; x++)   //2025-08-05
                    {
                        // 獲取像素的 RGB 值
                        Color pixelColor = inputImage.GetPixel(x, y);
                        byte r = pixelColor.R;
                        byte g = pixelColor.G;
                        byte b = pixelColor.B;
                        Color newColor = Color.FromArgb(0, 0, 0);   //default
                        byte Cut = 8;  //no show out if under the value

                        if (bPxsToDec)  //2025-08-05
                        {
                            if (bRGBseparate)
                            {
                                if (x % 3 == 1 && r > Cut)
                                { richTextBox1.AppendText("255,"); }
                                else { richTextBox1.AppendText("0,"); ; }
                            }
                            else
                            {
                                if (r > Cut || g > Cut || b > Cut)
                                { richTextBox1.AppendText("255,"); }
                                else { richTextBox1.AppendText("0,"); ; }
                            }
                        }
                        else
                        {
                            if (bRGBseparate)   //2025-07-23
                            {
                                if (x % 3 == 1 && r > Cut)
                                { richTextBox1.AppendText("0xFF,"); }
                                else { richTextBox1.AppendText("0x00,"); ; }
                            }
                            else
                            {
                                if (r > Cut || g > Cut || b > Cut)
                                { richTextBox1.AppendText("0xFF,"); }
                                else { richTextBox1.AppendText("0x00,"); ; }
                            }
                        }                                              
                    }
                    richTextBox1.AppendText("\r");
                }                  
                File.AppendAllText(outputTextPath, richTextBox1.Text);

                // 釋放資源
                inputImage.Dispose();

                Console.WriteLine("圖片處理完成，已儲存至 " + outputTextPath);
                toolStripStatusLabel1.Text = "圖片處理完成，已儲存至 " + outputTextPath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder); //打開輸出圖檔所在資料夾:2025-06-18
            }
            catch (Exception ex)
            {
                Console.WriteLine("btnPxsToHex_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btnPxsToHex_Click" + "發生錯誤: " + ex.Message;
            }
            finally
            { toolStripStatusLabel1.Text = "PxsToHex Done."; }
        }

        private void btn1from9_Click(object sender, EventArgs e)    //2025-07-30：圖片pxs從9pxs中只擷取1px出來
        {
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   //"D:\\temporary\\imgs\\ITRI_img1-72x176.png"; //"input.png"; // 替換為你的輸入圖片路徑
            string outputImagePath = userAppFolder + "\\1pxFrom9.png";    //"output.png"; // 替換為你的輸出圖片路徑
            utilCheckUserFile(); Boolean bRGBmix = false; //非純RGB是否要混色
            int iWidthDiv = 3; //原圖width每iWidthDiv個px取中間1px出來
            int iHeightDiv = 3; //原圖Height每iHeightDiv個px取中間1px出來
            Boolean bSelect1from3x3 = false; //選擇是否1px從3x3 px中取出來

            try
            {
                if (!bSelect1from3x3)   //2025-07-31
                {
                    iWidthDiv = 9; //原圖width每iWidthDiv個px取中間1px出來
                    iHeightDiv = 1; //原圖Height每iHeightDiv個px取中間1px出來
                }

                // 讀取圖片
                Bitmap inputImage = new Bitmap(inputImagePath);
                // 建立輸出圖片:原圖每3px取中間1px出來
                Bitmap outputImage = new Bitmap(inputImage.Width / iWidthDiv, inputImage.Height/ iHeightDiv);

                int j = 0;

                // 遍歷每個像素
                for (int y = 0 + (int)Math.Round((double)(iHeightDiv / 2)); y < inputImage.Height; y += iHeightDiv)
                {
                    int i = 0;
                    for (int x = 0 + (int)Math.Round((double)(iWidthDiv / 2)); x < inputImage.Width; x += iWidthDiv)
                    {
                        // 獲取像素的 RGB 值
                        Color pixelColor = inputImage.GetPixel(x, y);
                        byte r = pixelColor.R;
                        byte g = pixelColor.G;
                        byte b = pixelColor.B;
                        Color newColor = Color.FromArgb(0, 0, 0);   //default
                        byte Cut = 8;  //no show out if under the value

                        if (r > Cut || g > Cut || b > Cut)
                        { newColor = Color.FromArgb(r, g, b); }
                        else
                        { newColor = Color.FromArgb(0, 0, 0); } //不顯示

                        // 設定輸出圖片的像素
                        outputImage.SetPixel(i, j, newColor);
                        i += 1;
                    }
                    j += 1;
                }

                // 儲存輸出圖片
                outputImage.Save(outputImagePath, ImageFormat.Png);//或其他你想要的格式

                // 釋放資源
                inputImage.Dispose();
                outputImage.Dispose();

                Console.WriteLine("圖片處理完成，已儲存至 " + outputImagePath);
                toolStripStatusLabel1.Text = "圖片處理完成，已儲存至 " + outputImagePath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("btn1from9_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btn1from9_Click" + "發生錯誤: " + ex.Message;
            }
        }

        private void btn25344_Click(object sender, EventArgs e) //2025-08-04：選取Text檔案，當下還用不到
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Title = "Select A File";
            openDialog.Filter = "Text Files (*.txt;*.c;*.h;*.html)|*.txt;*.c;*.h;*.html" + "|" + "All Files (*.*)|*.*";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string file = openDialog.FileName;
                FilePath2.Text = file;
                toolStripStatusLabel1.Text = "InputFile= " + FilePath2.Text;
            }
            if (FilePath2.Text != "") { btnDecToHex.Enabled = true; }
            else { btnDecToHex.Enabled = false; }

            toolStripStatusLabel1.Text = "按下 ToBGRimage 或者 To_Hex 按鈕以產出結果";
        }

        private void btnDecToHex_Click(object sender, EventArgs e) //2025-08-04：Decimal 轉 Hex，功能先不需要，暫時保留，後續應可加上其他功能
        {
            // 設定輸入和輸出檔案路徑
            string inputTextPath = FilePath2.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\DecToHex.text";    //"輸出路徑檔案
            utilCheckUserFile();

            File.Delete(outputTextPath); //刪除舊檔案  
            richTextBox1.Text = ""; //清空richTextBox1

            try
            {
                // 讀取檔案
                string[] lines = File.ReadAllLines(inputTextPath);

                Console.WriteLine("圖片處理完成，已儲存至 " + outputTextPath);
                toolStripStatusLabel1.Text = "圖片處理完成，已儲存至 " + outputTextPath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder); //打開輸出圖檔所在資料夾
            }
            catch (Exception ex)
            {
                Console.WriteLine("btnDecToHex_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btnDecToHex_Click" + "發生錯誤: " + ex.Message;
            }
            finally
            { toolStripStatusLabel1.Text = "DecToHex Done."; }
        }

        private void btnPicToDec_Click(object sender, EventArgs e)  //2025-08-05：72x176 pxs圖片轉RGB565分兩個Byte傳輸，72*176*2=25344 bytes 
        {   //在ITRI的LED pannel上驗證OK
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\PxsToDec.text";    //"輸出路徑檔案
            string outputImagePath = userAppFolder + "\\ImgResizeTo72x176.png";
            utilCheckUserFile();

            File.Delete(outputTextPath); //刪除舊檔案  
            richTextBox1.Text = ""; //清空richTextBox1

            try
            {
                // 讀取圖片
                Bitmap inputImage = new Bitmap(inputImagePath);

                //resize圖片為72x176 pxs
                if (inputImage.Width != 72 || inputImage.Height != 176)
                {
                    Bitmap resizedImage = new Bitmap(inputImage, new Size(72, 176));
                    inputImage.Dispose(); //釋放原圖資源
                    inputImage = resizedImage; //使用新的調整大小的圖片
                    //儲存輸出圖片                    
                    File.Delete(outputImagePath); //刪除舊檔案 
                    resizedImage.Save(outputImagePath, ImageFormat.Png); //或其他你想要的格式
                }

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
                        byte Cut = 8;  //no show out if under the value
                        int RGB565data1 = 0; //高位Byte
                        int RGB565data2 = 0; //低位Byte

                        //if (r > Cut || g > Cut || b > Cut) //將RGB值轉為最大值255
                        //{ r = 255; g = 255; b = 255; }

                        ushort rgb565Color = ConvertToRGB565(r, g, b);
                        Console.WriteLine($"RGB565 Color: {rgb565Color}");
                        //richTextBox1.AppendText(rgb565Color + ",");
                        RGB565data1 = (rgb565Color >> 8) & 0xFF; //高位Byte
                        richTextBox1.AppendText(RGB565data1 + ",");
                        RGB565data2 = rgb565Color & 0xFF; //低位Byte
                        richTextBox1.AppendText(RGB565data2 + ",");
                    }
                    //richTextBox1.AppendText("\r");
                }
                File.AppendAllText(outputTextPath, richTextBox1.Text);

                // 釋放資源
                inputImage.Dispose();

                Console.WriteLine("圖片處理完成，已儲存至 " + outputTextPath);
                toolStripStatusLabel1.Text = "圖片處理完成，已儲存至 " + outputTextPath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder);
            }
            catch (Exception ex)
            {
                Console.WriteLine("btnPicToDec_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btnPicToDec_Click" + "發生錯誤: " + ex.Message;
            }
            finally
            { toolStripStatusLabel1.Text = "btnPicToDec_Click Done."; }
        }
        public static int Clamp(int value, int min, int max)    //Math未包含Clamp的定義，因此自行定義
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public ushort ConvertToRGB565(int r, int g, int b)  //RGB888 to RGB565
        {
            // 确保 RGB 值在 0 到 255 之间
            r = /*Math.*/Clamp(r, 0, 255);
            g = /*Math.*/Clamp(g, 0, 255);
            b = /*Math.*/Clamp(b, 0, 255);

            // 转换到 RGB565
            ushort rgb565 = (ushort)((r >> 3) << 11 | (g >> 2) << 5 | (b >> 3));

            return rgb565;
        }

    }
}
