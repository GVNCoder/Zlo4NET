using System;
using System.Diagnostics;

namespace Zlo4NET.Api.Service
{
    /// <summary>
    /// Defines process tracker
    /// </summary>
    public interface IZProcessTracker
    {
        /// <summary>
        /// Occurs when a detected process has been lost.
        /// </summary>
        event EventHandler ProcessLost;
        /// <summary>
        /// Occurs when a given process has been detected.
        /// </summary>
        event EventHandler<Process> ProcessDetected;
        /// <summary>
        /// Gets detected process
        /// </summary>
        Process Process { get; }
        /// <summary>
        /// Gets a value indicating the current state of the process, running or not.
        /// </summary>
        bool IsRun { get; }
        /// <summary>
        /// Gets the given process name
        /// </summary>
        string TargetProcessName { get; }
        /// <summary>
        /// Starts tracking the process list.
        /// </summary>
        void StartTrack();
        /// <summary>
        /// Stop tracking process list.
        /// </summary>
        void StopTrack();
    }
}