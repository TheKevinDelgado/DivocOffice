using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DivocCommon;
using Serilog;
using Serilog.Core;

namespace TestShell
{
    public partial class TestShell : Form
    {
        readonly Logger _log = null;

        public TestShell()
        {
            InitializeComponent();

            _log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\TestShell.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            _log.Information("TestShell -> TestShell");
        }

        private void ListBoxCommonDialogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.listBoxCommonDialogs.SelectedIndex > -1)
            {
                this.buttonRunCommonDialog.Enabled = true;
            }
            else
            {
                this.buttonRunCommonDialog.Enabled = false;
            }
        }

        private void ButtonRunCommonDialog_Click(object sender, EventArgs e)
        {
            if (listBoxCommonDialogs.SelectedItem.ToString() == "WebBrowserControl")
            {
                WebBrowserHost webBrowserHost = new WebBrowserHost();

                webBrowserHost.ShowDialog();
            }
            else
            {
                WebViewHost webViewHost = new WebViewHost();

                webViewHost.ShowDialog();
            }
        }

        private async void TestShell_Load(object sender, EventArgs e)
        {
            AuthenticationManager auth = new AuthenticationManager();
            await auth.Authenticate(this.Handle);
        }
    }
}
