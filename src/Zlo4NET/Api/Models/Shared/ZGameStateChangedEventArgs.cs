using System;

using Zlo4NET.Core.Data;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InheritdocConsiderUsage
// disable Missing XML documentation
#pragma warning disable 1591

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class ZGameStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string RawEvent { get; }
        /// <summary>
        /// 
        /// </summary>
        public string RawState { get; }
        /// <summary>
        /// 
        /// </summary>
        public string RawFullMessage { get; }
        /// <summary>
        /// 
        /// </summary>
        public ZGameEvent Event { get; }
        /// <summary>
        /// 
        /// </summary>
        public ZGameState[] States { get; }

        public ZGameStateChangedEventArgs(ZGameEvent eventEnum, string rawEvent, ZGameState[] stateEnums, string rawState)
        {
            Event = eventEnum;
            States = stateEnums;

            RawEvent = rawEvent;
            RawState = rawState;

            RawFullMessage = $"{rawEvent} {rawState}";
        }
    }
}