using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DivocCommon
{
    public partial class WebBrowserHost : Form
    {
        public WebBrowserHost()
        {
            InitializeComponent();
        }

        private void WebBrowserHost_Load(object sender, EventArgs e)
        {
            webBrowserCtrl.Url = new Uri("https://www.git-scm.com");
        }
    }
}
