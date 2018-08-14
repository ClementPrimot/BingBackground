using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Configuration;
using System.Xml; 

namespace BingBackground
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.NotifyIcon trayIcon = new NotifyIcon();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(
                                int uAction, int uParam,
                                string lpvParam, int fuWinIni);
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_SENDCHANGE = 0x2; 


 

        public Form1()
        {
            InitializeComponent();

            trayIcon.Text = "Bing Background";
            trayIcon.Icon = new Icon("icon.ico");
            trayIcon.Visible = true;
            trayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClick);

            bgupdater.Interval = Int32.Parse(ConfigurationManager.AppSettings["updatetime"]);

            UpdateWallpaper();
        }

        public void SetWallpaper(String path)
        { 
            SystemParametersInfo(0x0014, 0, AppDomain.CurrentDomain.BaseDirectory+"Background\\"+path, 0x0001);
        }

        public void UpdateWallpaper()
        {
            WebClient client = new WebClient();
            XmlDocument rssXmlDoc = new XmlDocument();
            rssXmlDoc.Load("https://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&mkt=" + ConfigurationManager.AppSettings["location"]);
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("images/image");
            StringBuilder rssContent = new StringBuilder();
            string backgroundurl = "";
            foreach (XmlNode rssNode in rssNodes)
            {
                XmlNode rssSubNode = rssNode.SelectSingleNode("url");
                backgroundurl = rssSubNode != null ? rssSubNode.InnerText : ""; 
            }

            var fileCount = (from file in Directory.EnumerateFiles(@"Background/", "*.jpg", SearchOption.AllDirectories)
                             select file).Count();
            backgroundurl = backgroundurl.Replace("1366x768", ConfigurationManager.AppSettings["resolution"]);
            client.DownloadFile("http://bing.com" + backgroundurl, "Background/" + fileCount + ".jpg"); 
            SetWallpaper( fileCount  + ".jpg");

            if (ConfigurationManager.AppSettings["save"] == "false")
            {
                File.Delete("Background/" + fileCount + ".jpg");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        { 
            Visible       = false;
            ShowInTaskbar = false;
            Icon = new Icon("icon.ico"); 
        } 

        private void trayIcon_DoubleClick(object Sender, EventArgs e)
        {
            Visible = true;
            ShowInTaskbar = true;
        } 

        private void button1_Click(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Volting/BingBackground");
        }

        private void bgupdater_Tick(object sender, EventArgs e)
        {
            UpdateWallpaper();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bing Background by Volting\n\n"+
                            "Update interval (Based on timer): "+bgupdater.Interval+" sec \n"+
                            "Save background (Based on config): " + ConfigurationManager.AppSettings["save"] + "\n"+
                            "Background Location (Based on config): " + ConfigurationManager.AppSettings["location"]);
        }
    }
}
