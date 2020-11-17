namespace Zlo4NET.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public enum ZGameState
    {
        Unknown,

        State_Empty,
        State_GameRun,
        State_GameClose,

        State_NA,
        State_Error,
        State_Starting,
        State_Init,
        State_NotLoggedIn,
        State_MenuReady,
        State_Matchmaking,
        State_MatchmakeResultHost,
        State_MatchmakeResultJoin,
        State_ConnectToGameId,
        State_ConnectToUserId,
        State_CreateCoOpPeer,
        State_MatchmakeCoOp,
        State_ResumeCampaign,
        State_LaunchPlayground,
        State_WeaponCustomization,
        State_LoadLevel,
        State_Connecting,
        State_WaitForLevel,
        State_GameLoading,
        State_Game,
        State_GameLeaving,
        State_InQueue,
        State_WaitForPeerClient,
        State_PeerClientConnected,
        State_ClaimReservation,
        State_Ready
    }
}