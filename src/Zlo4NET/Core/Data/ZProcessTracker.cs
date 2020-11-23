using System;
using System.Diagnostics;
using System.Timers;
using Zlo4NET.Api.Service;

namespace Zlo4NET.Core.Data
{
    /// <inheritdoc />
    public class ZProcessTracker : IZProcessTracker
    {
        private readonly Timer _checkLoopRepeater;
        private readonly Func<Process[], Process> _processDetectorFunc;
        private readonly bool _trackAfterLost;

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

            _OnProcessLost();

            if (_trackAfterLost)
            {
                StartTrack();
            }
        }

        private void _checkHandler(object sender, ElapsedEventArgs e)
        {
            // multi-call protection
            if (IsRun)
            {
                _checkLoopRepeater.Stop();
                return;
            }

            // get process list
            var processes = Process.GetProcessesByName(TargetProcessName);

            // if we get empty process list => return
            if (processes.Length == 0) return;

            var targetProcess = _processDetectorFunc.Invoke(processes);

            if (targetProcess == null || targetProcess.HasExited) return;

            Process = targetProcess;

            _checkLoopRepeater.Stop();

            targetProcess.EnableRaisingEvents = true;
            targetProcess.Exited += _processLostHandler;
            
            _OnProcessDetected(targetProcess);
        }

        /// <param name="targetProcessName">The target process name.</param>
        /// <param name="checkInterval">The check interval.</param>
        /// <param name="trackAfterLost">The flag who indicates, need to track after process lost.</param>
        /// <param name="processDetectorFunc">The needed process detector func.</param>
        public ZProcessTracker(string targetProcessName, TimeSpan checkInterval, bool trackAfterLost, Func<Process[], Process> processDetectorFunc)
        {
            _trackAfterLost = trackAfterLost;
            _checkLoopRepeater = new Timer { Enabled = false, Interval = checkInterval.TotalMilliseconds };
            _checkLoopRepeater.Elapsed += _checkHandler;
            _processDetectorFunc = processDetectorFunc;

            TargetProcessName = targetProcessName;
        }

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
            _checkHandler(this, null);
            _checkLoopRepeater.Start();
        }

        /// <inheritdoc />
        public void StopTrack()
        {
            _checkLoopRepeater.Stop();

            if (Process != null)
            {
                Process.Exited -= _processLostHandler;
                Process.EnableRaisingEvents = false;
            }

            Process = null;
        }
    }
}