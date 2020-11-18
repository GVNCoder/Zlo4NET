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
        private const int _messageHeaderSize = 4;

        private readonly IZClientService _clientService;
        private readonly IZGameRunParser _parser;
        private readonly Thread _pipeReadThread;
        private readonly NamedPipeClientStream _pipeClient;
        private readonly IZProcessTracker _processTracker;
        private readonly ZInstalledGame _targetGame;
        private readonly string _runArgs;
        private readonly ZLogger _logger;

        private readonly _Buffer _dynamicBuffer;

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
            _pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.In);
            _processTracker = new ZProcessTracker(processName, TimeSpan.FromSeconds(1), false, processes => processes.First());

            _processTracker.ProcessDetected += _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost += _ProcessTrackerOnProcessLost;

            _dynamicBuffer = new _Buffer();
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

            // read data from pipe
            while (_pipeClient.IsConnected && _pipeClient.CanRead)
            {
                var buffer = new byte[4096];
                var bytesRead = _pipeClient.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    // create message data block
                    var messageData = buffer.Take(bytesRead);

                    // append message data
                    _dynamicBuffer.Append(messageData);

                    // parse message
                    _parseData();
                }

                Thread.Sleep(50); // wait to data availability
            }
        }

        private void _parseData()
        {
            var eventString = string.Empty;
            var stateString = string.Empty;

            try
            {
                using (var memoryStream = new MemoryStream(_dynamicBuffer.BufferData, false))
                using (var br = new BinaryReader(memoryStream, Encoding.ASCII))
                {
                    br.ReadBytes(2); // skip two unknown bytes Approved by ZLOFENIX
                    
                    // read message length
                    var messageLength = br.ReadUInt16();

                    // check, we got the full message or not
                    if (_dynamicBuffer.Size < messageLength - _messageHeaderSize)
                    {
                        return;
                    }

                    // read and parse message data
                    var eventStringLength = br.ReadByte();

                    eventString = br.ReadCountedString(eventStringLength)
                        .Trim();

                    var stateStringLength = br.ReadUInt16();

                    stateString = br.ReadCountedString(stateStringLength)
                        .Trim();

                    //    .Replace('\0'.ToString(), string.Empty);
                    //secondPartString = Uri.UnescapeDataString(secondPartString);

                    // normalize state string
                    stateString = Uri.UnescapeDataString(stateString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{nameof(_parseData)} message {ex.Message}");
            }

            _dynamicBuffer.Clear();
            _onMessage(eventString, stateString);
        }

        private void _ProcessTrackerOnProcessLost(object sender, EventArgs e)
        {
            _processTracker.ProcessDetected -= _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost -= _ProcessTrackerOnProcessLost;

            _onMessage("StateChanged", "State_GameClose");
        }

        private void _ProcessTrackerOnProcessDetected(object sender, Process e)
        {
            _onMessage("StateChanged", "State_GameRun");

            // ? cuz we cannot always create an instance
            _pipeReadThread?.Start();
        }

        private void _onMessage(string firstPart, string secondPart)
        {
            if (Pipe == null) return;

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