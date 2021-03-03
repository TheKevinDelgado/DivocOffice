
namespace DivocCommon
{
    partial class WebViewHost
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webViewCtrl = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.SuspendLayout();
            // 
            // webViewCtrl
            // 
            this.webViewCtrl.CreationProperties = null;
            this.webViewCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewCtrl.Location = new System.Drawing.Point(0, 0);
            this.webViewCtrl.Name = "webViewCtrl";
            this.webViewCtrl.Size = new System.Drawing.Size(800, 450);
            this.webViewCtrl.Source = new System.Uri("https://www.git-scm.com", System.UriKind.Absolute);
            this.webViewCtrl.TabIndex = 0;
            this.webViewCtrl.ZoomFactor = 1D;
            this.webViewCtrl.NavigationStarting += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs>(this.WebViewCtrl_NavigationStarting);
            this.webViewCtrl.NavigationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs>(this.WebViewCtrl_NavigationCompleted);
            this.webViewCtrl.SourceChanged += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs>(this.WebViewCtrl_SourceChanged);
            this.webViewCtrl.ContentLoading += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2ContentLoadingEventArgs>(this.WebViewCtrl_ContentLoading);
            // 
            // WebViewHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webViewCtrl);
            this.Name = "WebViewHost";
            this.Text = "WebViewHost";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebViewHost_FormClosing);
            this.Load += new System.EventHandler(this.WebViewHost_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webViewCtrl;
    }
}