namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// It is an enumeration of possible events that can occur with the server list
    /// </summary>
    public enum ZServerListAction : byte
    {
        /// <summary>
        /// Occurs when server a new server added or an exists server updated
        /// </summary>
        ServerAddOrUpdate,
        /// <summary>
        /// Occurs when 
        /// </summary>
        ServerPlayersList,
        /// <summary>
        /// Occurs when exists server removed
        /// </summary>
        ServerRemove,
    }
}