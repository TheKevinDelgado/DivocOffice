
namespace DivocCommon
{
    partial class SaveWithProgressForm
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
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBarSave = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // labelProgress
            // 
            this.labelProgress.BackColor = System.Drawing.SystemColors.ControlLight;
            this.labelProgress.Location = new System.Drawing.Point(12, 40);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(500, 25);
            this.labelProgress.TabIndex = 0;
            this.labelProgress.Text = "0";
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBarSave
            // 
            this.progressBarSave.Location = new System.Drawing.Point(12, 12);
            this.progressBarSave.Name = "progressBarSave";
            this.progressBarSave.Size = new System.Drawing.Size(500, 25);
            this.progressBarSave.TabIndex = 1;
            // 
            // SaveWithProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(524, 77);
            this.Controls.Add(this.progressBarSave);
            this.Controls.Add(this.labelProgress);
            this.Name = "SaveWithProgressForm";
            this.Text = "Saving to Divoc...";
            this.Load += new System.EventHandler(this.SaveWithProgressFrom_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.ProgressBar progressBarSave;
    }
}