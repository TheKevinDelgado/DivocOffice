using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DivocCommon
{
    public delegate void ViewWrapperClosedDelegate(Guid id);

    public abstract class ViewWrapperBase
    {
        public event ViewWrapperClosedDelegate Closed;

        public Guid Id { get; private set; }

        protected void OnClosed()
        {
            Closed?.Invoke(Id);
        }

        public ViewWrapperBase()
        {
            Id = Guid.NewGuid();
        }
    }
}
