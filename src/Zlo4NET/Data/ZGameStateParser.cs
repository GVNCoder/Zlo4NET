using System;
using System.Linq;
using System.Collections.Generic;
using Zlo4NET.Extensions;
using Zlo4NET.Helpers;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Data
{
    internal static class ZGameStateParser
    {
        // where string is state name
        private static readonly IReadOnlyDictionary<string, ZGameState> _statesCollection = new Dictionary<string, ZGameState>
        {
            { "", ZGameState.State_Empty },
            { nameof(ZGameState.State_GameRun), ZGameState.State_GameRun },
            { nameof(ZGameState.State_GameClose), ZGameState.State_GameClose },

            { nameof(ZGameState.State_NA), ZGameState.State_NA },
            { nameof(ZGameState.State_Error), ZGameState.State_Error },
            { nameof(ZGameState.State_Starting), ZGameState.State_Starting },
            { nameof(ZGameState.State_Init), ZGameState.State_Init },
            { nameof(ZGameState.State_NotLoggedIn), ZGameState.State_NotLoggedIn },
            { nameof(ZGameState.State_MenuReady), ZGameState.State_MenuReady },
            { nameof(ZGameState.State_Matchmaking), ZGameState.State_Matchmaking },
            { nameof(ZGameState.State_MatchmakeResultHost), ZGameState.State_MatchmakeResultHost },
            { nameof(ZGameState.State_MatchmakeResultJoin), ZGameState.State_MatchmakeResultJoin },
            { nameof(ZGameState.State_ConnectToGameId), ZGameState.State_ConnectToGameId },
            { nameof(ZGameState.State_ConnectToUserId), ZGameState.State_ConnectToUserId },
            { nameof(ZGameState.State_CreateCoOpPeer), ZGameState.State_CreateCoOpPeer },
            { nameof(ZGameState.State_MatchmakeCoOp), ZGameState.State_MatchmakeCoOp },
            { nameof(ZGameState.State_ResumeCampaign), ZGameState.State_ResumeCampaign },
            { nameof(ZGameState.State_LaunchPlayground), ZGameState.State_LaunchPlayground },
            { nameof(ZGameState.State_WeaponCustomization), ZGameState.State_WeaponCustomization },
            { nameof(ZGameState.State_LoadLevel), ZGameState.State_LoadLevel },
            { nameof(ZGameState.State_Connecting), ZGameState.State_Connecting },
            { nameof(ZGameState.State_WaitForLevel), ZGameState.State_WaitForLevel },
            { nameof(ZGameState.State_GameLoading), ZGameState.State_GameLoading },
            { nameof(ZGameState.State_Game), ZGameState.State_Game },
            { nameof(ZGameState.State_GameLeaving), ZGameState.State_GameLeaving },
            { nameof(ZGameState.State_InQueue), ZGameState.State_InQueue },
            { nameof(ZGameState.State_WaitForPeerClient), ZGameState.State_WaitForPeerClient },
            { nameof(ZGameState.State_PeerClientConnected), ZGameState.State_PeerClientConnected },
            { nameof(ZGameState.State_ClaimReservation), ZGameState.State_ClaimReservation },
            { nameof(ZGameState.State_Ready), ZGameState.State_Ready },
            { nameof(ZGameState.State_Sparta), ZGameState.State_Sparta },
            { nameof(ZGameState.State_ShutdownStateMachine), ZGameState.State_ShutdownStateMachine }
        };

        // where string is event name
        private static readonly IReadOnlyDictionary<string, ZGameEvent> _eventsCollection = new Dictionary<string, ZGameEvent>
        {
            { nameof(ZGameEvent.GameWaiting), ZGameEvent.GameWaiting },
            { nameof(ZGameEvent.StateChanged), ZGameEvent.StateChanged },
            { nameof(ZGameEvent.Alert), ZGameEvent.Alert }
        };

        private static readonly ZLoggerImpl _log = ZLoggerImpl.Instance;

        #region Interface

        public static ZGameStateModel ParseStates(string rawEvent, string rawState)
        {
            // get pipe game event enum value
            _eventsCollection.TryGetValue(rawEvent, out var pipeEvent);

            var resultState = new ZGameStateModel
            {
                RawEvent = rawEvent,
                RawState = rawState,
                Event = pipeEvent,
                States = Array.Empty<ZGameState>()
            };

            // select handle paths
            switch (pipeEvent)
            {
                case ZGameEvent.StateChanged:

                    var endOfStatesIndex = rawState.IndexOfAny("0123456789".ToCharArray());

                    // Some states may not have numbers, so we need to handle this situation
                    endOfStatesIndex = endOfStatesIndex != -1 ? endOfStatesIndex : rawState.Length;

                    var stateString = rawState.Substring(0, endOfStatesIndex);
                    var splitStates = stateString.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    var states = splitStates.Select(state =>
                    {
                        // get game state enum value
                        _statesCollection.TryGetValue(state, out var stateEnum);

                        return stateEnum;
                    }).ToArray();

                    resultState.States = states;

                    break;
                case ZGameEvent.Alert:
                    break;
                case ZGameEvent.GameWaiting:
                    break;

                case ZGameEvent.Unknown:
                default:

                    _log.Warning($"{nameof(ZGameStateParser)} event doesn't match ({rawEvent} {rawState})");

                    break;
            }

            return resultState;
        }

        #endregion
    }
}