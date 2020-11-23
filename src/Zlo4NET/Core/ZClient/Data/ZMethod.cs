namespace Zlo4NET.Core.ZClient.Data
{
    /// <summary>
    /// Defines request method
    /// </summary>
    internal enum ZMethod : byte
    {
        /// <summary>
        /// Puts some data and wait response data
        /// </summary>
        Get,
        /// <summary>
        /// Puts some data and no wait response
        /// </summary>
        Put,
        /// <summary>
        /// Puts some data and wait data in tunnel object
        /// </summary>
        Tunnel
    }
}