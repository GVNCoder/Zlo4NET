namespace Zlo4NET.ZClientAPI
{
    /// <summary>
    /// Represents ZClient commands identifiers
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