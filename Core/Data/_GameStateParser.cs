using System.Collections.Generic;

namespace Zlo4NET.Core.Data
{
    internal static class _GameStateParser
    {
        private const string __CallerStateChanged = "StateChanged";
        private const string __CallerAlert = "Alert";

        // where string is state name
        private static readonly IReadOnlyDictionary<string, ZGameState> _states = new Dictionary<string, ZGameState>
        {
            { "", ZGameState.StateEmpty },
            { "State_NA", ZGameState.StateNA },
            { "State_Error", ZGameState.StateError },
            { "State_Starting", ZGameState.StateStarting },
            { "State_Init", ZGameState.StateInit },
            { "State_NotLoggedIn", ZGameState.StateNotLoggedIn },
            { "State_MenuReady", ZGameState.StateMenuReady },
            { "State_Matchmaking", ZGameState.StateMatchmaking },
            { "State_MatchmakeResultHost", ZGameState.StateMatchmakeResultHost },
            { "State_MatchmakeResultJoin", ZGameState.StateMatchmakeResultJoin },
            { "State_ConnectToGameId", ZGameState.StateConnectToGameId },
            { "State_ConnectToUserId", ZGameState.StateConnectToUserId },
            { "State_CreateCoOpPeer", ZGameState.StateCreateCoOpPeer },
            { "State_MatchmakeCoOp", ZGameState.StateMatchmakeCoOp },
            { "State_ResumeCampaign", ZGameState.StateResumeCampaign },
            { "State_LaunchPlayground", ZGameState.StateLaunchPlayground },
            { "State_WeaponCustomization", ZGameState.StateWeaponCustomization },
            { "State_LoadLevel", ZGameState.StateLoadLevel },
            { "State_Connecting", ZGameState.StateConnecting },
            { "State_WaitForLevel", ZGameState.StateWaitForLevel },
            { "State_GameLoading", ZGameState.StateGameLoading },
            { "State_Game", ZGameState.StateGame },
            { "State_GameLeaving", ZGameState.StateGameLeaving },
            { "State_InQueue", ZGameState.StateInQueue },
            { "State_WaitForPeerClient", ZGameState.StateWaitForPeerClient },
            { "State_PeerClientConnected", ZGameState.StatePeerClientConnected },
            { "State_ClaimReservation", ZGameState.StateClaimReservation },
            { "State_Ready", ZGameState.StateReady },
            { "State_GameRunning", ZGameState.StateGameRunning },
            { "State_GameClosed", ZGameState.StateGameClose }
        };

        public static ZGameState GetStateByName(string stateName)
        {
            // try get state
            _states.TryGetValue(stateName, out var gameState);

            return gameState;
        }

        public static ZGameStateCaller GetCallerByName(string callerName)
        {
            switch (callerName)
            {
                case __CallerStateChanged:
                    return ZGameStateCaller.StateChanged;
                case __CallerAlert:
                    return ZGameStateCaller.Alert;
                default:
                    return ZGameStateCaller.Unknown;
            }
        }
    }
}