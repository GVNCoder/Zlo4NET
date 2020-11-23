using System.Linq;
using System.Threading.Tasks;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.ZClient.Data
{
    /// <summary>
    /// Defines ZClient request
    /// </summary>
    internal class ZRequest
    {
        public ZCommand Id { get; set; } // request id
        public byte[] Payload { get; set; } = { }; // request payload
        public ZMethod Method { get; set; } = ZMethod.Get; // request send method

        private readonly IZClient _client;
        private TaskCompletionSource<ZResponse> _taskCompletionSource;

        internal ZRequest(IZClient client)
        {
            _client = client;
        }

        internal void SetResponse(ZResponse response)
        {
            _taskCompletionSource?.SetResult(response);
        }

        public byte[] GetBytes()
        {
            var request = new[] { (byte) Id } // packet id
                .Concat(ZBitConverter.Convert(Payload.Length)) // packet length
                .Concat(Payload) // packet content
                .ToArray();
            return request;
        }

        public async Task<ZResponse> GetResponseAsync()
        {
            _client.SendRequest(this);

            if (Method != ZMethod.Get) return null;

            _taskCompletionSource = new TaskCompletionSource<ZResponse>();

            var response = await _taskCompletionSource.Task;
            return response;
        }
    }
}