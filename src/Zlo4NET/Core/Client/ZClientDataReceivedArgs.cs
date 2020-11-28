using System;

namespace Zlo4NET.Core.Client
{
    internal class ZClientDataReceivedArgs : EventArgs
    {
        public byte[] ResponseData { get; }

        public ZClientDataReceivedArgs(byte[] responseData)
        {
            ResponseData = responseData;
        }
    }
}