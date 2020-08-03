using System;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.ZClient.Data
{
    internal class ZTunnel
    {
        public ZCommand Id { get; }
        public bool IsOpen { get; private set; }

        private readonly IZClient _client;
        private readonly ZRequest _openRequest;
        private readonly ZRequest _closeRequest;

        public event EventHandler<ZPacket[]> PacketsReceived;

        internal ZTunnel(IZClient client, ZRequest openRequest, ZRequest closeRequest)
        {
            _client = client;
            _openRequest = openRequest;
            _closeRequest = closeRequest;

            Id = _openRequest.Id;
        }

        internal void OnPacketsReceived(ZPacket[] packets)
        {
            if (packets == null) IsOpen = false;

            PacketsReceived?.Invoke(this, packets);

            //var invokeList = PacketsReceived?.GetInvocationList();
            //foreach (var eventHandler in invokeList)
            //{
            //    var handler = (EventHandler<ZPacket[]>) eventHandler;
            //    handler.BeginInvoke(this, packets, _EndAsyncEvent, null);
            //}
        }

        //private void _EndAsyncEvent(IAsyncResult result)
        //{
        //    var asyncResult = (AsyncResult) result;
        //    var invokedMethod = (EventHandler<ZPacket[]>) asyncResult.AsyncDelegate;

        //    try
        //    {
        //        invokedMethod.EndInvoke(result);
        //    }
        //    catch
        //    {
        //        // suppress
        //    }
        //}

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;

            _client.SendRequest(_openRequest);
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;

            _client.SendRequest(_closeRequest);
        }
    }
}