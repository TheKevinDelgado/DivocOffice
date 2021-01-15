
namespace DivocCommon
{
    partial class BrowseHostForm
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
            this.elementHostDriveBrowser = new System.Windows.Forms.Integration.ElementHost();
            this.driveBrowserControl = new DivocCommon.DriveBrowserControl();
            this.SuspendLayout();
            // 
            // elementHostDriveBrowser
            // 
            this.elementHostDriveBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementHostDriveBrowser.Location = new System.Drawing.Point(12, 12);
            this.elementHostDriveBrowser.Name = "elementHostDriveBrowser";
            this.elementHostDriveBrowser.Size = new System.Drawing.Size(776, 426);
            this.elementHostDriveBrowser.TabIndex = 4;
            this.elementHostDriveBrowser.Text = "elementHost1";
            this.elementHostDriveBrowser.Child = this.driveBrowserControl;
            // 
            // BrowseHostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.elementHostDriveBrowser);
            this.Name = "BrowseHostForm";
            this.Text = "Open";
            this.Load += new System.EventHandler(this.OpenForm_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Integration.ElementHost elementHostDriveBrowser;
        private DriveBrowserControl driveBrowserControl;
    }
}