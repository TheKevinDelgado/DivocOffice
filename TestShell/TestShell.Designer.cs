
namespace TestShell
{
    partial class TestShell
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
            this.listBoxCommonDialogs = new System.Windows.Forms.ListBox();
            this.groupBoxCommonDialogs = new System.Windows.Forms.GroupBox();
            this.buttonRunCommonDialog = new System.Windows.Forms.Button();
            this.groupBoxCommonDialogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBoxCommonDialogs
            // 
            this.listBoxCommonDialogs.FormattingEnabled = true;
            this.listBoxCommonDialogs.Items.AddRange(new object[] {
            "WebView2",
            "WebBrowserControl"});
            this.listBoxCommonDialogs.Location = new System.Drawing.Point(6, 19);
            this.listBoxCommonDialogs.Name = "listBoxCommonDialogs";
            this.listBoxCommonDialogs.Size = new System.Drawing.Size(438, 199);
            this.listBoxCommonDialogs.TabIndex = 0;
            this.listBoxCommonDialogs.SelectedIndexChanged += new System.EventHandler(this.listBoxCommonDialogs_SelectedIndexChanged);
            // 
            // groupBoxCommonDialogs
            // 
            this.groupBoxCommonDialogs.Controls.Add(this.buttonRunCommonDialog);
            this.groupBoxCommonDialogs.Controls.Add(this.listBoxCommonDialogs);
            this.groupBoxCommonDialogs.Location = new System.Drawing.Point(12, 12);
            this.groupBoxCommonDialogs.Name = "groupBoxCommonDialogs";
            this.groupBoxCommonDialogs.Size = new System.Drawing.Size(450, 260);
            this.groupBoxCommonDialogs.TabIndex = 1;
            this.groupBoxCommonDialogs.TabStop = false;
            this.groupBoxCommonDialogs.Text = "Common Dialogs";
            // 
            // buttonRunCommonDialog
            // 
            this.buttonRunCommonDialog.Enabled = false;
            this.buttonRunCommonDialog.Location = new System.Drawing.Point(6, 231);
            this.buttonRunCommonDialog.Name = "buttonRunCommonDialog";
            this.buttonRunCommonDialog.Size = new System.Drawing.Size(75, 23);
            this.buttonRunCommonDialog.TabIndex = 1;
            this.buttonRunCommonDialog.Text = "Run";
            this.buttonRunCommonDialog.UseVisualStyleBackColor = true;
            this.buttonRunCommonDialog.Click += new System.EventHandler(this.buttonRunCommonDialog_Click);
            // 
            // TestShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 289);
            this.Controls.Add(this.groupBoxCommonDialogs);
            this.Name = "TestShell";
            this.Text = "Divoc Test Shell";
            this.Load += new System.EventHandler(this.TestShell_Load);
            this.groupBoxCommonDialogs.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxCommonDialogs;
        private System.Windows.Forms.GroupBox groupBoxCommonDialogs;
        private System.Windows.Forms.Button buttonRunCommonDialog;
    }
}

