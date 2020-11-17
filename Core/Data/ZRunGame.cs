using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZRunGame : IZRunGame
    {
        private readonly IZClientService _clientService;
        private readonly IZGameRunParser _parser;
        private readonly Thread _pipeReadThread;
        private readonly NamedPipeClientStream _pipeClient;
        private readonly IZProcessTracker _processTracker;
        private readonly ZInstalledGame _targetGame;
        private readonly string _runArgs;
        private readonly ZLogger _logger;

        public ZRunGame(
            IZClientService clientService,
            string runArgs,
            ZInstalledGame targetGame,
            string pipeName,
            string processName)
        {
            _clientService = clientService;
            _parser = ZParsersFactory.BuildGameRunInfoParser();
            _runArgs = runArgs;
            _targetGame = targetGame;
            _logger = ZLogger.Instance;

            _pipeReadThread = new Thread(_ReadPipeMethod) { IsBackground = true };
            _pipeClient = new NamedPipeClientStream(".", pipeName);
            _processTracker = new ZProcessTracker(processName, TimeSpan.FromSeconds(1), false, processes => processes.First());

            _processTracker.ProcessDetected += _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost += _ProcessTrackerOnProcessLost;
        }

        public ZRunGame(string processName)
        {
            _logger = ZLogger.Instance;
            _processTracker = new ZProcessTracker(processName, TimeSpan.FromSeconds(1), false, processes => processes.First());

            _processTracker.ProcessDetected += _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost += _ProcessTrackerOnProcessLost;
        }

        public event EventHandler<ZGamePipeArgs> Pipe;
        //public event EventHandler<ZGameStateChangedEventArgs> StateChanged;
        public Process GameProcess => _processTracker.Process;
        public bool IsRun => _processTracker.IsRun;

        public bool TryClose()
        {
            if (! IsRun) return false;

            try
            {
                _processTracker.Process.Kill();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool TryUnfoldGameWindow()
        {
            if (! IsRun) return false;

            var setForegroundWnd = ZUnsafeMethods.SetForegroundWindow(_processTracker.Process.MainWindowHandle);
            var setWndShowState = ZUnsafeMethods.ShowWindow(_processTracker.Process.MainWindowHandle, 1);

            return setForegroundWnd && setWndShowState;
        }

        public async Task<ZRunResult> RunAsync()
        {
            _processTracker.StartTrack();

            var response = await _clientService.SendGameRunRequestAsync(_targetGame.RunnableName, _runArgs);
            if (response.Status != ZResponseStatusCode.Ok)
            {
                return ZRunResult.Error;
            }

            var runResult = _parser.Parse(response.Packets);
            if (runResult != ZRunResult.Success)
            {
                _processTracker.ProcessDetected -= _ProcessTrackerOnProcessDetected;
                _processTracker.ProcessLost -= _ProcessTrackerOnProcessLost;
                _processTracker.StopTrack();
            }

            return runResult;
        }

        #region Private methods

        private void _ReadPipeMethod()
        {
            _pipeClient.Connect();

            while (_pipeClient.IsConnected)
            {
                var buffer = new byte[1024];
                var bytesRead = _pipeClient.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    var readBlock = new byte[bytesRead]; // +1
                    Array.Copy(buffer, readBlock, bytesRead); // +1

                    _parseData(readBlock);
                }

                Thread.Sleep(50); // wait to data availability
            }
        }

        private void _parseData(byte[] data)
        {
            try
            {
                using (var memoryStream = new MemoryStream(data, false))
                using (var br = new BinaryReader(memoryStream, Encoding.ASCII))
                {
                    br.ReadBytes(2); // skip 2 bytes ?
                    br.ReadUInt16(); // message length - 4 bytes (2 skipped and 2 current message length)

                    var firstPartLength = br.ReadByte();
                    var firstPartString = br.ReadCountedString(firstPartLength)
                        .Trim();

                    var secondPartLength = br.ReadByte() + 1;
                    var secondPartString = br.ReadCountedString(secondPartLength)
                        .Trim()
                        .Replace('\0'.ToString(), string.Empty);
                    secondPartString = Uri.UnescapeDataString(secondPartString);

                    _onMessage(firstPartString, secondPartString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(_parseData)} -> Occured {ex.Message}");
            }
        }

        private void _ProcessTrackerOnProcessLost(object sender, EventArgs e)
        {
            _processTracker.ProcessDetected -= _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost -= _ProcessTrackerOnProcessLost;

            //_onMessage("StateChanged", "State_GameClosed");
        }

        private void _ProcessTrackerOnProcessDetected(object sender, Process e)
        {
            //_onMessage("StateChanged", "State_GameRunning");

            // ? cuz we cannot always create an instance
            _pipeReadThread?.Start();
        }

        private void _onMessage(string firstPart, string secondPart)
        {
            if (Pipe == null) return;

            // prepare event args
            var x = _GameStateParser.ParseStates(firstPart, secondPart);

            // raise event
            var invocationList = Pipe.GetInvocationList();
            //var eventArgs = new ZGameStateChangedEventArgs(state, caller, firstPart, secondPart);
            var eventArgs = new ZGamePipeArgs(firstPart, secondPart);

            foreach (var handler in invocationList)
            {
                var eventHandler = (EventHandler<ZGamePipeArgs>) handler;
                eventHandler.BeginInvoke(this, eventArgs, _EndAsyncEvent, null);
            }
        }

        private void _EndAsyncEvent(IAsyncResult iar)
        {
            var ar = (AsyncResult) iar;
            var invokedMethod = (EventHandler<ZGamePipeArgs>) ar.AsyncDelegate;

            try
            {
                invokedMethod.EndInvoke(iar);
            }
            catch (Exception ex)
            {
                _logger.Error($"Pipe event handler throws exception. MSG: {ex.Message}");
                throw new Exception("Pipe event handler throws exception.", ex);
            }
        }

        #endregion
    }
}