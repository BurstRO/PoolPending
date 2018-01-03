using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace roPendingNotifyer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBoxBurstAddress.Text = Properties.Settings.Default.BurstAddress.ToString();
            backgroundWorker1.RunWorkerAsync();
            notifyIcon.Visible = true;
            this.ShowInTaskbar = false;

        }
        public string pendingBurst = "Pending:";
        public string lastAnnouncedPB = "Pending:";
        private void textBoxBurstAddress_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BurstAddress = textBoxBurstAddress.Text;
            Properties.Settings.Default.Save();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while(true)
            {
                try
                {
                    var url = "http://pool.burstcoin.ro/pending2.json";
                    // for a simple get, use WebClient
                    var json = new WebClient().DownloadString(url);
                    var root = JsonConvert.DeserializeObject<PendingBurst>(json);
                    string process = root.pendingPaymentList.ToString().Replace("\r\n", "");
                    JObject pending = (JObject)JsonConvert.DeserializeObject(process);
                    foreach (var pen in pending)
                    {
                        if (pen.Key.ToString().Equals(textBoxBurstAddress.Text.ToString()))
                        {
                            pendingBurst = pen.Value.ToString();
                            break;
                        }
                    }
                }
                catch (Exception d) { };
                System.Threading.Thread.Sleep(60000);
                // Maybe you would like to check if the worker cancelled
                if (backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    // return from method here
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelPending.Text = "Pending: "+pendingBurst;
            notifyIcon.Text = "Pending: " + pendingBurst;
            if (!pendingBurst.Equals(lastAnnouncedPB))
            {
                notifyIcon.BalloonTipText = labelPending.Text;
                notifyIcon.ShowBalloonTip(3000);
                lastAnnouncedPB = pendingBurst;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;
            }
        }
    }
}
