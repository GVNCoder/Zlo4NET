using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZInstalledGame
    {
        /// <summary>
        /// 
        /// </summary>
        public ZGame Game { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReadableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InternalName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RunnableName { get; set; }
    }
}