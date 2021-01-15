using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FileTransfer.Definitions
{
    public class CancellationTokenWithReason : CancellationTokenSource
    {
        public string CancellationReason { get; private set; } = "";

        public void Cancel(string reason)
        {
            this.CancellationReason = reason;

            base.Cancel();
        }
    }
}
