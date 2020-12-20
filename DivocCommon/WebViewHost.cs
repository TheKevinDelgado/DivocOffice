using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;

namespace DivocCommon
{
    public partial class WebViewHost : Form
    {
        public WebViewHost()
        {
            InitializeComponent();
        }

        private void WebViewHost_Load(object sender, EventArgs e)
        {
        }

        private void webViewCtrl_CoreWebView2Ready(object sender, EventArgs e)
        {
            Debug.WriteLine("webViewCtrl_CoreWebView2Ready");
        }

        private void webViewCtrl_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_NavigationStarting");
        }

        private void webViewCtrl_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_NavigationCompleted");
        }

        private void webViewCtrl_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_ContentLoading");
        }

        private void webViewCtrl_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_SourceChanged");
        }

        private void WebViewHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            //webViewCtrl.Dispose();
        }
    }
}
