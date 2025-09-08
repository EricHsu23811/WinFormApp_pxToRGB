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
 ** btnDecToHex_Click，Decimal 轉 Hex，並將RGB565轉回RGB888圖檔=>OK，2025-08-19
 ** btnPicToDec_Click，新增bRGB888toSingle=true時，會讀取216*176 pxs圖片用單R色產出25344 bytes，2025-08-20
 ** 版本1.0.3.0；新增tabControl2加入用以產生輸入Bestom燒錄工具字元的功能，之前的程式碼功能放在tabPage1， 2025-09-08
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
        Boolean bRemoveTabpge1=true; //預設不顯示tabPage1，2025-09-08

        private void Form1_Load(object sender, EventArgs e)
        {
            FilePath1.ReadOnly = true; FilePath2.ReadOnly = true; 
            richTextBox1.ReadOnly = true; richTextBox2.ReadOnly = true;
            FilePath1.BackColor = SystemColors.Info;
            toolStripStatusLabel1.Text = "Select a image file to convert.";
            btnToBGR.Enabled = false; btnPxsToHex.Enabled = false; //2025-07-22
            btn1from9.Enabled = false; //2025-07-30
            btnDecToHex.Enabled = false;    //2025-08-04
            btnPicToDec.Enabled = false; //2025-08-05
            btnPicToDec.ForeColor = Color.Red;

            txtMac.Text = GetMacAddress().ToString();
            lblSWver.Text = /*"SW : " +*/ fileName + " - ver: " + strSwVer + " ; 程式碼日期 : " + buildDateTime;

            if (bRemoveTabpge1) //預設不顯示tabPage2，2025-09-08
            {
                tabControl1.TabPages.Remove(tabPage1);
                toolStripStatusLabel1.Text = "請輸入9x7字元陣列，按下 ArrToText 轉成文字";
            }
            
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


        private void btnPxsToHex_Click(object sender, EventArgs e)  //2025-07-22：圖片pxs轉成十六進位數字(img_to_Hex/Dec)
        {
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\PxsToHex.text";    //"輸出路徑檔案
            utilCheckUserFile();
            //2025-07-23
            Boolean bRGBseparate = true; //RGB是否要分色
            //File.Delete(outputTextPath); //刪除舊檔案  =>改為File.WriteAllText就不必先delete舊檔案
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
                        byte Cut = 0;  //no show out if under the value

                        if (bPxsToDec)  //2025-08-05
                        {
                            if (bRGBseparate)
                            {
                                if (/*x % 3 == (int)pxColor.Red &&*/ r > Cut)
                                { richTextBox1.AppendText(r.ToString() + ","); }    //2025-08-13
                                else { richTextBox1.AppendText("0,"); }
                            }
                            else
                            {
                                if (r > Cut || g > Cut || b > Cut)
                                { richTextBox1.AppendText( "255,"); }   
                                else { richTextBox1.AppendText("0,"); }
                            }
                        }
                        else
                        {
                            if (bRGBseparate)   //2025-07-23
                            {
                                if (/*x % 3 == (int)pxColor.Red &&*/ r > Cut)
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
                /*File.AppendAllText*/
                File.WriteAllText(outputTextPath, richTextBox1.Text);   //2025-09-03

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

        private void btn25344_Click(object sender, EventArgs e) //2025-08-04：選取Text檔案
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

        private void btnDecToHex_Click(object sender, EventArgs e) //2025-08-19：將RGB565轉回RGB888圖檔=>OK；btnPicToDec_Click的反向操作
        {
            // 設定輸入和輸出檔案路徑
            string inputTextPath = FilePath2.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\DecToHex.text";    //"輸出路徑檔案
            utilCheckUserFile();
            //File.Delete(outputTextPath); //刪除舊檔案  =>改為File.WriteAllText就不必先delete舊檔案
            richTextBox1.Text = ""; //清空richTextBox1

            string outputImagePath = userAppFolder + "\\DecToImg.png";    //2025-08-18："輸出圖檔路徑檔案
            File.Delete(outputImagePath); //刪除舊檔案  

            int iWidth = 72*2;  int iHeight = 176;    //轉72x176 pxs圖片
            int RGB565data1 = 0; //高位Byte
            int RGB565data2 = 0; //低位Byte

            // 建立輸出圖片:2025-08-19
            Bitmap outputImage = new Bitmap(iWidth / 2, iHeight);
            Color newColor = Color.FromArgb(0, 0, 0);   //default
            int n = 0; //計數器

            try
            {
                // 讀取輸入檔案內容
                string inputText = File.ReadAllText(inputTextPath);
                // 移除大括號並分割字串
                string[] numbers = inputText.Trim('{', '}').Split(','); 
                Console.WriteLine(numbers.Length + " numbers found in input file.");    //numbers.Length：輸入檔案有多少個數字，應為72*176*2=25344

                if (numbers.Length > iWidth * iHeight)     //超出陣列的就不要，陣列應為25344個元素
                { Array.Resize(ref numbers, iWidth * iHeight); }

                // 建立一個 List 來儲存要寫入檔案的行
                List<string> outputLines = new List<string>();

                // 取用 iWidth 個數字為一行
                for (int i = 0; i < numbers.Length; i += iWidth)
                {
                    // 提取 iWidth 個數字，如果不足 ，則取剩餘的
                    string[] lineNumbers = numbers.Skip(i).Take(iWidth).ToArray();
                    
                    Console.WriteLine("565data[1] = " + lineNumbers[0]);   //lineNumbers[0]); 
                    Console.WriteLine("565data[2] = " + lineNumbers[1]);   //lineNumbers[1]);
                    //Console.WriteLine("565data[3] = " + lineNumbers[2]);
                    //Console.WriteLine("565data[4] = " + lineNumbers[3]);                    

                    for (int m=0; m < lineNumbers.Length; m++)  //2025-08-18
                    {                        
                        if (int.TryParse(lineNumbers[m], out int number))  //檢查是否為有效的整數
                        {
                            if (m % 2 == 0) //偶數位為高位Byte
                            { RGB565data1 = number; }
                            else //奇數位為低位Byte
                            { 
                                RGB565data2 = number;

                                // RGB565轉為RGB888
                                var rgb888 = ConvertToRGB888(RGB565data1, RGB565data2);
                                newColor = Color.FromArgb(rgb888.r, rgb888.g, rgb888.b);    //2025-08-19
                                
                                // 設定輸出圖片的像素
                                outputImage.SetPixel((m-1) / 2, n, newColor); //2025-08-19
                            }    
                        }
                        else
                        {
                            Console.WriteLine("無效的數字: " + lineNumbers[m]);
                            toolStripStatusLabel1.Text = "無效的數字: " + lineNumbers[m] + " (must be int 0~255) ";
                            return; //跳出循環
                        }
                    }
                    // 將數字用逗號分隔，組成一行
                    string line = string.Join(",", lineNumbers);

                    // 將該行加入到輸出行列表中
                    outputLines.Add(line);
                    richTextBox1.AppendText(line + ",\r");

                    n++;    //2025-08-19：每處理一行就增加n，代表輸出圖片的y座標
                }
                // 儲存輸出圖片
                outputImage.Save(outputImagePath, ImageFormat.Png);//或其他你想要的格式，outputImage.SetPixel計數器

                // 釋放資源
                outputImage.Dispose();

                File.WriteAllText(outputTextPath, richTextBox1.Text);   //2025-09-03

                Console.WriteLine("處理完成，已儲存至 " + outputTextPath);
                toolStripStatusLabel1.Text = "處理完成，已儲存至 " + outputTextPath;
                //Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                Process.Start("explorer.exe", userAppFolder); //打開輸出圖檔所在資料夾
            }
            catch (Exception ex)
            {
                Console.WriteLine("btnDecToHex_Click" + "發生錯誤: " + ex.Message);
                toolStripStatusLabel1.Text = "btnDecToHex_Click" + "發生錯誤: " + ex.Message;
            }
            finally
            { toolStripStatusLabel1.Text += "DecToHex Done."; }
        }

        private void btnPicToDec_Click(object sender, EventArgs e)  //2025-08-05(ImgToDec[25344])：72x176 pxs圖片轉RGB565分兩個Byte傳輸，72*176*2=25344 bytes 
        {   //在ITRI的LED pannel上驗證OK
            // 設定輸入和輸出檔案路徑
            string inputImagePath = FilePath1.Text;   // 替換為輸入圖片路徑
            string outputTextPath = userAppFolder + "\\PxsToDec.text";    //"輸出路徑檔案
            string outputImagePath = userAppFolder + "\\ImgResizeTo72x176.png";
            utilCheckUserFile();

            //File.Delete(outputTextPath); //刪除舊檔案=>改為File.WriteAllText就不必先delete舊檔案
            richTextBox1.Text = ""; //清空richTextBox1

            Boolean bRGB888toSingle = false; //2025-08-20：是否要RGB888(216*176pxs)轉為單色，預設為false(圖片應為72x176 pxs)

            try
            {
                // 讀取圖片
                Bitmap inputImage = new Bitmap(inputImagePath);
                byte Cut = 8;  //超過某數值的顏色才顯示出來，預設為8
                byte r = 0;
                byte g = 0;
                byte b = 0;
                int RGB565data1 = 0; //高位Byte
                int RGB565data2 = 0; //低位Byte

                if (bRGB888toSingle) //2025-08-20：是否RGB888圖片(216*176 pxs)只擷取R單色？=>再轉換ConvertToRGB565
                {
                    //將216*176 pxs圖片轉為單色
                    Bitmap singleColorImage = new Bitmap(inputImage.Width, inputImage.Height);
                    for (int y = 0; y < inputImage.Height; y++)
                    {
                        for (int x = 0; x < inputImage.Width; x++)
                        {
                            // 獲取像素的 RGB 值
                            Color pixelColor = inputImage.GetPixel(x, y);

                            if (x % 3 == (int)pxColor.Red)
                            {
                                r = pixelColor.R; //取1st px的R值
                            }
                            else if (x % 3 == (int)pxColor.Green)
                            {
                                g = pixelColor.R; //取2nd px的R值
                            }
                            else //if (x % 3 == (int)pxColor.Blue)
                            {
                                b = pixelColor.R; //取3rd px的R值
                                ushort rgb565Color = ConvertToRGB565(r, g, b);  // 轉換ConvertToRGB565
                                Console.WriteLine($"RGB565 Color: {rgb565Color}");
                                //richTextBox1.AppendText(rgb565Color + ",");
                                RGB565data1 = (rgb565Color >> 8) & 0xFF; //高位Byte
                                richTextBox1.AppendText(RGB565data1 + ",");
                                RGB565data2 = rgb565Color & 0xFF; //低位Byte
                                if (y == inputImage.Height - 1 && x == inputImage.Width - 1)
                                { richTextBox1.AppendText(RGB565data2.ToString() /*+ "\r"*/); } //最後一個pxs不加逗號  //最後一個pxs換行
                                else
                                { richTextBox1.AppendText(RGB565data2 + ","); }
                            }
                            //byte grayValue = (byte)((pixelColor.R + pixelColor.G + pixelColor.B) / 3); //計算灰階值
                            //singleColorImage.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                        }
                    }
                    inputImage.Dispose(); //釋放原圖資源
                    inputImage = singleColorImage; //使用新的單色圖片
                }
                else
                {
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
                            r = pixelColor.R;
                            g = pixelColor.G;
                            b = pixelColor.B;

                            //int RGB565data1 = 0; //高位Byte
                            //int RGB565data2 = 0; //低位Byte

                            ushort rgb565Color = ConvertToRGB565(r, g, b);  // 轉換ConvertToRGB565
                            Console.WriteLine($"RGB565 Color: {rgb565Color}");
                            //richTextBox1.AppendText(rgb565Color + ",");
                            RGB565data1 = (rgb565Color >> 8) & 0xFF; //高位Byte
                            richTextBox1.AppendText(RGB565data1 + ",");
                            RGB565data2 = rgb565Color & 0xFF; //低位Byte

                            if (y == inputImage.Height - 1 && x == inputImage.Width - 1) 
                            { richTextBox1.AppendText(RGB565data2.ToString() /*+ "\r"*/); } //最後一個pxs不加逗號  //最後一個pxs換行
                            else
                            { richTextBox1.AppendText(RGB565data2 + ","); }
                        }
                    }                
                }
                File.WriteAllText(outputTextPath, richTextBox1.Text);  //2025-09-03, File.AppendAllText

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
        public (int r, int g, int b) ConvertToRGB888(int RGB565data1, int RGB565data2)  //2025-08-18，RGB565 to RGB888
        {
            // 轉換到 RGB888
            ushort rgb565= (ushort)(RGB565data1<<8 | RGB565data2);

            // 提取 RGB565 分量
            int r5 = (rgb565 >> 11) & 0x1F;  // 0x1F = 00011111 (5 位)
            int g6 = (rgb565 >> 5) & 0x3F;   // 0x3F = 00111111 (6 位)
            int b5 = rgb565 & 0x1F;                // 0x1F = 00011111 (5 位)

            // 扩展到 RGB888
            int r8 = (r5 << 3) | (r5 >> 2);        // 將 5 位擴展到 8 位
            int g8 = (g6 << 2) | (g6 >> 4);     // 將 5 位擴展到 8 位
            int b8 = (b5 << 3) | (b5 >> 2);     // 將 5 位擴展到 8 位

            return (r8, g8, b8);
        }

        private void btnArrToText_Click(object sender, EventArgs e)
        {
            string strInput9x7= inputToChars(txtChar1_1.Text) + inputToChars(txtChar1_2.Text) + inputToChars(txtChar1_3.Text) 
                + inputToChars(txtChar1_4.Text) + inputToChars(txtChar1_5.Text) + inputToChars(txtChar1_6.Text) 
                + inputToChars(txtChar1_7.Text) + inputToChars(txtChar1_8.Text) + inputToChars(txtChar1_9.Text)
                + inputToChars(txtChar2_1.Text) + inputToChars(txtChar2_2.Text) + inputToChars(txtChar2_3.Text)
                + inputToChars(txtChar2_4.Text) + inputToChars(txtChar2_5.Text) + inputToChars(txtChar2_6.Text)
                + inputToChars(txtChar2_7.Text) + inputToChars(txtChar2_8.Text) + inputToChars(txtChar2_9.Text) 
                + inputToChars(txtChar3_1.Text) + inputToChars(txtChar3_2.Text) + inputToChars(txtChar3_3.Text)
                + inputToChars(txtChar3_4.Text) + inputToChars(txtChar3_5.Text) + inputToChars(txtChar3_6.Text)
                + inputToChars(txtChar3_7.Text) + inputToChars(txtChar3_8.Text) + inputToChars(txtChar3_9.Text) 
                + inputToChars(txtChar4_1.Text) + inputToChars(txtChar4_2.Text) + inputToChars(txtChar4_3.Text)
                + inputToChars(txtChar4_4.Text) + inputToChars(txtChar4_5.Text) + inputToChars(txtChar4_6.Text)
                + inputToChars(txtChar4_7.Text) + inputToChars(txtChar4_8.Text) + inputToChars(txtChar4_9.Text) 
                + inputToChars(txtChar5_1.Text) + inputToChars(txtChar5_2.Text) + inputToChars(txtChar5_3.Text)
                + inputToChars(txtChar5_4.Text) + inputToChars(txtChar5_5.Text) + inputToChars(txtChar5_6.Text)
                + inputToChars(txtChar5_7.Text) + inputToChars(txtChar5_8.Text) + inputToChars(txtChar5_9.Text) 
                + inputToChars(txtChar6_1.Text) + inputToChars(txtChar6_2.Text) + inputToChars(txtChar6_3.Text)
                + inputToChars(txtChar6_4.Text) + inputToChars(txtChar6_5.Text) + inputToChars(txtChar6_6.Text)
                + inputToChars(txtChar6_7.Text) + inputToChars(txtChar6_8.Text) + inputToChars(txtChar6_9.Text) 
                + inputToChars(txtChar7_1.Text) + inputToChars(txtChar7_2.Text) + inputToChars(txtChar7_3.Text)
                + inputToChars(txtChar7_4.Text) + inputToChars(txtChar7_5.Text) + inputToChars(txtChar7_6.Text)
                + inputToChars(txtChar7_7.Text) + inputToChars(txtChar7_8.Text) + inputToChars(txtChar7_9.Text);
            richTextBox2.Text = ReverseByArray(strInput9x7);
            toolStripStatusLabel1.Text = "已將9x7陣列轉成字串，按Copy to Clipboard將結果拷貝到剪貼簿中";
        }

        private string inputToChars(string inputStr) //將輸入的字串轉成char陣列
        {
            //char[] chars = inputStr.ToCharArray();
            string result = inputStr.Trim();
            if (result.Length > 0) {
                result = result.Substring(0, 1); //只取第一個字元
            }
            else
            { result = "\u3000"; } //全形空白字元
            return result;
        }

        public string ReverseByArray(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        #region
        private void txtChar1_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_1.Text = ""; }
        private void txtChar1_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_2.Text = ""; }
        private void txtChar1_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_3.Text = ""; }
        private void txtChar1_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_4.Text = ""; }
        private void txtChar1_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_5.Text = ""; }
        private void txtChar1_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_6.Text = ""; }
        private void txtChar1_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_7.Text = ""; }
        private void txtChar1_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_8.Text = ""; }
        private void txtChar1_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar1_9.Text = ""; }
        #endregion

        #region
        private void txtChar2_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_1.Text = ""; }
        private void txtChar2_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_2.Text = ""; }
        private void txtChar2_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_3.Text = ""; }
        private void txtChar2_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_4.Text = ""; }
        private void txtChar2_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_5.Text = ""; }
        private void txtChar2_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_6.Text = ""; }
        private void txtChar2_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_7.Text = ""; }
        private void txtChar2_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_8.Text = ""; }
        private void txtChar2_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar2_9.Text = ""; }
        #endregion

        #region
        private void txtChar3_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_1.Text = ""; }
        private void txtChar3_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_2.Text = ""; }
        private void txtChar3_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_3.Text = ""; }
        private void txtChar3_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_4.Text = ""; }
        private void txtChar3_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_5.Text = ""; }
        private void txtChar3_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_6.Text = ""; }
        private void txtChar3_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_7.Text = ""; }
        private void txtChar3_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_8.Text = ""; }
        private void txtChar3_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar3_9.Text = ""; }
        #endregion

        #region
        private void txtChar4_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_1.Text = ""; }
        private void txtChar4_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_2.Text = ""; }
        private void txtChar4_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_3.Text = ""; }
        private void txtChar4_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_4.Text = ""; }
        private void txtChar4_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_5.Text = ""; }
        private void txtChar4_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_6.Text = ""; }
        private void txtChar4_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_7.Text = ""; }
        private void txtChar4_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_8.Text = ""; }
        private void txtChar4_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar4_9.Text = ""; }
        #endregion

        #region
        private void txtChar5_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_1.Text = ""; }
        private void txtChar5_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_2.Text = ""; }
        private void txtChar5_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_3.Text = ""; }
        private void txtChar5_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_4.Text = ""; }
        private void txtChar5_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_5.Text = ""; }
        private void txtChar5_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_6.Text = ""; }
        private void txtChar5_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_7.Text = ""; }
        private void txtChar5_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_8.Text = ""; }
        private void txtChar5_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar5_9.Text = ""; }
        #endregion

        #region
        private void txtChar6_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_1.Text = ""; }
        private void txtChar6_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_2.Text = ""; }
        private void txtChar6_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_3.Text = ""; }
        private void txtChar6_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_4.Text = ""; }
        private void txtChar6_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_5.Text = ""; }
        private void txtChar6_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_6.Text = ""; }
        private void txtChar6_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_7.Text = ""; }
        private void txtChar6_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_8.Text = ""; }
        private void txtChar6_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar6_9.Text = ""; }
        #endregion

        #region
        private void txtChar7_1_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_1.Text = ""; }
        private void txtChar7_2_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_2.Text = ""; }
        private void txtChar7_3_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_3.Text = ""; }
        private void txtChar7_4_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_4.Text = ""; }
        private void txtChar7_5_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_5.Text = ""; }
        private void txtChar7_6_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_6.Text = ""; }
        private void txtChar7_7_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_7.Text = ""; }
        private void txtChar7_8_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_8.Text = ""; }
        private void txtChar7_9_KeyDown(object sender, KeyEventArgs e)
        { txtChar7_9.Text = ""; }
        #endregion

        private void btnArrClear_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            txtChar1_1.Text = ""; txtChar1_2.Text = ""; txtChar1_3.Text = ""; 
            txtChar1_4.Text = ""; txtChar1_5.Text = ""; txtChar1_6.Text = "";
            txtChar1_7.Text = ""; txtChar1_8.Text = ""; txtChar1_9.Text = "";
            txtChar2_1.Text = ""; txtChar2_2.Text = ""; txtChar2_3.Text = "";
            txtChar2_4.Text = ""; txtChar2_5.Text = ""; txtChar2_6.Text = "";
            txtChar2_7.Text = ""; txtChar2_8.Text = ""; txtChar2_9.Text = "";
            txtChar3_1.Text = ""; txtChar3_2.Text = ""; txtChar3_3.Text = "";
            txtChar3_4.Text = ""; txtChar3_5.Text = ""; txtChar3_6.Text = "";
            txtChar3_7.Text = ""; txtChar3_8.Text = ""; txtChar3_9.Text = "";
            txtChar4_1.Text = ""; txtChar4_2.Text = ""; txtChar4_3.Text = "";
            txtChar4_4.Text = ""; txtChar4_5.Text = ""; txtChar4_6.Text = "";
            txtChar4_7.Text = ""; txtChar4_8.Text = ""; txtChar4_9.Text = "";
            txtChar5_1.Text = ""; txtChar5_2.Text = ""; txtChar5_3.Text = "";
            txtChar5_4.Text = ""; txtChar5_5.Text = ""; txtChar5_6.Text = "";
            txtChar5_7.Text = ""; txtChar5_8.Text = ""; txtChar5_9.Text = "";
            txtChar6_1.Text = ""; txtChar6_2.Text = ""; txtChar6_3.Text = "";
            txtChar6_4.Text = ""; txtChar6_5.Text = ""; txtChar6_6.Text = "";
            txtChar6_7.Text = ""; txtChar6_8.Text = ""; txtChar6_9.Text = "";
            txtChar7_1.Text = ""; txtChar7_2.Text = ""; txtChar7_3.Text = "";
            txtChar7_4.Text = ""; txtChar7_5.Text = ""; txtChar7_6.Text = "";
            txtChar7_7.Text = ""; txtChar7_8.Text = ""; txtChar7_9.Text = "";
            toolStripStatusLabel1.Text = "已清除9x7陣列，請輸入9x7字元陣列，按下 ArrToText 轉成文字";
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox2.Text);
            toolStripStatusLabel1.Text = "已將結果拷貝到剪貼簿中";
        }

    }
}
