﻿using System;
using System.Threading.Tasks;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Represents a container for a method that processes a list of servers
    /// </summary>
    /// <param name="action">Event that happened to the server list</param>
    /// <param name="serverId">Server ID</param>
    /// <param name="server">Server model</param>
    public delegate void ZServerListActionCallback(ZServerListAction action, uint serverId, ZServerBase server);

    /// <summary>
    /// Represents a service that allows you to get a list of servers
    /// </summary>
    public interface IZServersList
    {
        /// <summary>
        /// Gets True if the current instance can be used and the <see cref="IZServersList.StopReceivingAsync"/> method has not been called yet, and False otherwise.
        /// </summary>
        bool IsInstanceAvailable { get; }
        /// <summary>
        /// Makes a request to receive data about servers and starts processing them
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when object is disposed after call <see cref="IZServersList.StopReceivingAsync"/> method</exception>
        Task StartReceivingAsync();
        /// <summary>
        /// Releases all resources used by this instance. After calling this method, this instance can no longer be used
        /// </summary>
        /// <exception cref="InvalidOperationException">Occurs when object is disposed after call <see cref="IZServersList.StopReceivingAsync"/> method</exception>
        Task StopReceivingAsync();
        /// <summary>
        /// This callback will be called on every new server list event
        /// </summary>
        ZServerListActionCallback ServerListActionCallback { get; set; }
    }
}