using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data.Parsers;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;
using Zlo4NET.Core.ZClient.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZGameProcess : IZGameProcess
    {
        private readonly IZClientService _clientService;
        private readonly IZGameRunParser _parser;
        private readonly IZProcessTracker _processTracker;
        private readonly ZInstalledGame _targetGame;
        private readonly string _runArgs;
        private readonly string _pipeName;
        private readonly ZLogger _logger;

        private _GamePipe _pipe;

        public ZGameProcess(
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
            _pipeName = pipeName;

            _processTracker = new ZProcessTracker(processName, TimeSpan.FromSeconds(1), false, processes => processes.First());
        }

        private void _PipeEventHandler(_GameState state)
        {
            _onMessage(state.Event, state.RawEvent, state.States, state.RawState);
        }

        public event EventHandler<ZGamePipeArgs> StateChanged;
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
            // send request to run the game
            var response = await _clientService.SendGameRunRequestAsync(_targetGame.RunnableName, _runArgs);
            if (response.Status != ZResponseStatusCode.Ok)
            {
                return ZRunResult.Error;
            }

            // parse run results
            var runResult = _parser.Parse(response.Packets);
            // ReSharper disable once InvertIf
            if (runResult == ZRunResult.Success)
            {
                // prepare instance state to track game state
                // begin track 
                _pipe = new _GamePipe(_logger, _pipeName);
                _pipe.PipeEvent += _PipeEventHandler;

                _processTracker.ProcessDetected += _ProcessTrackerOnProcessDetected;
                _processTracker.ProcessLost += _ProcessTrackerOnProcessLost;

                // begin track process tracker
                _processTracker.StartTrack();
            }

            return runResult;
        }

        #region Private methods

        private void _ProcessTrackerOnProcessLost(object sender, EventArgs e)
        {
            _processTracker.ProcessDetected -= _ProcessTrackerOnProcessDetected;
            _processTracker.ProcessLost -= _ProcessTrackerOnProcessLost;

            _OnCustomPipeEvent("StateChanged", "State_GameClose");

            _pipe.PipeEvent -= _PipeEventHandler;
            _pipe = null;

            // after closing the game process,
            // its pipe is destroyed automatically + the thread that processes the reading will be completed
        }

        private void _ProcessTrackerOnProcessDetected(object sender, Process e)
        {
            _OnCustomPipeEvent("StateChanged", "State_GameRun");

            // begin connect to game pipe
            _pipe.Begin();
        }

        private void _OnCustomPipeEvent(string eventName, string stateName)
        {
            // parse custom state
            var state = _GameStateParser.ParseStates(eventName, stateName);

            _onMessage(state.Event, state.RawEvent, state.States, state.RawState);
        }

        private void _onMessage(ZGameEvent eventEnum, string rawEvent, ZGameState[] stateEnums, string rawState)
        {
            if (StateChanged == null) return;

            // raise event
            var invocationList = StateChanged.GetInvocationList();
            var eventArgs = new ZGamePipeArgs(eventEnum, rawEvent, stateEnums, rawState);

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