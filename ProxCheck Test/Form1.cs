using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using Microsoft.Win32;

namespace ProxCheck_Test
{
    public partial class Form1 : Form
    {
        private const int _secondsToSleep = 15;
        private bool _stopThread = false;
        private Thread _proxyEnabledCheckThread;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            _proxyEnabledCheckThread = new Thread(new ThreadStart(proxyRegistryValueCheck));
            _proxyEnabledCheckThread.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _stopThread = true;
            _proxyEnabledCheckThread.Abort();
        }

        private void proxyRegistryValueCheck()
        {
            RegistryKey proxyEnabledKey = null;
            try
            {
                while (!_stopThread)
                {
                    proxyEnabledKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                    int isProxyEnabled = (int)proxyEnabledKey.GetValue("ProxyEnable");
                    if (isProxyEnabled != 0)
                        proxyEnabledKey.SetValue("ProxyEnable", 0);

                    proxyEnabledKey.Close();
                    proxyEnabledKey = null;
                    Thread.Sleep(_secondsToSleep * 1000);
                }
            }
            catch (ThreadAbortException tae)
            {
                if (proxyEnabledKey != null)
                    proxyEnabledKey.Close();
            }
        }

    }
}
