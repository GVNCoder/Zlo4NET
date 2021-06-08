using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;
using Zlo4NET.Api.Shared;
using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.Data
{
    internal class ZGameProcess : IZGameProcess
    {
        private readonly IZGameRunParser _parser;
        private readonly IZProcessTracker _processTracker;
        private readonly ZInstalledGame _targetGame;
        private readonly string _runArgs;
        private readonly string _pipeName;

        private ZGamePipe _pipe;

        #region Ctor

        public ZGameProcess(
            ZInstalledGame targetGame,
            string runArgs,
            string pipeName,
            string processName)
        {
            _parser = ZParsersFactory.CreateGameRunInfoParser();
            _runArgs = runArgs;
            _targetGame = targetGame;
            _pipeName = pipeName;

            _processTracker = new ZProcessTracker(processName, TimeSpan.FromSeconds(1), false, processes => processes.First());
        }

        #endregion

        #region IZGameProcess interface

        public event EventHandler<ZGameStateChangedEventArgs> StateChanged;
        public Process Process => _processTracker.Process;

        public bool TryClose()
        {
            // nothing to close
            if (Process == null)
            {
                return false;
            }

            // try to close
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

        public async Task<ZRunResult> RunAsync()
        {
            ZConnectionHelper.ThrowIfNotConnected();

            // send request to run the game
            var request = ZRequestFactory.CreateRunGameRequest(_targetGame.RunnableName, _runArgs);
            var response = await ZRouter.GetResponseAsync(request);

            if (response.StatusCode != ZResponseStatusCode.Ok)
            {
                return ZRunResult.ApiInternalError;
            }

            // parse run results
            var responsePacket = response.ResponsePackets.Single();
            var runResult = _parser.Parse(responsePacket);

            // ReSharper disable once InvertIf
            if (runResult == ZRunResult.Success)
            {
                // prepare instance state to track game state
                _pipe = new ZGamePipe(_pipeName);
                _pipe.PipeEvent += _OnGameStateChanged;

                _processTracker.ProcessDetected += _OnGameProcessDetected;
                _processTracker.ProcessLost += _OnGameProcessLost;

                // begin track game process
                _processTracker.StartTrack();
            }

            return runResult;
        }

        #endregion

        #region Private methods

        private void _OnGameStateChanged(ZGameStateModel state)
        {
            _OnGameStateChangedMessage(state.Event, state.RawEvent, state.States, state.RawState);
        }

        private void _OnGameProcessLost(object sender, EventArgs e)
        {
            _processTracker.StopTrack();

            _processTracker.ProcessDetected -= _OnGameProcessDetected;
            _processTracker.ProcessLost -= _OnGameProcessLost;

            // raise custom game state changed event
            _OnCustomPipeEvent("StateChanged", "State_GameClose");

            _pipe.PipeEvent -= _OnGameStateChanged;
            _pipe = null;

            // after closing the game process,
            // its pipe is destroyed automatically + the thread that processes the reading will be completed
        }

        private void _OnGameProcessDetected(object sender, Process e)
        {
            // raise custom game state changed event
            _OnCustomPipeEvent("StateChanged", "State_GameRun");

            // begin connect to game pipe
            _pipe.Begin();
        }

        private void _OnCustomPipeEvent(string eventName, string stateName)
        {
            // parse custom state
            var state = ZGameStateParser.ParseStates(eventName, stateName);

            _OnGameStateChangedMessage(state.Event, state.RawEvent, state.States, state.RawState);
        }

        private void _OnGameStateChangedMessage(ZGameEvent eventEnum, string rawEvent, ZGameState[] stateEnums, string rawState)
        {
            if (StateChanged == null)
            {
                return;
            }

            // raise event
            var invocationList = StateChanged.GetInvocationList();
            var eventArgs = new ZGameStateChangedEventArgs(eventEnum, rawEvent, stateEnums, rawState);

            foreach (var handler in invocationList)
            {
                var eventHandler = (EventHandler<ZGameStateChangedEventArgs>) handler;
                eventHandler.BeginInvoke(this, eventArgs, _EndAsyncEvent, null);
            }
        }

        private static void _EndAsyncEvent(IAsyncResult iar)
        {
            var asyncResult = (AsyncResult) iar;
            var invokedMethod = (EventHandler<ZGameStateChangedEventArgs>) asyncResult.AsyncDelegate;

            invokedMethod.EndInvoke(iar);
        }

        #endregion
    }
}