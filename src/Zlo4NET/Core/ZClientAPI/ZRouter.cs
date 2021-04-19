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
    internal static class ZRouter
    {
        #region Internal types

        private struct _RequestMetadata
        {
            public _RequestMetadata(ZRequest request)
            {
                Request = request;
                Response = new ZResponse(request);
                TaskCompletionSource = request.Method == ZRequestMethod.Get
                    ? new TaskCompletionSource<object>()
                    : null;
            }

            public TaskCompletionSource<object> TaskCompletionSource { get; set; }
            public ZRequest Request { get; set; }
            public ZResponse Response { get; set; }
            public Guid RequestGuid => Request.RequestGuid;
        }

        private struct _StreamMetadata
        {
            public ZPacketsStreamCallback OnPacketsReceivedCallback { get; set; }
        }

        #endregion

        #region Constants

        private const int RQ_TIMEOUT = 3000;

        #endregion

        private static IZClient _client;
        private static IList<_RequestMetadata> _requestsPool;

        #region Public Interface

        public static event Action<bool> ConnectionChanged; 

        /// <summary>
        /// Initializes <see cref="ZRouter"/> to work
        /// </summary>
        public static void Initialize()
        {
            _client = new ZClientImpl();
            _requestsPool = new List<_RequestMetadata>();

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

        #endregion

        #region Private helpers

        private static async Task<ZResponse> _RegisterRequestAndWaitResponseAsync(ZRequest request)
        {
            // create request metadata to register it
            var requestMetadata = new _RequestMetadata
            {
                Request = request,
                TaskCompletionSource = new TaskCompletionSource<object>()
            };

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

        private static void _ClientOnConnectionChangedCallback(bool connectionState) => _OnConnectionChanged(connectionState);

        private static void _ClientOnPacketsReceivedCallback(ZPacket[] packets)
        {
            
        }

        #endregion
    }
}