using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivocCommon
{
    public partial class SaveWithProgressForm : Form
    {
        public List<(string name, string webDavUrl)> WebDavUrls { get; private set; }

        private readonly ContentManager _contentMgr;
        private readonly List<KeyValuePair<string, string>> _fileInfoList;
        private readonly string _parentId = string.Empty;
        private readonly dynamic progressHandler;

        public SaveWithProgressForm(ContentManager contentManager, List<KeyValuePair<string, string>> fileInfoList, string parentId = "")
        {
            InitializeComponent();

            _contentMgr = contentManager;
            _fileInfoList = fileInfoList;
            _parentId = parentId;
            progressHandler = new Progress<KeyValuePair<int, string>>(HandleAttachmentProcessProgress);

            StartPosition = FormStartPosition.CenterParent;

            progressBarSave.Maximum = fileInfoList.Count;
            progressBarSave.Step = 1;
            progressBarSave.Value = 0;

            labelProgress.Text = string.Empty;
        }

        private async void SaveWithProgressFrom_Load(object sender, EventArgs e)
        {
            WebDavUrls = await _contentMgr.SaveDocuments(_fileInfoList, _parentId, progressHandler);
        }

        void HandleAttachmentProcessProgress(KeyValuePair<int, string> data)
        {
            int val = data.Key;
            string name = data.Value;

            LogManager.LogInformation(string.Format("Attachment save progress: {0}\t{1}", val, name));

            if(val == -1)
            {
                // This is just anal retentiveness. It allows for the UI to show the final step of progress
                // before the dialog closes.
                Task.Run(() =>
                {
                    Thread.Sleep(500);
                    DialogResult = DialogResult.OK;
                });
            }
            else
            {
                progressBarSave.Value = val;

                if (!String.IsNullOrEmpty(name))
                    labelProgress.Text = name;
            }
        }
    }
}
