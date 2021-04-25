using System.Collections.Generic;
using System.Net;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// Represents an server DTO (Data Transfer Object)
    /// </summary>
    public class ZServerDto
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
        public ZServerAttributesDto Attributes { get; set; }
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
        public ZMapRotationDto MapRotation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ZPlayerDto> PlayersList { get; set; }
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