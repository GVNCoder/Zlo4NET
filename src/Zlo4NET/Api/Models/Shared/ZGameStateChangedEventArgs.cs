using System;

using Zlo4NET.Core.Data;

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
        public string RawEvent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RawState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RawFullMessage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZGameEvent Event { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZGameState[] States { get; set; }

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