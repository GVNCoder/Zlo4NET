using System;
using System.Collections.Generic;

namespace Zlo4NET.Core.Data
{
    internal static class _GameStateParser
    {
        private const char __StatesSeparator = ' ';

        // where string is state name
        private static readonly IReadOnlyDictionary<string, ZGameState> _states = new Dictionary<string, ZGameState>
        {
            { "", ZGameState.State_Empty },
            { "State_GameRunning", ZGameState.State_GameRun },
            { "State_GameClosed", ZGameState.State_GameClose },

            { "State_NA", ZGameState.State_NA },
            { "State_Error", ZGameState.State_Error },
            { "State_Starting", ZGameState.State_Starting },
            { "State_Init", ZGameState.State_Init },
            { "State_NotLoggedIn", ZGameState.State_NotLoggedIn },
            { "State_MenuReady", ZGameState.State_MenuReady },
            { "State_Matchmaking", ZGameState.State_Matchmaking },
            { "State_MatchmakeResultHost", ZGameState.State_MatchmakeResultHost },
            { "State_MatchmakeResultJoin", ZGameState.State_MatchmakeResultJoin },
            { "State_ConnectToGameId", ZGameState.State_ConnectToGameId },
            { "State_ConnectToUserId", ZGameState.State_ConnectToUserId },
            { "State_CreateCoOpPeer", ZGameState.State_CreateCoOpPeer },
            { "State_MatchmakeCoOp", ZGameState.State_MatchmakeCoOp },
            { "State_ResumeCampaign", ZGameState.State_ResumeCampaign },
            { "State_LaunchPlayground", ZGameState.State_LaunchPlayground },
            { "State_WeaponCustomization", ZGameState.State_WeaponCustomization },
            { "State_LoadLevel", ZGameState.State_LoadLevel },
            { "State_Connecting", ZGameState.State_Connecting },
            { "State_WaitForLevel", ZGameState.State_WaitForLevel },
            { "State_GameLoading", ZGameState.State_GameLoading },
            { "State_Game", ZGameState.State_Game },
            { "State_GameLeaving", ZGameState.State_GameLeaving },
            { "State_InQueue", ZGameState.State_InQueue },
            { "State_WaitForPeerClient", ZGameState.State_WaitForPeerClient },
            { "State_PeerClientConnected", ZGameState.State_PeerClientConnected },
            { "State_ClaimReservation", ZGameState.State_ClaimReservation },
            { "State_Ready", ZGameState.State_Ready }
        };

        private static readonly IReadOnlyDictionary<string, ZGameEvent> _events = new Dictionary<string, ZGameEvent>
        {
            { nameof(ZGameEvent.GameWaiting), ZGameEvent.GameWaiting },
            { nameof(ZGameEvent.StateChanged), ZGameEvent.StateChanged },
            { nameof(ZGameEvent.Alert), ZGameEvent.Alert }
        };

        public static ZGameState[] ParseStates(string eventName, string stateName)
        {
            // TODO: Index of first number in string
            // TODO: 

            return null;
        }
    }
}