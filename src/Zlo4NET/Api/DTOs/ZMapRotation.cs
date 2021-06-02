using System.Collections.Generic;

// ReSharper disable ClassNeverInstantiated.Global

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZMapRotation
    {
        /// <summary>
        /// 
        /// </summary>
        public ZMap Current { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZMap Next { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ZMap> Rotation { get; set; }
    }
}