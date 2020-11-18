using System.Collections.Generic;

namespace Zlo4NET.Core.Data
{
    internal static class _GameStateParser
    {
        // where string is state name
        private static readonly IReadOnlyDictionary<string, ZGameState> _states = new Dictionary<string, ZGameState>
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
            { nameof(ZGameState.State_Ready), ZGameState.State_Ready }
        };
        // where string is event name
        private static readonly IReadOnlyDictionary<string, ZGameEvent> _events = new Dictionary<string, ZGameEvent>
        {
            { nameof(ZGameEvent.GameWaiting), ZGameEvent.GameWaiting },
            { nameof(ZGameEvent.StateChanged), ZGameEvent.StateChanged },
            { nameof(ZGameEvent.Alert), ZGameEvent.Alert }
        };

        public static _GameState ParseStates(string eventName, string stateName)
        {
            // TODO: Index of first number in string

            // use str.IndexOfAny("0123456789".ToCharArray())
            // use str.split by ' '

            // get pipe game event enum value
            _events.TryGetValue(eventName, out var pipeEvent);

            // select handle paths
            switch (pipeEvent)
            {
                case ZGameEvent.StateChanged:
                    break;
                case ZGameEvent.Alert:
                    break;
                case ZGameEvent.GameWaiting:
                    break;

                case ZGameEvent.Unknown:
                default:

                    // TODO: Log error here

                    break;
            }

            return null;
        }
    }
}