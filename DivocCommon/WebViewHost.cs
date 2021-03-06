﻿using System;
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

        private void WebViewCtrl_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_NavigationStarting");
        }

        private void WebViewCtrl_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_NavigationCompleted");
        }

        private void WebViewCtrl_ContentLoading(object sender, CoreWebView2ContentLoadingEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_ContentLoading");
        }

        private void WebViewCtrl_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            Debug.WriteLine("webViewCtrl_SourceChanged");
        }

        private void WebViewHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            //webViewCtrl.Dispose();
        }
    }
}
