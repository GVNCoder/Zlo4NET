namespace Zlo4NET.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public enum ZGameState
    {
        Unknown,

        StateEmpty,
        StateNA,
        StateError,
        StateStarting,
        StateInit,
        StateNotLoggedIn,
        StateMenuReady,
        StateMatchmaking,
        StateMatchmakeResultHost,
        StateMatchmakeResultJoin,
        StateConnectToGameId,
        StateConnectToUserId,
        StateCreateCoOpPeer,
        StateMatchmakeCoOp,
        StateResumeCampaign,
        StateLaunchPlayground,
        StateWeaponCustomization,
        StateLoadLevel,
        StateConnecting,
        StateWaitForLevel,
        StateGameLoading,
        StateGame,
        StateGameLeaving,
        StateInQueue,
        StateWaitForPeerClient,
        StatePeerClientConnected,
        StateClaimReservation,
        StateReady,
        StateGameRunning,
        StateGameClose
    }
}