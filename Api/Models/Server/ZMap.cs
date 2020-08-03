using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines server map
    /// </summary>
    public class ZMap : ZObservableObject
    {
        /// <summary>
        /// Gets name of map
        /// </summary>
        [ZObservableProperty]
        public string Name { get; set; }
        /// <summary>
        /// Gets name of game mode
        /// </summary>
        [ZObservableProperty]
        public string GameModeName { get; set; }

        public ZMapRole Role { get; set; }
    }
}