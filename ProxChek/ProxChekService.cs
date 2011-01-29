using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

using Microsoft.Win32;

namespace ProxChek
{
    partial class ProxChekService : ServiceBase
    {
        private const int _secondsToSleep = 15;
        private bool _stopThread = false;
        private Thread _proxyEnabledCheckThread;

        public ProxChekService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _stopThread = false;
            _proxyEnabledCheckThread = new Thread(new ThreadStart(proxyRegistryValueCheck));
            _proxyEnabledCheckThread.Start();
        }

        protected override void OnStop()
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
