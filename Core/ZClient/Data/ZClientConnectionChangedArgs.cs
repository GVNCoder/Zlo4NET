using System;

namespace Zlo4NET.Core.ZClient.Data
{
    internal class ZClientConnectionChangedArgs : EventArgs
    {
        public bool ConnectionState { get; }

        public ZClientConnectionChangedArgs(bool connectionState)
        {
            ConnectionState = connectionState;
        }
    }
}