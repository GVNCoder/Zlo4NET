using System.Collections.Generic;
using System.Net;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// Represents an server DTO (Data Transfer Object)
    /// </summary>
    public class ZServer
    {
        /// <summary>
        /// 
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZGame TargetGame { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint ServerEndPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IPEndPoint ServerInEndPoint { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZServerAttributes Attributes { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Settings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> RawServerAttributesDictionary { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZMapRotation MapRotation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ZPlayer> PlayersList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CurrentPlayersCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PlayersCapacity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SpectatorsCapacity { get; set; }
    }
}