using System;

using Zlo4NET.Api.DTO;
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
        public ZUserDTO AuthorizedUser { get; }
        /// <summary>
        /// Creates an instance of <see cref="ZAuthorizedEventArgs"/>
        /// </summary>
        public ZAuthorizedEventArgs(ZUserDTO user)
        {
            AuthorizedUser = user;
        }
    }
}