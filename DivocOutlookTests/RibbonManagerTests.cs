using Microsoft.VisualStudio.TestTools.UnitTesting;
using DivocOutlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace DivocOutlook.Tests
{
    [TestClass()]
    public class RibbonManagerTests
    {
        [TestMethod()]
        public void OnGetLabelTest()
        {
            var app = new Outlook.Application();

            if(app != null)
            {
                var addins = app.COMAddIns;

                if(addins != null)
                {
                    var olAddin = addins.Item("DivocOutlook");

                    if(olAddin != null)
                    {
                        System.Diagnostics.Debug.WriteLine("TODO: figure out how to get ribbon");

                    }
                }
            }
        }

        [TestMethod()]
        public void OnGetImageTest()
        {
        }
    }
}