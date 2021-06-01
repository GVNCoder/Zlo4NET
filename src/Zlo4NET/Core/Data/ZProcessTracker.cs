using System;
using System.Timers;
using System.Diagnostics;

using Zlo4NET.Api.Service;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Data
{
    /// <inheritdoc />
    public class ZProcessTracker : IZProcessTracker
    {
        #region Constant

        private const int MIN_INTERVAL = 1000;

        #endregion

        private readonly Timer _timer;
        private readonly Func<Process[], Process> _processDetectorFunc;
        private readonly bool _trackAfterLost;

        #region Private helpers

        private void _OnProcessDetected(Process detectedProcess)
        {
            ProcessDetected?.Invoke(this, detectedProcess);
        }

        private void _OnProcessLost()
        {
            ProcessLost?.Invoke(this, EventArgs.Empty);
        }

        private void _processLostHandler(object sender, EventArgs e)
        {
            StopTrack();

            // raise event
            _OnProcessLost();

            if (_trackAfterLost)
            {
                StartTrack();
            }
        }

        private void _checkHandler(object sender, ElapsedEventArgs e)
        {
            // get process list
            var processes = Process.GetProcessesByName(TargetProcessName);

            // if we get empty process list => return
            if (processes.Length == 0) return;

            var targetProcess = _processDetectorFunc.Invoke(processes);

            if (targetProcess == null || targetProcess.HasExited) return;

            Process = targetProcess;

            _timer.Stop();

            targetProcess.EnableRaisingEvents = true;
            targetProcess.Exited += _processLostHandler;
            
            _OnProcessDetected(targetProcess);
        }

        #endregion

        #region Ctor

        /// <param name="targetProcessName">The target process name.</param>
        /// <param name="checkInterval">The check interval.</param>
        /// <param name="trackAfterLost">The flag who indicates, need to track after process lost.</param>
        /// <param name="processDetectorFunc">The needed process detector func.</param>
        public ZProcessTracker(string targetProcessName, TimeSpan checkInterval, bool trackAfterLost, Func<Process[], Process> processDetectorFunc)
        {
            // validate parameters
            if (checkInterval < TimeSpan.FromMilliseconds(MIN_INTERVAL))
            {
                throw new ArgumentException($"The minimum interval is {MIN_INTERVAL} ms", nameof(checkInterval));
            }

            _trackAfterLost = trackAfterLost;
            _timer = new Timer { Enabled = false, Interval = checkInterval.TotalMilliseconds };
            _timer.Elapsed += _checkHandler;
            _processDetectorFunc = processDetectorFunc;

            TargetProcessName = targetProcessName;
        }

        #endregion

        #region IZProcessTracker interface

        /// <inheritdoc />
        public event EventHandler ProcessLost;

        /// <inheritdoc />
        public event EventHandler<Process> ProcessDetected;

        /// <inheritdoc />
        public Process Process { get; private set; }

        /// <inheritdoc />
        public bool IsRun => Process != null;

        /// <inheritdoc />
        public string TargetProcessName { get; }

        /// <inheritdoc />
        public void StartTrack()
        {
            if (_timer.Enabled)
            {
                return;
            }

            _checkHandler(this, null);
            _timer.Start();
        }

        /// <inheritdoc />
        public void StopTrack()
        {
            _timer.Stop();

            if (Process != null)
            {
                Process.Exited -= _processLostHandler;
                Process.EnableRaisingEvents = false;
            }

            Process = null;
        }

        #endregion
    }
}