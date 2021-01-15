using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DivocCommon.DataModel;

namespace DivocCommon
{
    /// <summary>
    /// UI shell for the WPF DriveBrowserControl.
    /// </summary>
    /// <notes>
    /// Since it is easier to databind in WPF, the work will be done in the user control
    /// and the user control will be hosted on this form. The ContentManager stuff should
    /// probably get refactored into a singleton rather than having to get passed around.
    /// Maybe also consider creating a separate WPF project with a proper WPF window
    /// instead of doing a user control. Localization can still be pulled from the common
    /// dll for the UI, but it would factor out this control hosting layer and make it 
    /// cleaner.
    /// </notes>
    public partial class BrowseHostForm : Form
    {
        public string ItemUrl { get; private set; }
        public string ItemId { get; private set; }
        public string ParentId { get; private set; }

        private ContentManager _contentMgr = null;
        private List<string> _fileTypes = null;
        private bool _location = false;
        DriveBrowserControl browser = null;

        public BrowseHostForm(ContentManager contentManager, List<string> fileTypes = null, bool location = false)
        {
            InitializeComponent();
            _contentMgr = contentManager;
            _fileTypes = fileTypes;
            _location = location;
            browser = elementHostDriveBrowser.Child as DriveBrowserControl;
        }

        private void OpenForm_Load(object sender, EventArgs e)
        {
            browser.DriveItemSelected += Browser_DriveItemSelected;
            browser.BrowseCanceled += Browser_BrowseCanceled;
            browser.Init(_contentMgr, _fileTypes, _location);
        }

        private void Browser_BrowseCanceled(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Browser_DriveItemSelected(object sender, DriveItem item)
        {
            if(item != null)
            {
                if (_location)
                    ItemId = item.id;
                else
                    ItemUrl = item.webDavUrl;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
