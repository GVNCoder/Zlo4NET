namespace Zlo4NET.Core.ZClient.Data
{
    /// <summary>
    /// Defines packet command identifier
    /// </summary>
    internal enum ZCommand : byte
    {
        Ping,
        UserInfo,
        PlayerInfo,
        ServerList,
        Stats,
        Items,
        Packs,
        Inject,
        GameList,
        RunGame,

        None = 255
    }
}