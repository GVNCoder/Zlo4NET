using System;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Represents <see cref="IZConnection.Authorized"/> event args
    /// </summary>
    public class ZAuthorizedEventArgs : EventArgs
    {
        /// <summary>
        /// Authorized in ZClient user
        /// </summary>
        public ZUser AuthorizedUser { get; }
        /// <summary>
        /// Creates an instance of <see cref="ZAuthorizedEventArgs"/>
        /// </summary>
        public ZAuthorizedEventArgs(ZUser user)
        {
            AuthorizedUser = user;
        }
    }
}