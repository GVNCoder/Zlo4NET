using System;

using Zlo4NET.Api.DTOs;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Zlo4NET.Api.Shared
{
    /// <inheritdoc />
    /// <summary>
    /// Represents <see cref="E:Zlo4NET.Api.Service.IZConnection.ConnectionChanged" /> event args
    /// </summary>
    public class ZConnectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets current connection state with ZClient
        /// </summary>
        public bool IsConnected { get; }
        /// <summary>
        /// Gets current authorized in ZClient user
        /// </summary>
        public ZUser AuthorizedUser { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates an instance of <see cref="T:Zlo4NET.Api.Shared.ZConnectionChangedEventArgs" />
        /// </summary>
        public ZConnectionChangedEventArgs(bool isConnected, ZUser authorizedUserDto)
        {
            IsConnected = isConnected;
            AuthorizedUser = authorizedUserDto;
        }
    }
}