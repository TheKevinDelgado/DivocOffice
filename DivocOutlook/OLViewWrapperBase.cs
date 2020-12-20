using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocOutlook
{
    public delegate void WindowWrapperClosedDelegate(Guid id);

    abstract class OLViewWrapperBase
    {
        public event WindowWrapperClosedDelegate Closed;

        public Guid Id { get; private set; }

        protected void OnClosed()
        {
            if (Closed != null) Closed(Id);
        }

        public OLViewWrapperBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
