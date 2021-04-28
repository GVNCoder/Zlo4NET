using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="packets"></param>
    internal delegate void ZPacketsStreamCallback(ZPacket[] packets);
    /// <summary>
    /// 
    /// </summary>
    internal delegate void ZStreamRejectedCallback();

    /// <summary>
    /// 
    /// </summary>
    internal static class ZRouter
    {
        #region Internal types

        private class _RequestMetadata
        {
            public _RequestMetadata(ZRequest request)
            {
                Request = request;
                Response = new ZResponse(request);
                TaskCompletionSource = request.Method == ZRequestMethod.Get
                    ? new TaskCompletionSource<object>()
                    : null;
            }

            public TaskCompletionSource<object> TaskCompletionSource { get; }
            public ZRequest Request { get; }
            public ZResponse Response { get; }
            public Guid RequestGuid => Request.RequestGuid;
        }

        private class _StreamMetadata
        {
            public _StreamMetadata(ZCommand streamCommand, ZPacketsStreamCallback packetsReceivedCallback, ZStreamRejectedCallback streamRejectedCallback)
            {
                StreamCommand = streamCommand;
                OnPacketsReceivedCallback = packetsReceivedCallback;
                StreamRejectedCallback = streamRejectedCallback;
            }

            public ZCommand StreamCommand { get; }
            public ZPacketsStreamCallback OnPacketsReceivedCallback { get; }
            public ZStreamRejectedCallback StreamRejectedCallback { get; }
        }

        #endregion

        #region Constants

        private const int RQ_TIMEOUT = 3000;

        #endregion

        private static IZClient _client;
        private static IList<_RequestMetadata> _requestsPool;
        private static IList<_StreamMetadata> _streamsPool;

        #region Public Interface

        /// <summary>
        /// Occurs when connection state with ZClient changed
        /// </summary>
        public static event Action<bool> ConnectionChanged; 

        /// <summary>
        /// Initializes <see cref="ZRouter"/> to work
        /// </summary>
        public static void Initialize()
        {
            _client = new ZClientImpl();
            _requestsPool = new List<_RequestMetadata>();
            _streamsPool = new List<_StreamMetadata>();

            _client.ConnectionStateChanged += _ClientOnConnectionChangedCallback;
            _client.PacketsReceived += _ClientOnPacketsReceivedCallback;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void Start() => _client.Run();
        /// <summary>
        /// 
        /// </summary>
        public static void Stop() => _client.Close();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ZResponse> GetResponseAsync(ZRequest request)
        {
            // check incoming argument
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // check is it already exists in pool
            if (_requestsPool.Any(i => i.RequestGuid == request.RequestGuid))
            {
                throw new ArgumentException($"This request is already exists in requests pool. {request}");
            }

            // send request and get response
            var response = await _RegisterRequestAndWaitResponseAsync(request);

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="onPacketsReceivedCallback"></param>
        /// <param name="streamRejectedCallback"></param>
        /// <returns></returns>
        public static async Task<ZResponse> OpenStreamAsync(ZRequest request, ZPacketsStreamCallback onPacketsReceivedCallback, ZStreamRejectedCallback streamRejectedCallback = null)
        {
            // check incoming arguments
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (onPacketsReceivedCallback == null)
            {
                throw new ArgumentNullException(nameof(onPacketsReceivedCallback));
            }

            // check is it already exists in pool
            if (_streamsPool.Any(i => i.StreamCommand == request.RequestCommand))
            {
                throw new ArgumentException($"This stream request is already exists in streams pool. {request}");
            }

            // send request and get response
            var response = await _RegisterStreamAndWaitResponseAsync(request, onPacketsReceivedCallback, streamRejectedCallback);

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ZResponse> CloseStreamAsync(ZRequest request)
        {
            // check incoming arguments
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // check is it exists in pool
            if (_streamsPool.All(i => i.StreamCommand != request.RequestCommand))
            {
                throw new ArgumentException($"This stream request is doesn't exists in streams pool. {request}");
            }

            // send request and get response
            var response = await _TryCloseStreamAndWaitResponseAsync(request);

            return response;
        }

        #endregion

        #region Private helpers

        private static async Task<ZResponse> _TryCloseStreamAndWaitResponseAsync(ZRequest request)
        {
            // find stream metadata
            var streamMetadata = _streamsPool.First(i => i.StreamCommand == request.RequestCommand);

            // send close stream request
            var closeStreamResponse = await _RegisterRequestAndWaitResponseAsync(request);

            if (closeStreamResponse.StatusCode == ZResponseStatusCode.Ok)
            {
                _streamsPool.Remove(streamMetadata);
            }

            return closeStreamResponse;
        }
        private static async Task<ZResponse> _RegisterStreamAndWaitResponseAsync(ZRequest request, ZPacketsStreamCallback onPacketsReceivedCallback, ZStreamRejectedCallback streamRejectedCallback)
        {
            // create stream metadata to register it
            var streamMetadata = new _StreamMetadata(request.RequestCommand, onPacketsReceivedCallback, streamRejectedCallback);

            // register stream to help find it and pass packets
            _streamsPool.Add(streamMetadata);

            // send open stream request
            var openStreamResponse = await _RegisterRequestAndWaitResponseAsync(request);

            // check response status code
            if (openStreamResponse.StatusCode != ZResponseStatusCode.Ok)
            {
                _streamsPool.Remove(streamMetadata);
            }

            return openStreamResponse;
        }
        private static async Task<ZResponse> _RegisterRequestAndWaitResponseAsync(ZRequest request)
        {
            // create request metadata to register it
            var requestMetadata = new _RequestMetadata(request);

            // register request to help find it and set response
            _requestsPool.Add(requestMetadata);

            var acceptResult = _SendRequest(request);
            if (acceptResult)
            {
                await _WaitResponseAsync(request, requestMetadata);
            }
            else
            {
                requestMetadata.Response.StatusCode = ZResponseStatusCode.Declined;
            }

            // remove closed request from pool
            _requestsPool.Remove(requestMetadata);

            return requestMetadata.Response;
        }
        private static bool _SendRequest(ZRequest request)
        {
            // convert request to byte array
            var requestBytes = request.ToByteArray();

            // send request to ZClient
            return _client.SendRequest(requestBytes);
        }
        private static async Task _WaitResponseAsync(ZRequest request, _RequestMetadata metadata)
        {
            if (request.Method == ZRequestMethod.Get)
            {
                // create request timeout task
                var timeoutTask = Task.Delay(RQ_TIMEOUT);

                // wait response or timeout
                var task = await Task.WhenAny(metadata.TaskCompletionSource.Task, timeoutTask);

                // set result status code
                metadata.Response.StatusCode = task == timeoutTask ? ZResponseStatusCode.Timeout : ZResponseStatusCode.Ok;
            }
            else
            {
                metadata.Response.StatusCode = ZResponseStatusCode.Ok;
            }
        }
        private static void _OnConnectionChanged(bool connectionState) => ConnectionChanged?.Invoke(connectionState);

        #endregion

        #region Client Callbacks

        private static void _ClientOnConnectionChangedCallback(bool connectionState)
        {
            _OnConnectionChanged(connectionState);

            // reject all requests and streams
            // ReSharper disable once InvertIf
            if (! connectionState)
            {
                foreach (var requestMetadata in _requestsPool)
                {
                    requestMetadata.Response.StatusCode = ZResponseStatusCode.Rejected;
                    requestMetadata.TaskCompletionSource.SetResult(null);
                }

                _requestsPool.Clear();

                foreach (var streamMetadata in _streamsPool)
                {
                    streamMetadata.StreamRejectedCallback?.BeginInvoke(
                        ar => streamMetadata.StreamRejectedCallback.EndInvoke(ar), null);
                }

                _streamsPool.Clear();
            }
        }

        private static void _ClientOnPacketsReceivedCallback(ZPacket[] packets)
        {
            var packetGroups = packets.GroupBy(i => i.Id);

            foreach (var packetGroup in packetGroups)
            {
                var responsePackets = packetGroup.ToArray();

                // check streams first
                var streamMetadata = _streamsPool.FirstOrDefault(i => i.StreamCommand == packetGroup.Key);
                if (streamMetadata != null)
                {
                    // begin async execution
                    streamMetadata.OnPacketsReceivedCallback.BeginInvoke(responsePackets, ar => streamMetadata.OnPacketsReceivedCallback.EndInvoke(ar), null);

                    continue;
                }

                // check requests
                var requestMetadata = _requestsPool.FirstOrDefault(i => i.Request.RequestCommand == packetGroup.Key);
                if (requestMetadata != null)
                {
                    // set response
                    requestMetadata.Response.ResponsePackets = responsePackets;
                    requestMetadata.Response.StatusCode = ZResponseStatusCode.Ok;

                    // close request
                    requestMetadata.TaskCompletionSource.SetResult(null);
                }
            }
        }

        #endregion
    }
}