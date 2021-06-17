using Zlo4NET.Api.Shared;

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
        public ZGameArchitecture Architecture { get; set; }
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