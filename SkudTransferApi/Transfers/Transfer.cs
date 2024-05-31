using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkudTransfer.Transfers
{
    public abstract class Transfer
    {
        public abstract Task DoTransfer();
    }
}
