using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class ZMapRotationDto
    {
        /// <summary>
        /// 
        /// </summary>
        public ZMapDto Current { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ZMapDto Next { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<ZMapDto> Rotation { get; set; }
    }
}