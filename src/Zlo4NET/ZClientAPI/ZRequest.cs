using System;
using System.Linq;

using Zlo4NET.Helpers;

namespace Zlo4NET.ZClientAPI
{
    /// <summary>
    /// Represents the unit of request to ZClient
    /// </summary>
    internal class ZRequest
    {
        public ZRequest()
        {
            RequestGuid = Guid.NewGuid();
        }

        /// <summary>
        /// Gets request guid
        /// </summary>
        public Guid RequestGuid { get; }
        /// <summary>
        /// Gets or sets method of request. By default it's <see cref="ZRequestMethod.Get"/>
        /// </summary>
        public ZRequestMethod Method { get; set; } = ZRequestMethod.Get;
        /// <summary>
        /// Gets or sets request command
        /// </summary>
        public ZCommand RequestCommand { get; set; }
        /// <summary>
        /// Gets or sets request payload. By default it's Empty
        /// </summary>
        public byte[] RequestPayload { private get; set; } = { };
        /// <summary>
        /// Converts this instance to byte array representation
        /// </summary>
        /// <returns>Request in byte array representation</returns>
        public byte[] ToByteArray()
        {
            var requestBytes = new[] { (byte) RequestCommand }
                .Concat(ZBitConverter.Convert(RequestPayload.Length))
                .Concat(RequestPayload)
                .ToArray();

            return requestBytes;
        }

        #region Overrides

        public override string ToString()
        {
            return $"Request {RequestGuid} - Method {Method} Command {RequestCommand} Payload {BitConverter.ToString(RequestPayload)}";
        }

        #endregion
    }
}